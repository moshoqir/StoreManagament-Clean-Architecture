using System.Net.Http;
using Application.DTOs.UserManagement;
using Application.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebPortal.Common;

namespace WebPortal.Controllers
{
    [Authorize]
    public sealed class RolesController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;

        public RolesController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [HttpGet]
        [Permission(PermissionCodes.RolesView)]
        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        [Permission(PermissionCodes.RolesView)]
        public async Task<IActionResult> GetList()
        {
            HttpClient client = CreateApiClient();
            HttpResponseMessage response = await client.GetAsync("api/roles");
            return await ApiResponseHelper.ToActionResultAsync(response);
        }

        [HttpGet]
        [Permission(PermissionCodes.RolesView)]
        public async Task<IActionResult> GetById(int roleId)
        {
            HttpClient client = CreateApiClient();
            HttpResponseMessage response = await client.GetAsync($"api/roles/{roleId}");
            return await ApiResponseHelper.ToActionResultAsync(response);
        }

        [HttpPost]
        [Permission(PermissionCodes.RolesCreate)]
        public async Task<IActionResult> Create([FromBody] CreateRoleRequest request)
        {
            HttpClient client = CreateApiClient();
            HttpResponseMessage response = await client.PostAsJsonAsync("api/roles", request);
            return await ApiResponseHelper.ToActionResultAsync(response);
        }

        [HttpPut]
        [Permission(PermissionCodes.RolesUpdate)]
        public async Task<IActionResult> Update(int roleId, [FromBody] UpdateRoleRequest request)
        {
            HttpClient client = CreateApiClient();
            HttpResponseMessage response = await client.PutAsJsonAsync($"api/roles/{roleId}", request);
            return await ApiResponseHelper.ToActionResultAsync(response);
        }

        [HttpDelete]
        [Permission(PermissionCodes.RolesDelete)]
        public async Task<IActionResult> Delete(int roleId)
        {
            HttpClient client = CreateApiClient();
            HttpResponseMessage response = await client.DeleteAsync($"api/roles/{roleId}");
            return await ApiResponseHelper.ToActionResultAsync(response);
        }

        [HttpGet]
        [Permission(PermissionCodes.RolesManagePermissions)]
        public async Task<IActionResult> GetPermissions(int roleId)
        {
            HttpClient client = CreateApiClient();
            HttpResponseMessage response = await client.GetAsync($"api/roles/{roleId}/permissions");
            return await ApiResponseHelper.ToActionResultAsync(response);
        }

        [HttpPut]
        [Permission(PermissionCodes.RolesManagePermissions)]
        public async Task<IActionResult> UpdatePermissions(
            int roleId,
            [FromBody] UpdateRolePermissionsRequest request)
        {
            HttpClient client = CreateApiClient();
            HttpResponseMessage response = await client.PutAsJsonAsync($"api/roles/{roleId}/permissions", request);
            return await ApiResponseHelper.ToActionResultAsync(response);
        }

        private HttpClient CreateApiClient()
        {
            HttpClient client = _clientFactory.CreateClient("WebApi");
            ApiRequestHelper.AddBearerToken(client, User);
            return client;
        }
    }
}
