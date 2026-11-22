using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// apiBaseUrl Configuration (Extract the base URL for the backend Web API from appsettings.json)
string apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? throw new InvalidOperationException("ApiSettings: BaseUrl is missing from appsettings.json");

builder.Services.AddControllersWithViews().AddJsonOptions(x =>
{
    x.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
});

// Register a named and centralized HttpClient ("WebApi") to communicate with the backend
builder.Services.AddHttpClient("WebApi", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});


// Configure Cookie-based Authentication for the frontend portal
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true; // Renews expiration if the user is active

        options.Events.OnRedirectToLogin = async context =>
        {
            // If the request was made via AJAX, return a JSON error instead of an HTML redirect
            if (IsAjaxRequest(context.Request))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new
                {
                    code = 401,
                    status = false,
                    message = "Your session has expired. Please login again."
                });
                return;
            }
            // Standard page requests redirect to the login page
            context.Response.Redirect(context.RedirectUri);
        };

        options.Events.OnRedirectToAccessDenied = async context =>
        {
            if (IsAjaxRequest(context.Request))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;

                await context.Response.WriteAsJsonAsync(new
                {
                    code = 403,
                    status = false,
                    message = "You do not have permission to perform this action."
                });
            }

            context.Response.Redirect(context.RedirectUri);
        };

    });

builder.Services.AddAuthorization();


var app = builder.Build();

app.UseHttpsRedirection();


app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Index}/{id?}");


app.Run();


// Helper method to detect if an incoming request is an AJAX call (sent by jQuery/Fetch)
static bool IsAjaxRequest(HttpRequest request)
{
    return string.Equals(
        request.Headers["X-Requested-With"].ToString(),
        "XMLHttpRequest",
        StringComparison.OrdinalIgnoreCase);
}