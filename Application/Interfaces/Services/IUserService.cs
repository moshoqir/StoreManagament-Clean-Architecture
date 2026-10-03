using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.UserManagement;

namespace Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<IReadOnlyList<UserDto>> GetAllAsync();
        Task<UserDto?> GetByIdAsync(int userId);
        Task<int> CreateAsync(CreateUserRequest request);
        Task<bool> UpdateAsync(int userId, UpdateUserRequest request);
        Task<bool> DeleteAsync(int userId);
    }
}
