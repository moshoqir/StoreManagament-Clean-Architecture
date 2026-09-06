using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Product;

namespace Application.Interfaces.Services
{
    public interface IProductService
    {
        Task<IReadOnlyList<ProdcutDto>> GetAllAsync();

        Task<ProdcutDto?> GetByIdAsync(int productId);

        Task<IReadOnlyList<ProdcutDto>> GetByCategoryIdAsync(int categoryId);

        Task<int> CreateAsync(CreateProductRequest request);
        Task<bool> UpdateAsync(int productId, UpdateProductRequest request);
        
        Task<bool> DeleteAsync(int productId);
    }
}
