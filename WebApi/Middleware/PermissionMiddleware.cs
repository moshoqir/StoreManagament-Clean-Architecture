using System.Security.Claims;
using Application.Interfaces.Services;
using Domain.Entities;
using WebApi.Contracts.Common;
using WebApi.Security;

namespace WebApi.Middleware
{
    // Middleware that intercepts requests to enforce endpoint permissions
    public sealed class PermissionMiddleware
    {
        private readonly RequestDelegate _next;

        public PermissionMiddleware(RequestDelegate next)
        {
            _next = next; // The next middleware in the pipeline
        }

        public async Task InvokeAsync(HttpContext context, IPermissionService permissionService)
        {
            PermissionAttribute? permission = context.GetEndpoint()?
                .Metadata
                .GetMetadata<PermissionAttribute>();

             if (permission is null)
            {
                await _next(context);
                return;
            }

            string? userIdValue = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdValue, out int userId) || userId <= 0)
            {
                await WriteErrorAsync(
                    context,
                    StatusCodes.Status401Unauthorized,
                    "User identity is required.");
                return;
            }

            bool allowed = await permissionService.HasPermissionAsync(userId, permission.Code);

            if (!allowed)
            {
                await WriteErrorAsync(
                    context,
                    StatusCodes.Status403Forbidden,
                    $"Permission '{permission.Code}' is required.");
                return;
            }

            await _next(context);
        }

        private static async Task WriteErrorAsync(HttpContext context, int statusCode, string message)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsJsonAsync(
                ApiResponse.Failure(message, statusCode));
        }
    }
}
