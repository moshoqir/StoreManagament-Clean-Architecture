using System.Text.Json;
using Application.DTOs.Category;
using Microsoft.AspNetCore.Mvc;
using WebPortal.Common;

namespace WebPortal.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CategoriesController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Index()
        {
            return View();
        }

        //[HttpPost]
        //public async Task<ResponseStandardJson<List<CategoryDto>>> GetData()
        //{
        //    // HttpClient is used to open any connect with any API. When using or building ani API, we must focus on : 1. Business 2. Security 3. The Method
        //    // HttpClient handles all the three of them.

        //    using (var client = new HttpClient())
        //    {
        //        // getting the endpoint
        //        var response = await client.GetAsync("http://localhost:57502/api/Categories");

        //        if (response.IsSuccessStatusCode)
        //        {
        //            // if success, get the content, read it, and get the result
        //            var result = response.Content.ReadAsStringAsync().Result;

        //            // same for this var, but we here only read the data of it to deserialize it later
        //            string responseBody = await response.Content.ReadAsStringAsync();

        //            // From the ResponseStandard, we will make a var (ResultData), and deserialize the body
        //            ResponseStandardJson<List<CategoryDto>>? ResultData = (ResponseStandardJson<List<CategoryDto>>?)JsonSerializer.Deserialize(responseBody, (typeof(ResponseStandardJson<List<CategoryDto>>)),
        //                // DON'T FORGET THAT MVC STANDARD RESPONSE JSON IS CAMEL CASE, SO WE MUST MAKE IT INSENSITIVE
        //                new JsonSerializerOptions
        //                {
        //                    PropertyNameCaseInsensitive = true
        //                }
        //                );



        //            return ResultData;
        //        }

        //        return new ResponseStandardJson<List<CategoryDto>>();
        //    }
        //}



        [HttpGet]
        //******************************************
        // There is a better way to make this method. The method above deserilizes the data, then returns it as JSON. HOWEVER, the response already returns as JSON. Therefore, we don't need to deserilize

        // 1. we register baseurl in appsettings
        // ==> In program.cs (WePortal layer):
        // 2. we register apiBaseUrl Configuration.
        // 3. we add HttpClient.
        // THEN
        // 4. we make ApiResponseHelper Class to avoid using deserialize and get the data as json directly
        // 5. Inject HttpClientFactory in the Controller ctor

        [HttpGet]
        public async Task<IActionResult> GetData()
        {
            // create new connection and name the project (layer) name we want to connect from
            HttpClient client = _httpClientFactory.CreateClient("WebApi");

            // read the response from the api
            HttpResponseMessage response = await client.GetAsync("api/Categories");

            // use the ApiResponseHelper's method we made
            return await ApiResponseHelper.ToActionResultAsync(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            HttpClient client = _httpClientFactory.CreateClient("WebApi");

            HttpResponseMessage response = await client.GetAsync($"api/Categories/{id}");

            return await ApiResponseHelper.ToActionResultAsync(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request)
        {
            HttpClient client = _httpClientFactory.CreateClient("WebApi");

            HttpResponseMessage response = await client.PostAsJsonAsync("api/Categories", request);

            return await ApiResponseHelper.ToActionResultAsync(response);
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromBody] UpdateCategoryRequest request, int id)
        {
            HttpClient client = _httpClientFactory.CreateClient("WebApi");

            HttpResponseMessage response = await client.PostAsJsonAsync($"api/Categories/{id}", request);

            return await ApiResponseHelper.ToActionResultAsync(response);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            HttpClient client = _httpClientFactory.CreateClient("WebApi");

            HttpResponseMessage response = await client.DeleteAsync($"api/Categories/{id}");

            return await ApiResponseHelper.ToActionResultAsync(response);
        }

    }

    public class ResponseStandardJson<T>
    {
        public int Code { get; set; }

        public bool Status { get; set; }

        public string Message { get; set; } = string.Empty;

        public T? Data { get; set; }

        public object? Errors { get; set; }
    }
}
