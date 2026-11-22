using Application.DTOs.UserManagement;
using Application.Interfaces.Services;
using Application.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Contracts.Common;
using WebApi.Security;

namespace WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/users")]
    public sealed class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IPermissionService _permissionService;

        public UsersController(
            IUserService userService,
            IRoleService roleService,
            IPermissionService permissionService)
        {
            _userService = userService;
            _roleService = roleService;
            _permissionService = permissionService;
        }

        [HttpGet]
        [Permission(PermissionCodes.UsersView)]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllAsync();
            return Ok(ApiResponse.Success(users, "Users retrieved successfully."));
        }

        [HttpGet("{userId:int}")]
        [Permission(PermissionCodes.UsersView)]
        public async Task<IActionResult> GetById(int userId)
        {
            var user = await _userService.GetByIdAsync(userId);

            if (user is null)
                return NotFound(ApiResponse.Failure("User was not found.", StatusCodes.Status404NotFound));

            return Ok(ApiResponse.Success(user, "User retrieved successfully."));
        }

        [HttpPost]
        [Permission(PermissionCodes.UsersCreate)]
        public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
        {
            int userId = await _userService.CreateAsync(request);

            return CreatedAtAction(
                nameof(GetById),
                new { userId },
                ApiResponse.Success(
                    new { UserId = userId },
                    "User created successfully.",
                    StatusCodes.Status201Created));
        }

        [HttpPut("{userId:int}")]
        [Permission(PermissionCodes.UsersUpdate)]
        public async Task<IActionResult> Update(int userId, [FromBody] UpdateUserRequest request)
        {
            bool updated = await _userService.UpdateAsync(userId, request);

            if (!updated)
                return NotFound(ApiResponse.Failure("User was not found.", StatusCodes.Status404NotFound));

            return Ok(ApiResponse.Success("User updated successfully."));
        }

        [HttpDelete("{userId:int}")]
        [Permission(PermissionCodes.UsersDelete)]
        public async Task<IActionResult> Delete(int userId)
        {
            bool deleted = await _userService.DeleteAsync(userId);

            if (!deleted)
                return NotFound(ApiResponse.Failure("User was not found.", StatusCodes.Status404NotFound));

            return Ok(ApiResponse.Success("User deleted successfully."));
        }

        // Dropdown Roles داخل شاشة Users.
        [HttpGet("available-roles")]
        [Permission(PermissionCodes.UsersView)]
        public async Task<IActionResult> GetAvailableRoles()
        {
            var roles = await _roleService.GetAllAsync();
            return Ok(ApiResponse.Success(roles, "Roles retrieved successfully."));
        }

        // User Override: null = Use Role, true = Allow, false = Deny.
        [HttpGet("{userId:int}/permissions")]
        [Permission(PermissionCodes.UsersManagePermissions)]
        public async Task<IActionResult> GetPermissions(int userId)
        {
            var user = await _userService.GetByIdAsync(userId);
            if (user is null)
                return NotFound(ApiResponse.Failure("User was not found.", StatusCodes.Status404NotFound));

            var permissions = await _permissionService.GetUserPermissionsAsync(userId);
            return Ok(ApiResponse.Success(permissions, "User permissions retrieved successfully."));
        }

        [HttpPut("{userId:int}/permissions")]
        [Permission(PermissionCodes.UsersManagePermissions)]
        public async Task<IActionResult> UpdatePermissions(
            int userId,
            [FromBody] UpdateUserPermissionsRequest request)
        {
            var user = await _userService.GetByIdAsync(userId);
            if (user is null)
                return NotFound(ApiResponse.Failure("User was not found.", StatusCodes.Status404NotFound));

            await _permissionService.UpdateUserPermissionsAsync(userId, request);
            return Ok(ApiResponse.Success("User permissions updated successfully."));
        }
    }
}
