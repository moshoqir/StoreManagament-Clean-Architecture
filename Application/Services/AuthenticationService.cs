using Application.DTOs.Authentication;
using Application.Interfaces.Repositories;
using Application.Interfaces.Security;
using Application.Interfaces.Services;

namespace Application.Services
{

    public sealed class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IPasswordHasher _passwordHasher;

        public AuthenticationService(
            IUserRepository userRepository,
            IPermissionRepository permissionRepository,
            IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _permissionRepository = permissionRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<AuthenticatedUserDto?> LoginAsync(LoginRequest request)
        {
            var user = await _userRepository.GetByUserNameAsync(request.UserName.Trim());

            if (user is null || !user.IsActive || user.IsDelete)
                return null;

            if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
                return null;

            var permissions = await _permissionRepository.GetEffectivePermissionsAsync(user.UserId);
            await _userRepository.UpdateLastLoginAsync(user.UserId);

            return new AuthenticatedUserDto
            {
                UserId = user.UserId,
                UserName = user.UserName,
                FullName = user.FullName,
                Permissions = permissions
            };
        }
    }
}