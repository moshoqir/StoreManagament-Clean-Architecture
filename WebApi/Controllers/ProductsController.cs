using Application.DTOs.Product;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Contracts.Common;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
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
    }
}
