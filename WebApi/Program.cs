using Microsoft.AspNetCore.Mvc;
using WebApi.Contracts.Common;
using Application.DependencyInjection;
using Infrastructure.DependencyInjection;
using WebApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

// to deal with how json data is dealt with. Such as naming policy and null valuse
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
})
    // to deal with how model validation errors are handled and how they are returned to the client
    // this will work like this : see the error --> filters it--> returns the error based on the Middleware we created in ExceptionHandlingMiddleware
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
            .Where(item => item.Value?.Errors.Count > 0)
            .ToDictionary(
                item => item.Key,
                item => item.Value?.Errors
                .Select(error =>

                    string.IsNullOrWhiteSpace(error.ErrorMessage)
                     ? "Invalid value"
                     : error.ErrorMessage

                ).ToArray());

            // return failer ApiResonse
            var response = ApiResponse.Failure(
                "Validation Failed",
                StatusCodes.Status400BadRequest,
                errors
                );

            return new BadRequestObjectResult(response);
        };
    });

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// adding our DI we created in each layer (MUST include it above)
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// call ExceptionHandlingMiddleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
// redirection
app.UseHttpsRedirection();
app.MapControllers();

app.Run();
