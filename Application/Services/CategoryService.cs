using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Category;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;
using MapsterMapper;

namespace Application.Services
{
    public class CategoryService : ICategoryService
    {
        public ICategoryRepository CategoryRepository { get; }
        public IMapper Mapper { get; }
        public CategoryService(ICategoryRepository _categoryRepository, IMapper _mapper)
        {
            CategoryRepository = _categoryRepository;
            Mapper = _mapper;
        }

       // In Services, we call IRepository in order to make it work with Entities, then we map with DTOs

       // IService deals only with DTOs--> We want to make request--> the request must come from
       // db-->Therefore, we need to call IRepository--> Mapping between them

        public async Task<int> CreateAsync(CreateCategoryRequest request)
        {

            // to check nullable
            ArgumentNullException.ThrowIfNull(request);

            // Capitalize first char and trim name
            if (!string.IsNullOrWhiteSpace(request.CategoryName))
            {
                request.CategoryName = $"{char.ToUpper(request.CategoryName[0])}{request.CategoryName.Substring(1)}".Trim();
            }

            // mapping request DTO to Entity
            var category = Mapper.Map<Category>(request);

            // return the result to Repository to create the category in db
            return await CategoryRepository.CreateAsync(category);

        }

        public async Task<bool> DeleteAsync(int categoryId)
        {
            if (categoryId <= 0)
                return false;

            return await CategoryRepository.DeleteAsync(categoryId);
             
        }

        public async Task<IReadOnlyList<CategoryDto>> GetAllAsync()
        {

            var categories = await CategoryRepository.GetAllAsync();

            return Mapper.Map<IReadOnlyList<CategoryDto>>(categories);
        }

        public async Task<CategoryDetailsDto?> GetByIdAsync(int categoryId)
        {
            // check if id valid
            if (categoryId <= 0)
            {
                return null;
            }

            // map from entity to DTO
            var category = await CategoryRepository.GetByIdWithProductsAsync(categoryId);

            return category is null
                ? null 
                : Mapper.Map<CategoryDetailsDto>(category);
        }

        public async Task<bool> UpdateAsync(int categoryId, UpdateCategoryRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            // capitalize and trim name
            if (!string.IsNullOrWhiteSpace(request.CategoryName))
            {
                request.CategoryName = $"{char.ToUpper(request.CategoryName[0])}{request.CategoryName.Substring(1)}".Trim();
            }


            // map from DTO to entity
            var category = Mapper.Map<Category>(request);

            // return the right id
            category.CategoryId = categoryId;

            return await CategoryRepository.UpdateAsync(category);
        }
    }
}
