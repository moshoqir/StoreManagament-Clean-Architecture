using Application.Common.Exceptions;
using Application.Interfaces.Repositories;
using Dapper;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Infrastructure.Repositories
{
    public sealed class RoleRepository : IRoleRepository
    {
        private readonly ISqlConnectionFactory _connectionFactory;

        public RoleRepository(ISqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IReadOnlyList<Role>> GetAllAsync()
        {
            await using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            var roles = await connection.QueryAsync<Role>(
                "[dbo].[USM_Role_GetAll]",
                commandType: CommandType.StoredProcedure);

            return roles.ToList();
        }

        public async Task<Role?> GetByIdAsync(int roleId)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            return await connection.QuerySingleOrDefaultAsync<Role>(
                "[dbo].[USM_Role_GetById]",
                new { RoleId = roleId },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<int> CreateAsync(Role role)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            try
            {
                return await connection.QuerySingleAsync<int>(
                    "[dbo].[USM_Role_Create]",
                    new { role.RoleNameAr, role.RoleNameEn, role.IsActive },
                    commandType: CommandType.StoredProcedure);
            }
            catch (SqlException exception) when (exception.Number is 2601 or 2627)
            {
                throw new ConflictException("Role name already exists.", exception);
            }
        }

        public async Task<bool> UpdateAsync(Role role)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            try
            {
                return await connection.QuerySingleAsync<bool>(
                    "[dbo].[USM_Role_Update]",
                    new { role.RoleId, role.RoleNameAr, role.RoleNameEn, role.IsActive },
                    commandType: CommandType.StoredProcedure);
            }
            catch (SqlException exception) when (exception.Number is 2601 or 2627)
            {
                throw new ConflictException("Role name already exists.", exception);
            }
        }

        public async Task<bool> DeleteAsync(int roleId)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            try
            {
                return await connection.QuerySingleAsync<bool>(
                    "[dbo].[USM_Role_Delete]",
                    new { RoleId = roleId },
                    commandType: CommandType.StoredProcedure);
            }
            catch (SqlException exception) when (exception.Number == 51000)
            {
                throw new ConflictException("Role cannot be deleted because it is assigned to users.", exception);
            }
        }
    }
}
