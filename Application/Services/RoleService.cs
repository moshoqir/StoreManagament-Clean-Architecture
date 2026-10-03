using Application.DTOs.UserManagement;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;

namespace Application.Services
{
    public sealed class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;

        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<IReadOnlyList<RoleDto>> GetAllAsync()
        {
            var roles = await _roleRepository.GetAllAsync();
            return roles.Select(ToDto).ToList();
        }

        public async Task<RoleDto?> GetByIdAsync(int roleId)
        {
            if (roleId <= 0)
                throw new ArgumentException("Role id must be greater than zero.");

            var role = await _roleRepository.GetByIdAsync(roleId);
            return role is null ? null : ToDto(role);
        }

        public async Task<int> CreateAsync(CreateRoleRequest request)
        {
            var role = new Role
            {
                RoleNameAr = request.RoleNameAr.Trim(),
                RoleNameEn = request.RoleNameEn.Trim(),
                IsActive = request.IsActive,
                IsDelete = false
            };

            return await _roleRepository.CreateAsync(role);
        }

        public async Task<bool> UpdateAsync(int roleId, UpdateRoleRequest request)
        {
            if (roleId <= 0)
                throw new ArgumentException("Role id must be greater than zero.");

            var role = new Role
            {
                RoleId = roleId,
                RoleNameAr = request.RoleNameAr.Trim(),
                RoleNameEn = request.RoleNameEn.Trim(),
                IsActive = request.IsActive
            };

            return await _roleRepository.UpdateAsync(role);
        }

        public async Task<bool> DeleteAsync(int roleId)
        {
            if (roleId <= 0)
                throw new ArgumentException("Role id must be greater than zero.");

            return await _roleRepository.DeleteAsync(roleId);
        }

        private static RoleDto ToDto(Role role)
        {
            return new RoleDto
            {
                RoleId = role.RoleId,
                RoleNameAr = role.RoleNameAr,
                RoleNameEn = role.RoleNameEn,
                IsActive = role.IsActive
            };
        }
    }
}