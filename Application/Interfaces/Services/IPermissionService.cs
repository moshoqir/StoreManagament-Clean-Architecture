using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.UserManagement;

namespace Application.Interfaces.Services
{
    public interface IPermissionService
    {
        Task<bool> HasPermissionAsync(int userId, string permissionCode);
        Task<IReadOnlyList<RolePermissionDto>> GetRolePermissionsAsync(int roleId);
        Task UpdateRolePermissionsAsync(int roleId, UpdateRolePermissionsRequest request);
        Task<IReadOnlyList<UserPermissionDto>> GetUserPermissionsAsync(int userId);
        Task UpdateUserPermissionsAsync(int userId, UpdateUserPermissionsRequest request);
    }
}
