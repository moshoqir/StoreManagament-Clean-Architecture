using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface IPermissionRepository
    {
        Task<bool> HasPermissionAsync(int userId, string permissionCode);
        Task<IReadOnlyList<string>> GetEffectivePermissionsAsync(int userId);
        Task<IReadOnlyList<PermissionItem>> GetRolePermissionsAsync(int roleId);
        Task ReplaceRolePermissionsAsync(int roleId, IReadOnlyCollection<int> menuActionIds);
        Task<IReadOnlyList<PermissionItem>> GetUserPermissionsAsync(int userId);
        Task ReplaceUserPermissionsAsync(int userId, IReadOnlyCollection<(int MenuActionId, bool IsAllowed)> permissions);
    }
}
