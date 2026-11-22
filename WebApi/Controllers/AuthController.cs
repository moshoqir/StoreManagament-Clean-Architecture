using System.Security.Claims;
using Application.DTOs.Authentication;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Contracts.Common;
using WebApi.Security;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public sealed class AuthController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IPermissionService _permissionService;
        private readonly JwtTokenService _jwtTokenService;
        private readonly RefreshTokenService _refreshTokenService;

        public AuthController(
            IAuthenticationService authenticationService,
            IPermissionService permissionService,
            JwtTokenService jwtTokenService,
            RefreshTokenService refreshTokenService)
        {
            _authenticationService = authenticationService;
            _permissionService = permissionService;
            _jwtTokenService = jwtTokenService;
            _refreshTokenService = refreshTokenService;
        }

         [AllowAnonymous]
         [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            AuthenticatedUserDto? user = await _authenticationService.LoginAsync(request);

            if (user is null)
            {
                return Unauthorized(
                    ApiResponse.Failure(
                        "Invalid user name or password.",
                        StatusCodes.Status401Unauthorized));
            }

            string accessToken = _jwtTokenService.CreateToken(user);

            // adding refreshToken
            string refreshToken = await _refreshTokenService.CreateAsync(user.UserId);

            // returning response
            var data = new LoginResponseDto
            {
                UserId = user.UserId,
                UserName = user.UserName,
                FullName = user.FullName,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                Permissions = user.Permissions
            };

            return Ok(ApiResponse.Success(data, "Login successful."));
        }

        [Authorize]
         [HttpGet("permissions")]
        public async Task<IActionResult> GetMyPermissions()
        {
            //string userIdValue = Request.Headers["X-User-Id"].ToString();

            // instead of sending the userId in the header, we must here extract it from the JWT service that we built:

            string? userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);


            // another way to get the userId, is from (JwtTokenService)

            // when token is sent like this, it'll show "Bearer eyj...." so we replace it
            //string token = Request.Headers["Authorization"].ToString().Replace("Bearer", "");

            //var data = _jwtTokenService.GetPrincipalFromToken(token);

            //string? userIds = data?.FindFirstValue(ClaimTypes.NameIdentifier);

            //string? userNames = data?.FindFirstValue(ClaimTypes.Name);
            //string? fullName = data?.FindFirstValue("FullName");

            if (!int.TryParse(userIdValue, out int userId) || userId <= 0)
            {
                return Unauthorized(
                    ApiResponse.Failure(
                        "User identity is required.",
                        StatusCodes.Status401Unauthorized));
            }

            IReadOnlyCollection<string> permissions =
                await _permissionService.GetEffectivePermissionsAsync(userId);

            return Ok(ApiResponse.Success(permissions, "Permissions retrieved successfully."));
        }

        // Adding refresh method for refresh token
        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
        {
            RefreshTokenResult? result = await _refreshTokenService.RotateAsync(request.RefreshToken);

            if (result is null)
            {
                return Unauthorized(ApiResponse.Failure("Invalid or expired refresh token.",
                        StatusCodes.Status401Unauthorized));
            }

            string accessToken = _jwtTokenService.CreateToken(result.User);

            var data = new LoginResponseDto
            {
                UserId = result.User.UserId,
                UserName = result.User.UserName,
                FullName = result.User.FullName,

                AccessToken = accessToken,
                RefreshToken = result.RefreshToken,

                Permissions = result.User.Permissions
            };

            return Ok(ApiResponse.Success(data, "Token refreshed successfully."));
        }
    }
}
