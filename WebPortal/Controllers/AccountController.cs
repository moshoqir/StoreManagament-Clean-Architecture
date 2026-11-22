using System.Security.Claims;
using System.Text.Json;
using Application.DTOs.Authentication;
using Application.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebPortal.Common;

namespace WebPortal.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;

        public AccountController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Index()
        {
            // if the user authenticated, return it to the allowed page they have
            if (User.Identity?.IsAuthenticated == true)
                return Redirect(GetFirstAllowedPage(User));
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            HttpClient client = _clientFactory.CreateClient("WebApi");

            HttpResponseMessage response = await client.PostAsJsonAsync("api/auth/login", request);

            // we will read as string to deserialize it and then send a new key with json response
            string json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return new ContentResult
                {
                    Content = json,
                    ContentType = "application/json; charset=utf-8",
                    StatusCode = (int)response.StatusCode
                };
            }

            // Deserialize the success payload to extract User ID, Name, and Permissions
            var apiResponse = JsonSerializer.Deserialize<ApiEnvelope<LoginResponseDto>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (apiResponse?.Data is null)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new { code = 500, status = false, message = "Invalid login response from API." }
                    );
            }

            LoginResponseDto login = apiResponse.Data;

            // Generate the encrypted session cookie based on the API data
            ClaimsPrincipal principal = await SignInWebUserAsync(
                login.UserId,
                login.UserName,
                login.FullName,
                login.AccessToken,
                login.Permissions
                );

            return Json(new
            {
                code = 200,
                status = true,
                message = "Login Successfull",
                data = new
                {
                    redirectUrl = GetFirstAllowedPage(principal)
                }
            });
        }


        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetPermissions()
        {
            HttpClient client = _clientFactory.CreateClient("WebApi");
            ApiRequestHelper.AddBearerToken(client, User);

            HttpResponseMessage response = await client.GetAsync("api/auth/permissions");

            string json = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = JsonSerializer.Deserialize<ApiEnvelope<List<string>>>(
                    json, 
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (apiResponse?.Data is not null)
                {
                    // Re-sign the user in to silently refresh their claims cookie with the updated permissions
                    int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                    string userName = User.Identity?.Name ?? string.Empty;
                    string fullName = User.FindFirst("FullName")?.Value ?? userName;
                    string accessToken = User.FindFirst("AccessToken")?.Value ?? string.Empty;

                    await SignInWebUserAsync(
                        userId,
                        userName,
                        fullName,
                        accessToken,
                        apiResponse.Data);
                }
            }

            return new ContentResult
            {
                Content = json,
                ContentType = "application/json; charset=utf-8",
                StatusCode = (int)response.StatusCode
            };
        }


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return Json(new
            {
                code = 200,
                status = true,
                message = "Logged out successfully."
            });
        }


        [AllowAnonymous]
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        // Core method to build the Claims list and generate the Authentication Cookie
        private async Task<ClaimsPrincipal> SignInWebUserAsync(
            int userId,
            string userName,
            string fullName,
            string accessToken,
            IReadOnlyCollection<string> permissions)
        {
            // Standard user identity claims
            var claims = new List<Claim>
            {
                new (ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, userName),
                new Claim("FullName", fullName),
                new Claim("AccessToken", accessToken)
            };
            // Embed every individual permission as a separate claim in the cookie
            foreach (string permission in permissions)
                claims.Add(new Claim("Permission", permission));

             var identity = new ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults.AuthenticationScheme);

             var principal = new ClaimsPrincipal(identity);

            // Execute the sign-in command that creates the physical cookie in the browser
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = false, // Cookie deletes if browser closes (Session Cookie)
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
                });

            return principal;
             
        }


        // checking what is the page the authenticated user has and redirect to it 
        private static string GetFirstAllowedPage(ClaimsPrincipal user)
        {
            if (user.HasClaim("Permission", PermissionCodes.CategoryView))
                return "/Categories";

            if (user.HasClaim("Permission", PermissionCodes.ProductView))
                return "/Products";

            if (user.HasClaim("Permission", PermissionCodes.UsersView))
                return "/Users";

            if (user.HasClaim("Permission", PermissionCodes.RolesView))
                return "/Roles";

            return "/Account/AccessDenied";
        }

        public sealed class ApiEnvelope<T>
        {
            public int Code { get; set; }
            public bool Status { get; set; }
            public string Message { get; set; } = string.Empty;
            public T? Data { get; set; }
        }
    }
}
