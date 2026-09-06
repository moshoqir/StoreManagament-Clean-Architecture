using Application.Common.Exceptions;
using WebApi.Contracts.Common;

namespace WebApi.Middleware
{

    // this class is used to handle exceptions globally
    public sealed class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        // ctor to inject:
        // 1. RequestDelegate: It's a delegate that can process an HTTP request. It'll trace the request
        // 2. ILogger: It's a logging interface to log exceptions

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }


        // this method is called for each HTTP request. It tries to invoke the next middleware in the pipeline. If an exception occurs, it catches it and handles it.
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // if no exception, go to the next middleware in the pipeline
                await _next(context);
            }
            catch(Exception exception)
            {
                // if the exception is not handled, log it and return a generic error response
                // in other words, 1. stop processsing the request
                if (context.Response.HasStarted)
                {
                    throw;
                }
                // 2. show the exception using this method
                await HandleExceptionAsync(context, exception);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // we will use Tuple to return status code in one go.
            (int statusCode, string message) = exception switch
            {
                NotFoundException =>
                (
                    StatusCodes.Status404NotFound,
                    exception.Message
                ),

                ConflictException =>
                (
                StatusCodes.Status409Conflict,
                exception.Message
                ),

                ArgumentException =>
                (
                 StatusCodes.Status400BadRequest,
                 exception.Message
                ),

                _ =>
                (
                    StatusCodes.Status500InternalServerError,
                    "An unexpected error occurred."
                )
            };

            // get the data like request method, path, and request id from the context to log the exception
            var requestMethod = context.Request.Method;
            var requestPath = context.Request.Path;
            string requestId = context.TraceIdentifier;

            // get the excetion type and show it in log using logger
            if (statusCode >= StatusCodes.Status500InternalServerError)
            {
                _logger.LogError(
                    exception,
                    $"An unhandled error while proceesing {requestMethod} {requestPath}." +
                    $"Status Code: {statusCode}. RequestId: {requestId}",
                    requestMethod,
                    requestPath,
                    statusCode,
                    requestId

                );
            }
            else
            {
                _logger.LogWarning(

                    exception,
                    $"Handled request error while processing {requestMethod} {requestPath}." +
                    $"StatusCode: {statusCode}. RequestId: {requestId}",
                    requestMethod,
                    requestPath,
                    statusCode,
                    requestId
                    );
            }

            // set the response status code and content type
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";
            ApiResponse<object?> response =
                ApiResponse.Failure(
                    message,
                    statusCode);

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}
