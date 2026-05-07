using BlogFlow.API.Data;
using BlogFlow.API.Extensions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
        .UseSnakeCaseNamingConvention());

// Controllers & JSON
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

// OpenAPI / Scalar
builder.Services.AddOpenApi();

// Repositories & Services 
builder.Services.AddApplicationServices();

// Authentication & Authorization
builder.Services.AddJwtAuthentication(builder.Configuration);

// Rate Limiting
builder.Services.AddAuthRateLimiting(builder.Configuration);

//Fluent Validation for DTOs
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

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