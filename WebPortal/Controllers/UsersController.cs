using Application.DTOs.UserManagement;
using Application.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebPortal.Common;

namespace WebPortal.Controllers
{

    [Authorize]
    public sealed class UsersController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public UsersController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        [Permission(PermissionCodes.UsersView)]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Permission(PermissionCodes.UsersView)]
        public async Task<IActionResult> GetList()
        {
            HttpClient client = CreateApiClient();
            HttpResponseMessage response = await client.GetAsync("api/users");
            return await ApiResponseHelper.ToActionResultAsync(response);
        }

        [HttpGet]
        [Permission(PermissionCodes.UsersView)]
        public async Task<IActionResult> GetById(int userId)
        {
            HttpClient client = CreateApiClient();
            HttpResponseMessage response = await client.GetAsync($"api/users/{userId}");
            return await ApiResponseHelper.ToActionResultAsync(response);
        }

        [HttpGet]
        [Permission(PermissionCodes.UsersView)]
        public async Task<IActionResult> GetRoles()
        {
            HttpClient client = CreateApiClient();
            HttpResponseMessage response = await client.GetAsync("api/users/available-roles");
            return await ApiResponseHelper.ToActionResultAsync(response);
        }

        [HttpPost]
        [Permission(PermissionCodes.UsersCreate)]
        public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
        {
            HttpClient client = CreateApiClient();
            HttpResponseMessage response = await client.PostAsJsonAsync("api/users", request);
            return await ApiResponseHelper.ToActionResultAsync(response);
        }

        [HttpPut]
        [Permission(PermissionCodes.UsersUpdate)]
        public async Task<IActionResult> Update(int userId, [FromBody] UpdateUserRequest request)
        {
            HttpClient client = CreateApiClient();
            HttpResponseMessage response = await client.PutAsJsonAsync($"api/users/{userId}", request);
            return await ApiResponseHelper.ToActionResultAsync(response);
        }

        [HttpDelete]
        [Permission(PermissionCodes.UsersDelete)]
        public async Task<IActionResult> Delete(int userId)
        {
            HttpClient client = CreateApiClient();
            HttpResponseMessage response = await client.DeleteAsync($"api/users/{userId}");
            return await ApiResponseHelper.ToActionResultAsync(response);
        }

        [HttpGet]
        [Permission(PermissionCodes.UsersManagePermissions)]
        public async Task<IActionResult> GetPermissions(int userId)
        {
            HttpClient client = CreateApiClient();
            HttpResponseMessage response = await client.GetAsync($"api/users/{userId}/permissions");
            return await ApiResponseHelper.ToActionResultAsync(response);
        }

        [HttpPut]
        [Permission(PermissionCodes.UsersManagePermissions)]
        public async Task<IActionResult> UpdatePermissions(
            int userId,
            [FromBody] UpdateUserPermissionsRequest request)
        {
            HttpClient client = CreateApiClient();
            HttpResponseMessage response = await client.PutAsJsonAsync($"api/users/{userId}/permissions", request);
            return await ApiResponseHelper.ToActionResultAsync(response);
        }

        private HttpClient CreateApiClient()
        {
            HttpClient client = _httpClientFactory.CreateClient("WebApi");
            ApiRequestHelper.AddBearerToken(client, User);
            return client;
        }
    }
}
