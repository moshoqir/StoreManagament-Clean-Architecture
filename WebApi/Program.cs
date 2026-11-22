using System.Text;
using Application.DependencyInjection;
using Infrastructure.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using WebApi.Contracts.Common;
using WebApi.Middleware;
using WebApi.Security;

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
builder.Services.AddEndpointsApiExplorer();

// to make swagger take bearer token
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT token"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// adding our DI we created in each layer (MUST include it above)
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Add the registered keys in appsettings
builder.Services.AddScoped<JwtTokenService>();

// Add RefreshToken
builder.Services.AddScoped<RefreshTokenService>();

string jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Jwt:Key is missing from appsettings.json.");
string jwtIssuer = builder.Configuration["Jwt:Issuer"]
    ?? throw new InvalidOperationException("Jwt:Issuer is missing from appsettings.json.");
string jwtAudience = builder.Configuration["Jwt:Audience"]
    ?? throw new InvalidOperationException("Jwt:Audience is missing from appsettings.json.");
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

 




var app = builder.Build();

// call ExceptionHandlingMiddleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();





// add permission checking middleware
app.UseMiddleware<PermissionMiddleware>();

app.MapControllers();




app.Run();
