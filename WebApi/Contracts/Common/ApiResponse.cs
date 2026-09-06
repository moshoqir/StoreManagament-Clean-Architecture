namespace WebApi.Contracts.Common
{

    // This is standard way to return response from APIs.
    public sealed class ApiResponse<T>
    {
        public int Code { get; set; }

        public bool Status { get; set; }

        public string Message { get; set; } = string.Empty;

        public T? Data { get; set; }

        public object? Errors { get; set; }
    }

    public class ApiResponse
    {
        public static ApiResponse<T> Success<T>(T? data, string message, int code = StatusCodes.Status200OK)
        {
            return new ApiResponse<T>
            {
                Code = code,
                Status = true,
                Message = message,
                Data = data
            };
        }

        // we use this when we want to return only message without any data.
        public static ApiResponse<object?> Success(string message, int code = StatusCodes.Status200OK)
        {
            return new ApiResponse<object?>
            {
                Code = code,
                Status = true,
                Message = message,
                Data = null
            };
        }

        public static ApiResponse<object?> Failure(string message, int code, object? errors = null)
        {
            return new ApiResponse<object?>
            {
                Code = code,
                Status = false,
                Message = message,
                Data = null,
                Errors = errors
            };
        }
    }
}
