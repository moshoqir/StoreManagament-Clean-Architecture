using Application.DTOs.Authentication;
using Application.Interfaces.Repositories;
using Domain.Entities;
using System.Security.Cryptography;
using System.Text;

namespace WebApi.Security
{
    public sealed class RefreshTokenService
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IConfiguration _configuration;

        public RefreshTokenService(
            IRefreshTokenRepository refreshTokenRepository,
            IUserRepository userRepository,
            IPermissionRepository permissionRepository,
            IConfiguration configuration)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _userRepository = userRepository;
            _permissionRepository = permissionRepository;
            _configuration = configuration;
        }

        public async Task<string> CreateAsync(int userId)
        {
            // generate the refreshToken
            string refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

            // hash it using HashToken fun we made
            string tokenHash = HashToken(refreshToken);

            int expireDays = int.Parse(_configuration["Jwt:RefreshTokenExpireDays"] ?? "7");

            var entity = new UserRefreshToken
            {
                UserId = userId,
                TokenHash = tokenHash,
                CreatedAtUtc = DateTime.UtcNow,
                ExpiresAtUtc = DateTime.UtcNow.AddDays(expireDays)
            };

            await _refreshTokenRepository.CreateAsync(entity);

            return refreshToken;
        }

        public async Task<RefreshTokenResult?> RotateAsync(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                return null;


            string tokenHash = HashToken(refreshToken);

            UserRefreshToken? storedToken =
                await _refreshTokenRepository.GetByHashAsync(tokenHash);

            if (storedToken is null)
                return null;

            // Delete the token directly if it has already expired
            if (storedToken.ExpiresAtUtc <= DateTime.UtcNow)
            {
                await _refreshTokenRepository.DeleteAsync(
                    storedToken.RefreshTokenId);

                return null;
            }


            var user = await _userRepository.GetByIdAsync(storedToken.UserId);

            if (user is null || !user.IsActive || user.IsDelete)
            {
                await _refreshTokenRepository.DeleteAsync(storedToken.RefreshTokenId);
                return null;
            }

            await _refreshTokenRepository.DeleteAsync(
                storedToken.RefreshTokenId);

            string newRefreshToken = await CreateAsync(user.UserId);

            IReadOnlyCollection<string> permissions = await _permissionRepository.GetEffectivePermissionsAsync(user.UserId);

            var authenticatedUser = new AuthenticatedUserDto
            {
                UserId = user.UserId,
                UserName = user.UserName,
                FullName = user.FullName,
                Permissions = permissions
            };

            return new RefreshTokenResult
            {
                User = authenticatedUser,
                RefreshToken = newRefreshToken
            };
        }

        private static string HashToken(string refreshToken)
        {
            byte[] bytes =
                Encoding.UTF8.GetBytes(refreshToken);

            byte[] hash =
                SHA256.HashData(bytes);

            return Convert.ToHexString(hash);
        }
    }


    public sealed class RefreshTokenResult
    {
        public AuthenticatedUserDto User { get; init; } = new();

        public string RefreshToken { get; init; } = string.Empty;
    }
}