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

        public Task<bool> HasPermissionAsync(int userId, string permissionCode)
        {
            if (userId <= 0)
                return Task.FromResult(false);

            if (string.IsNullOrWhiteSpace(permissionCode))
                return Task.FromResult(false);

            return _permissionRepository.HasPermissionAsync(userId, permissionCode);
        }

        public Task<IReadOnlyList<string>> GetEffectivePermissionsAsync(int userId)
        {
            if (userId <= 0)
                throw new ArgumentException("User id must be greater than zero.");

            return _permissionRepository.GetEffectivePermissionsAsync(userId);
        }

        public async Task<IReadOnlyList<RolePermissionDto>> GetRolePermissionsAsync(int roleId)
        {
            var items = await _permissionRepository.GetRolePermissionsAsync(roleId);

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

        public Task UpdateRolePermissionsAsync(int roleId, UpdateRolePermissionsRequest request)
        {
            if (roleId <= 0)
                throw new ArgumentException("Role id must be greater than zero.");

            return _permissionRepository.ReplaceRolePermissionsAsync(
                roleId,
                request.MenuActionIds.Distinct().ToArray());
        }

        public async Task<IReadOnlyList<UserPermissionDto>> GetUserPermissionsAsync(int userId)
        {
            var items = await _permissionRepository.GetUserPermissionsAsync(userId);

            return items.Select(item => new UserPermissionDto
            {
                MenuActionId = item.MenuActionId,
                MenuCode = item.MenuCode,
                MenuNameAr = item.MenuNameAr,
                MenuNameEn = item.MenuNameEn,
                ActionNameAr = item.ActionNameAr,
                ActionNameEn = item.ActionNameEn,
                PermissionCode = item.PermissionCode,
                RoleAllowed = item.RoleAllowed,
                UserOverride = item.UserOverride,
                EffectiveAllowed = item.EffectiveAllowed
            }).ToList();
        }

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