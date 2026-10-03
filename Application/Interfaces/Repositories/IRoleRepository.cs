using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface IRoleRepository
    {
        Task<IReadOnlyList<Role>> GetAllAsync();
        Task<Role?> GetByIdAsync(int roleId);
        Task<int> CreateAsync(Role role);
        Task<bool> UpdateAsync(Role role);
        Task<bool> DeleteAsync(int roleId);
    }
}
