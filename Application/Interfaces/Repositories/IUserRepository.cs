using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<IReadOnlyList<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int userId);
        Task<User?> GetByUserNameAsync(string userName);
        Task<int> CreateAsync(User user);
        Task<bool> UpdateAsync(User user, string? newPasswordHash);
        Task<bool> DeleteAsync(int userId);
        Task ReplaceRolesAsync(int userId, IReadOnlyCollection<int> roleIds);
        Task UpdateLastLoginAsync(int userId);
    }
}
