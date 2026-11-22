using System.Security.Claims;

namespace WebPortal.Common
{
    public static class ApiRequestHelper
    {
        //This common Class to add userId to the header request
        //public static void AddUserIdHeader(HttpClient client, ClaimsPrincipal user)
        //{
        //    // Retrieve the user ID encrypted inside the authentication cookie
        //    string? userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

        //    if (!string.IsNullOrWhiteSpace(userId))
        //    {
        //        // Remove existing header to prevent duplication in scoped HttpClients, then append
        //        client.DefaultRequestHeaders.Remove("X-User-Id");
        //        client.DefaultRequestHeaders.Add("X-User-Id", userId);
        //    }
        //}


        // #### We used to send the userId in the header, now, after we added the JWT, we must pass the bearer token in the header instead.

        public static void AddBearerToken(HttpClient client, ClaimsPrincipal user)
        {
            string? accessToken = user.FindFirstValue("AccessToken");

            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            }
        }
    }
}
