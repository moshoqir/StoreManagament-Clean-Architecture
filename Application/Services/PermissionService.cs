using Application.DTOs.UserManagement;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;

namespace Application.Services
{

    public sealed class PermissionService : IPermissionService
    {
        private readonly IPermissionRepository _permissionRepository;

        public PermissionService(IPermissionRepository permissionRepository)
        {
            _permissionRepository = permissionRepository;
        }

        // this method checks if a user has a specific permission. It takes the userId and permissionCode as parameters and returns a boolean indicating whether the user has the specified permission.
        public Task<bool> HasPermissionAsync(int userId, string permissionCode)
        {
            if (userId <= 0)
                return Task.FromResult(false);

            if (string.IsNullOrWhiteSpace(permissionCode))
                return Task.FromResult(false);

            return _permissionRepository.HasPermissionAsync(userId, permissionCode);
        }


        // getting effective permissions for a user means getting all permissions assigned to the user directly and also all permissions assigned to the roles that the user belongs to. This is typically done by querying the database for the user's roles and their associated permissions, as well as any direct permissions assigned to the user.
        public Task<IReadOnlyList<string>> GetEffectivePermissionsAsync(int userId)
        {
            if (userId <= 0)
                throw new ArgumentException("User id must be greater than zero.");

            return _permissionRepository.GetEffectivePermissionsAsync(userId);
        }


        // this method to get the permissions assigned to a specific role. It takes the roleId as a parameter and returns a list of RolePermissionDto objects representing the permissions associated with that role.
        public async Task<IReadOnlyList<RolePermissionDto>> GetRolePermissionsAsync(int roleId)
        {
            // get role permissions
            var items = await _permissionRepository.GetRolePermissionsAsync(roleId);

            // map the items to RolePermissionDto objects and return the list
            return items.Select(item => new RolePermissionDto
            {
                MenuActionId = item.MenuActionId,
                MenuCode = item.MenuCode,
                MenuNameAr = item.MenuNameAr,
                MenuNameEn = item.MenuNameEn,
                ActionNameAr = item.ActionNameAr,
                ActionNameEn = item.ActionNameEn,
                PermissionCode = item.PermissionCode,
                IsAllowed = item.IsAllowed
            }).ToList();
        }

        // this method updates the permissions assigned to a specific role. It takes the roleId and an UpdateRolePermissionsRequest object as parameters. The UpdateRolePermissionsRequest contains a list of menu action IDs that represent the permissions to be assigned to the role.
        public Task UpdateRolePermissionsAsync(int roleId, UpdateRolePermissionsRequest request)
        {
            if (roleId <= 0)
                throw new ArgumentException("Role id must be greater than zero.");

            return _permissionRepository.ReplaceRolePermissionsAsync(
                roleId,
                request.MenuActionIds.Distinct().ToArray());
        }

        // this method to get the permissions assigned to a specific user. It takes the userId as a parameter and returns a list of UserPermissionDto objects representing the permissions associated with that user.
        public async Task<IReadOnlyList<UserPermissionDto>> GetUserPermissionsAsync(int userId)
        {
            // get user permissions
            var items = await _permissionRepository.GetUserPermissionsAsync(userId);

            // map
            return items.Select(item => new UserPermissionDto
            {
                MenuActionId = item.MenuActionId,
                MenuCode = item.MenuCode,
                MenuNameEn = item.MenuNameEn,
                MenuNameAr = item.MenuNameAr,
                ActionNameEn = item.ActionNameEn,
                ActionNameAr = item.ActionNameAr,
                PermissionCode = item.PermissionCode,
                RoleAllowed = item.RoleAllowed,
                UserOverride = item.UserOverride,
                EffectiveAllowed = item.EffectiveAllowed
            }).ToList();
        }


        // this method updates the permissions assigned to a specific user. It takes the userId and an UpdateUserPermissionsRequest object as parameters. The UpdateUserPermissionsRequest contains a list of UserPermissionItemRequest objects that represent the permissions to be assigned to the user.
        public Task UpdateUserPermissionsAsync(int userId, UpdateUserPermissionsRequest request)
        {
            if (userId <= 0)
                throw new ArgumentException("User id must be greater than zero.");

            var overrides = request.Permissions
               .Where(item => item.IsAllowed.HasValue)
               .GroupBy(item => item.MenuActionId)
               .Select(group =>
               {
                   var item = group.Last();
                   return (item.MenuActionId, item.IsAllowed!.Value);
               })
               .ToArray();

            return _permissionRepository.ReplaceUserPermissionsAsync(userId, overrides);
        }
    }
}