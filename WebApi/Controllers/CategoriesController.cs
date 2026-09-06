using Application.DTOs.Category;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Contracts.Common;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public sealed class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoryService.GetAllAsync();

            return Ok(
                ApiResponse.Success(
                    categories,
                    "Categories retrieved successfully"
                )
            );
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var category =await _categoryService.GetByIdAsync(id);

            return Ok(
                ApiResponse.Success(
                    category,
                    "Category retrieved successfully"
                    )
                );
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryRequest request)
        {
            var category = await _categoryService.CreateAsync(request);

            return Ok(
                ApiResponse.Success(
                    category,
                    "Category created successfully",
                    201
                    )
                );
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> Update(int id, UpdateCategoryRequest request)
        {
            var category = await _categoryService.UpdateAsync(id, request);

            return Ok(
                ApiResponse.Success(
                    "Category updated successfully"
                    )
                );
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categoryService.DeleteAsync(id);

            return Ok(
                ApiResponse.Success(
                    "Category deleted successfully"
                    )
                );
        }

    }
}
