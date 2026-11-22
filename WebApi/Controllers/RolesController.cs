using System.Security.Claims;
using Application.DTOs.UserManagement;
using Application.Interfaces.Services;
using Application.Security;
using Microsoft.AspNetCore.Mvc;
using WebApi.Contracts.Common;
using WebApi.Security;

namespace WebApi.Controllers
{
     
    [ApiController]
    [Route("api/roles")]
    public sealed class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly IPermissionService _permissionService;

        public RolesController(IRoleService roleService, IPermissionService permissionService)
        {
            _roleService = roleService;
            _permissionService = permissionService;
        }

        [HttpGet]
        [Permission(PermissionCodes.RolesView)]
        public async Task<IActionResult> GetAll()
        {
           
            var roles = await _roleService.GetAllAsync();
            return Ok(ApiResponse.Success(roles, "Roles retrieved successfully."));
        }

        [HttpGet("{roleId:int}")]
        [Permission(PermissionCodes.RolesView)]
        public async Task<IActionResult> GetById(int roleId)
        {
            var role = await _roleService.GetByIdAsync(roleId);

            if (role is null)
                return NotFound(ApiResponse.Failure("Role was not found.", StatusCodes.Status404NotFound));

            return Ok(ApiResponse.Success(role, "Role retrieved successfully."));
        }

        [HttpPost]
        [Permission(PermissionCodes.RolesCreate)]
        public async Task<IActionResult> Create([FromBody] CreateRoleRequest request)
        {
            int roleId = await _roleService.CreateAsync(request);

            return CreatedAtAction(
                nameof(GetById),
                new { roleId },
                ApiResponse.Success(
                    new { RoleId = roleId },
                    "Role created successfully.",
                    StatusCodes.Status201Created));
        }

        [HttpPut("{roleId:int}")]
        [Permission(PermissionCodes.RolesUpdate)]
        public async Task<IActionResult> Update(int roleId, [FromBody] UpdateRoleRequest request)
        {
            bool updated = await _roleService.UpdateAsync(roleId, request);

            if (!updated)
                return NotFound(ApiResponse.Failure("Role was not found.", StatusCodes.Status404NotFound));

            return Ok(ApiResponse.Success("Role updated successfully."));
        }

        [HttpDelete("{roleId:int}")]
        [Permission(PermissionCodes.RolesDelete)]
        public async Task<IActionResult> Delete(int roleId)
        {
            bool deleted = await _roleService.DeleteAsync(roleId);

            if (!deleted)
                return NotFound(ApiResponse.Failure("Role was not found.", StatusCodes.Status404NotFound));

            return Ok(ApiResponse.Success("Role deleted successfully."));
        }

        [HttpGet("{roleId:int}/permissions")]
        [Permission(PermissionCodes.RolesManagePermissions)]
        public async Task<IActionResult> GetPermissions(int roleId)
        {
            var role = await _roleService.GetByIdAsync(roleId);
            if (role is null)
                return NotFound(ApiResponse.Failure("Role was not found.", StatusCodes.Status404NotFound));

            var permissions = await _permissionService.GetRolePermissionsAsync(roleId);
            return Ok(ApiResponse.Success(permissions, "Role permissions retrieved successfully."));
        }

        [HttpPut("{roleId:int}/permissions")]
        [Permission(PermissionCodes.RolesManagePermissions)]
        public async Task<IActionResult> UpdatePermissions(
            int roleId,
            [FromBody] UpdateRolePermissionsRequest request)
        {
            var role = await _roleService.GetByIdAsync(roleId);
            if (role is null)
                return NotFound(ApiResponse.Failure("Role was not found.", StatusCodes.Status404NotFound));

            await _permissionService.UpdateRolePermissionsAsync(roleId, request);
            return Ok(ApiResponse.Success("Role permissions updated successfully."));
        }
    }
}
