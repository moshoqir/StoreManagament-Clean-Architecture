using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;


namespace Application.Interfaces.Repositories
{
    public interface ICategoryRepository
    {
        Task<IReadOnlyList<Category>> GetAllAsync();

        Task<Category?> GetByIdAsync(int categoryId);

        Task<Category?> GetByIdWithProductsAsync(int categoryId);

        Task<bool> ExistsAsync(int categoryId);

        Task<int> CreateAsync(Category category);

        Task<bool> UpdateAsync(Category category);

        Task<bool> DeleteAsync(int categoryId);
    }
}
