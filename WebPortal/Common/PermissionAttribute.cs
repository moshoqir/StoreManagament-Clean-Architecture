using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebPortal.Common
{
    // This is this same as PermissionAttribute in API layer.
    //  // Specifies that this attribute can be applied to classes or methods, only once per target, and is inherited by derived classes
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class PermissionAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string _permissionCode;

        public PermissionAttribute(string permissionCode)
        {
            _permissionCode = permissionCode;
        }

        // Executes before the controller action starts
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            // check the user auth
            if (!(user.Identity?.IsAuthenticated ?? false))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // check if the user has permission
            if (!user.HasClaim("Permission", _permissionCode))
            {
                context.Result = new ForbidResult();
                
            }
        }
    }
}
