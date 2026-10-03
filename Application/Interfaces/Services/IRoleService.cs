using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.UserManagement;

namespace Application.Interfaces.Services
{
    public interface IRoleService
    {
        Task<IReadOnlyList<RoleDto>> GetAllAsync();
        Task<RoleDto?> GetByIdAsync(int roleId);
        Task<int> CreateAsync(CreateRoleRequest request);
        Task<bool> UpdateAsync(int roleId, UpdateRoleRequest request);
        Task<bool> DeleteAsync(int roleId);
    }
}
