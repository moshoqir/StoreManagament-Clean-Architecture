using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Product;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;
using MapsterMapper;

namespace Application.Services
{
    internal class ProductService : IProductService
    {
        public ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository ,IMapper mapper)
        {
            ProductRepository = productRepository;
            CategoryRepository = categoryRepository;
            Mapper = mapper;
        }

        public IProductRepository ProductRepository { get; }
        public ICategoryRepository CategoryRepository { get; }
        public IMapper Mapper { get; }

        public async Task<int> CreateAsync(CreateProductRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            // check if category id exists
            //if(!await CategoryRepository.ExistsAsync(request.CategoryId))
            //{
            //    throw new NotFoundException($"category with id {request.CategoryId} was not found.");
            //}

            if (!string.IsNullOrWhiteSpace(request.ProductName))
            {
                request.ProductName = $"{char.ToUpper(request.ProductName[0])}{request.ProductName[1..]}".Trim();
            }

            var product = Mapper.Map<Product>(request);

            return await ProductRepository.CreateAsync(product);
        }

        public async Task<bool> DeleteAsync(int productId)
        {
            if(productId <= 0)
            {
                return false;
            }

            // check if id exists
            if(!await ProductRepository.ExistsAsync(productId))
            {
                return false;
            }

            return await ProductRepository.DeleteAsync(productId);
        }

        public async Task<IReadOnlyList<ProdcutDto>> GetAllAsync()
        {
            var products = await ProductRepository.GetAllAsync();

            return Mapper.Map<IReadOnlyList<ProdcutDto>>(products);
        }

        public async Task<IReadOnlyList<ProdcutDto>> GetByCategoryIdAsync(int categoryId)
        {
           if(categoryId <= 0)
           {
                return null;
           }

            var category = await ProductRepository.GetByCategoryIdAsync(categoryId);

            return category is null
                ? null
                : Mapper.Map<IReadOnlyList<ProdcutDto>>(category);

        }

        public async Task<ProdcutDto?> GetByIdAsync(int productId)
        {
            if(productId <= 0)
            {
                return null;
            }

            var product = await ProductRepository.GetByIdAsync(productId);

            return product is null
                ? null
                : Mapper.Map<ProdcutDto>(product);
        }

        public async Task<bool> UpdateAsync(int productId, UpdateProductRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            if(productId <= 0)
            {
                return false;

            }

            // check if id exists
            if(!await ProductRepository.ExistsAsync(productId))
            {
                return false;
            }

            // check if the CategoryId exists also

            //if(!await CategoryRepository.ExistsAsync(request.CategoryId))
            //{
            //    throw new NotFoundException($"category with id {request.CategoryId} was not found.");
            //}

            if (!string.IsNullOrWhiteSpace(request.ProductName))
            {
                request.ProductName = $"{char.ToUpper(request.ProductName[0])}{request.ProductName.Substring(1)}".Trim();
            }

            var product = Mapper.Map<Product>(request);

            product.ProductId = productId;

            return await ProductRepository.UpdateAsync(product);
        }
    }
}
