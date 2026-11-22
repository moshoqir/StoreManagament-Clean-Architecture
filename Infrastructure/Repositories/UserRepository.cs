using Application.Common.Exceptions;
using Application.Interfaces.Repositories;
using Dapper;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Infrastructure.Repositories
{
   
    public sealed class UserRepository : IUserRepository
    {
        private readonly ISqlConnectionFactory _connectionFactory;

        public UserRepository(ISqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IReadOnlyList<User>> GetAllAsync()
        {
            await using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            // this to get the user with their id and then role
            var users = new Dictionary<int, User>();

            await connection.QueryAsync<User, Role, User>(
                "[dbo].[USM_User_GetAll]",
                (user, role) => AddUserRole(users, user, role),
                splitOn: "RoleId",
                commandType: CommandType.StoredProcedure);

            return users.Values.ToList();
        }

        public async Task<User?> GetByIdAsync(int userId)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            var users = new Dictionary<int, User>();

            await connection.QueryAsync<User, Role, User>(
                "[dbo].[USM_User_GetById]",
                (user, role) => AddUserRole(users, user, role),
                new { UserId = userId },
                splitOn: "RoleId",
                commandType: CommandType.StoredProcedure);

            return users.Values.SingleOrDefault();
        }

        public async Task<User?> GetByUserNameAsync(string userName)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            return await connection.QuerySingleOrDefaultAsync<User>(
                "[dbo].[USM_User_GetByUserName]",
                new { UserName = userName },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<int> CreateAsync(User user)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            try
            {
                return await connection.QuerySingleAsync<int>(
                    "[dbo].[USM_User_Create]",
                    new
                    {
                        user.UserName,
                        user.FullName,
                        user.Email,
                        user.PhoneNumber,
                        user.PasswordHash,
                        user.ImagePath,
                        user.IsFirstLogin,
                        user.IsActive
                    },
                    commandType: CommandType.StoredProcedure);
            }
            catch (SqlException exception) when (exception.Number is 2601 or 2627)
            {
                throw new ConflictException("User name already exists.", exception);
            }
        }

        public async Task<bool> UpdateAsync(User user, string? newPasswordHash)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            return await connection.QuerySingleAsync<bool>(
                "[dbo].[USM_User_Update]",
                new
                {
                    user.UserId,
                    user.FullName,
                    user.Email,
                    user.PhoneNumber,
                    PasswordHash = newPasswordHash,
                    user.IsActive
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<bool> DeleteAsync(int userId)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            return await connection.QuerySingleAsync<bool>(
                "[dbo].[USM_User_Delete]",
                new { UserId = userId },
                commandType: CommandType.StoredProcedure);
        }

        public async Task ReplaceRolesAsync(int userId, IReadOnlyCollection<int> roleIds)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                await connection.ExecuteAsync(
                    "[dbo].[USM_UserRole_DeleteByUserId]",
                    new { UserId = userId },
                    transaction,
                    commandType: CommandType.StoredProcedure);

                foreach (int roleId in roleIds)
                {
                    await connection.ExecuteAsync(
                        "[dbo].[USM_UserRole_Add]",
                        new { UserId = userId, RoleId = roleId },
                        transaction,
                        commandType: CommandType.StoredProcedure);
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task UpdateLastLoginAsync(int userId)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            await connection.ExecuteAsync(
                "[dbo].[USM_User_UpdateLastLogin]",
                new { UserId = userId },
                commandType: CommandType.StoredProcedure);
        }

        private static User AddUserRole(Dictionary<int, User> users, User user, Role? role)
        {
            if (!users.TryGetValue(user.UserId, out User? currentUser))
            {
                currentUser = user;
                currentUser.Roles = [];
                users.Add(currentUser.UserId, currentUser);
            }

            if (role is not null && role.RoleId > 0 && currentUser.Roles.All(item => item.RoleId != role.RoleId))
                currentUser.Roles.Add(role);

            return currentUser;
        }
    }
}
