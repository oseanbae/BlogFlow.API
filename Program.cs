using BlogFlow.API.Data;
using BlogFlow.API.Extensions;
using BlogFlow.API.Services;
using BlogFlow.API.Services.Interfaces;
using BlogFlow.API.Settings;
using Scalar.AspNetCore;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Controllers & JSON
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

// OpenAPI / Scalar
builder.Services.AddOpenApi();

// Dependency Injection
builder.Services.AddScoped<IAuthServices, AuthServices>();

// Authentication & Authorization
builder.Services.AddJwtAuthentication(builder.Configuration);

// Rate Limiting
builder.Services.AddAuthRateLimiting(builder.Configuration);

var app = builder.Build();

// Development
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

// Middleware
app.UseHttpsRedirection();
app.UseRouting();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();