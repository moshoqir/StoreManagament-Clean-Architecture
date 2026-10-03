using Application.DTOs.UserManagement;
using Application.Interfaces.Repositories;
using Application.Interfaces.Security;
using Application.Interfaces.Services;
using Domain.Entities;

namespace Application.Services
{

    public sealed class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        public UserService(IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<IReadOnlyList<UserDto>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(ToDto).ToList();
        }

        public async Task<UserDto?> GetByIdAsync(int userId)
        {
            if (userId <= 0)
                throw new ArgumentException("User id must be greater than zero.");

            var user = await _userRepository.GetByIdAsync(userId);
            return user is null ? null : ToDto(user);
        }

        public async Task<int> CreateAsync(CreateUserRequest request)
        {
            var user = new User
            {
                UserName = request.UserName.Trim(),
                FullName = request.FullName.Trim(),
                Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim(),
                PhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber) ? null : request.PhoneNumber.Trim(),
                PasswordHash = _passwordHasher.HashPassword(request.Password),
                IsFirstLogin = true,
                IsActive = request.IsActive,
                IsDelete = false
            };

            int userId = await _userRepository.CreateAsync(user);
            await _userRepository.ReplaceRolesAsync(userId, request.RoleIds.Distinct().ToArray());

            return userId;
        }

        public async Task<bool> UpdateAsync(int userId, UpdateUserRequest request)
        {
            if (userId <= 0)
                throw new ArgumentException("User id must be greater than zero.");

            var currentUser = await _userRepository.GetByIdAsync(userId);
            if (currentUser is null)
                return false;

            currentUser.FullName = request.FullName.Trim();
            currentUser.Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim();
            currentUser.PhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber) ? null : request.PhoneNumber.Trim();
            currentUser.IsActive = request.IsActive;

            string? newPasswordHash = string.IsNullOrWhiteSpace(request.NewPassword)
                ? null
                : _passwordHasher.HashPassword(request.NewPassword);

            bool updated = await _userRepository.UpdateAsync(currentUser, newPasswordHash);

            if (updated)
                await _userRepository.ReplaceRolesAsync(userId, request.RoleIds.Distinct().ToArray());

            return updated;
        }

        public async Task<bool> DeleteAsync(int userId)
        {
            if (userId <= 0)
                throw new ArgumentException("User id must be greater than zero.");

            return await _userRepository.DeleteAsync(userId);
        }

        private static UserDto ToDto(User user)
        {
            return new UserDto
            {
                UserId = user.UserId,
                UserName = user.UserName,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IsActive = user.IsActive,
                RoleIds = user.Roles.Select(role => role.RoleId).ToArray(),
                RoleNames = user.Roles.Select(role => role.RoleNameEn).ToArray()
            };
        }
    }
}