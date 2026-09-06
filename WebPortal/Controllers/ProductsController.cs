using Application.DTOs.Product;
using Microsoft.AspNetCore.Mvc;
using WebPortal.Common;

namespace WebPortal.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ProductsController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetData()
        {
            HttpClient client = _httpClientFactory.CreateClient("WebApi");

            HttpResponseMessage response = await client.GetAsync("api/Products");

            return await ApiResponseHelper.ToActionResultAsync(response);


        }

        public async Task<IActionResult> GetById(int id)
        {
            HttpClient client = _httpClientFactory.CreateClient("WebApi");

            HttpResponseMessage response = await client.GetAsync($"api/Products/{id}");

            return await ApiResponseHelper.ToActionResultAsync(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductRequest request)
        {
            HttpClient client = _httpClientFactory.CreateClient("WebApi");

            HttpResponseMessage response = await client.PostAsJsonAsync("api/Products", request);

            return await ApiResponseHelper.ToActionResultAsync(response);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProductRequest request)
        {
            HttpClient client = _httpClientFactory.CreateClient("WebApi");

            HttpResponseMessage response = await client.PostAsJsonAsync($"api/Products/{id}", request);

            return await ApiResponseHelper.ToActionResultAsync(response);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            HttpClient client = _httpClientFactory.CreateClient("WebApi");

            HttpResponseMessage response = await client.DeleteAsync($"api/Products/{id}");


            return await ApiResponseHelper.ToActionResultAsync(response);

        }
    }
}
