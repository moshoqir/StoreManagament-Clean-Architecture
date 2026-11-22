using Application.DTOs.Product;
using Application.Interfaces.Services;
using Application.Security;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Contracts.Common;
using WebApi.Security;

namespace WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public ProductsController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetAllAsync();

            return Ok(
                ApiResponse.Success(
                    products,
                    "Products retrieved successfully."
                    )
                );
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productService.GetByIdAsync(id);

            return Ok(
                ApiResponse.Success(
                    product,
                    "Product retrieved successfully."
                    )
                );
        }

        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetByCategoryId(int categoryId)
        {
            var products = await _productService.GetByCategoryIdAsync(categoryId);

            return Ok(
                ApiResponse.Success(
                    products,
                    "Products with the specified category retrieved successfully."

                    )
                );
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductRequest request)
        {
            var product = await _productService.CreateAsync(request);

            return Ok(
                ApiResponse.Success(
                    product,
                    "Product created successfully.",
                    201
                    )
                );
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProductRequest request)
        {
            var product = await _productService.UpdateAsync(id, request);

            return Ok(
                ApiResponse.Success(
                 
                    "Product updated successfully."
                    )
                );
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productService.DeleteAsync(id);

            return Ok(
                ApiResponse.Success(
                    "Product deleted successfully."
                    )
                );
        }


        // This api to get categories for products. We added it here and separate it from categories api because of the permissions. Sometimes the user may have the permission for products page, but doesn't for categories.
        [HttpGet("categories")]
        [Permission(PermissionCodes.ProductView)]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _categoryService.GetAllAsync();
            return Ok(ApiResponse.Success(categories, "Categories retrieved successfully."));
        }
    }
}
