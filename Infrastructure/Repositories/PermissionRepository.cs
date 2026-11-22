using Application.Interfaces.Repositories;
using Dapper;
using Domain.Entities;
using Infrastructure.Data;
using System.Data;

namespace Infrastructure.Repositories
{
   
    public sealed class PermissionRepository : IPermissionRepository
    {
        private readonly ISqlConnectionFactory _connectionFactory;

        public PermissionRepository(ISqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<bool> HasPermissionAsync(int userId, string permissionCode)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            return await connection.QuerySingleAsync<bool>(
                "[dbo].[USM_Permission_HasPermission]",
                new { UserId = userId, PermissionCode = permissionCode },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<IReadOnlyList<string>> GetEffectivePermissionsAsync(int userId)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            var permissions = await connection.QueryAsync<string>(
                "[dbo].[USM_Permission_GetEffectiveByUserId]",
                new { UserId = userId },
                commandType: CommandType.StoredProcedure);

            return permissions.ToList();
        }

        public async Task<IReadOnlyList<PermissionItem>> GetRolePermissionsAsync(int roleId)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            var permissions = await connection.QueryAsync<PermissionItem>(
                "[dbo].[USM_Permission_GetRoleMatrix]",
                new { RoleId = roleId },
                commandType: CommandType.StoredProcedure);

            return permissions.ToList();
        }

        public async Task ReplaceRolePermissionsAsync(int roleId, IReadOnlyCollection<int> menuActionIds)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                await connection.ExecuteAsync(
                    "[dbo].[USM_RolePermission_DeleteByRoleId]",
                    new { RoleId = roleId },
                    transaction,
                    commandType: CommandType.StoredProcedure);

                foreach (int menuActionId in menuActionIds)
                {
                    await connection.ExecuteAsync(
                        "[dbo].[USM_RolePermission_Add]",
                        new { RoleId = roleId, MenuActionId = menuActionId },
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

        public async Task<IReadOnlyList<PermissionItem>> GetUserPermissionsAsync(int userId)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            var permissions = await connection.QueryAsync<PermissionItem>(
                "[dbo].[USM_Permission_GetUserMatrix]",
                new { UserId = userId },
                commandType: CommandType.StoredProcedure);

            return permissions.ToList();
        }

        public async Task ReplaceUserPermissionsAsync(
            int userId,
            IReadOnlyCollection<(int MenuActionId, bool IsAllowed)> permissions)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                await connection.ExecuteAsync(
                    "[dbo].[USM_UserPermission_DeleteByUserId]",
                    new { UserId = userId },
                    transaction,
                    commandType: CommandType.StoredProcedure);

                foreach (var permission in permissions)
                {
                    await connection.ExecuteAsync(
                        "[dbo].[USM_UserPermission_Add]",
                        new
                        {
                            UserId = userId,
                            permission.MenuActionId,
                            permission.IsAllowed
                        },
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
    }
}
