using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Category;

namespace Application.Interfaces.Services
{
    public interface ICategoryService
    {
        Task<IReadOnlyList<CategoryDto>> GetAllAsync();

        Task<CategoryDetailsDto?> GetByIdAsync(int categoryId);

        Task<int> CreateAsync(CreateCategoryRequest request);

        Task<bool> UpdateAsync(int categoryId, UpdateCategoryRequest request);

        Task<bool> DeleteAsync(int categoryId);
    }
}
