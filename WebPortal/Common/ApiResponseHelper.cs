using Microsoft.AspNetCore.Mvc;

namespace WebPortal.Common
{
    public class ApiResponseHelper
    {
        // Deserializing data will take time when there is a lot of data. 
        // Therefore, we build this helper to get the data directly as json, so we don't need to deserialize anymore, because the response already returns as JSON

        public static async Task<ContentResult> ToActionResultAsync(HttpResponseMessage response)
        {
            string json = await response.Content.ReadAsStringAsync();
            // Return it exactly as it was received, preserving the original HTTP status code
            return new ContentResult
            {
                Content = json,
                ContentType = "application/json; charset=utf-8",
                StatusCode = (int)response.StatusCode,
            };
        }
    }
}
