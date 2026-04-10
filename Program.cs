using BlogFlow.API.Data;
using BlogFlow.API.Services;
using BlogFlow.API.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//
// -------------------- Database --------------------
//
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

//
// -------------------- Controllers & JSON --------------------
//
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

//
// -------------------- OpenAPI / Scalar --------------------
//
builder.Services.AddOpenApi();
//
// -------------------- Dependency Injection --------------------
//
builder.Services.AddScoped<IAuthServices, AuthServices>();

//
// -------------------- JWT Authentication --------------------
//
// Load JWT configuration section from appsettings.json
var jwt = builder.Configuration.GetSection("JwtSettings");

// Fail fast if secret is missing (critical security dependency)
var key = jwt["Secret"]
    ?? throw new Exception("JWT Secret is missing");

// Register authentication services in DI container
builder.Services
    .AddAuthentication(options =>
    {
        // Default scheme used when [Authorize] is triggered
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

        // Default behavior when auth fails (401 challenge response)
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })

    // Configure JWT Bearer authentication handler
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // Ensures token was issued by trusted issuer
            ValidateIssuer = true,

            // Ensures token is intended for this API
            ValidateAudience = true,

            // Ensures token is not expired
            ValidateLifetime = true,

            // Ensures signature is valid (token not tampered with)
            ValidateIssuerSigningKey = true,

            // Expected issuer value (must match token generation)
            ValidIssuer = jwt["Issuer"],

            // Expected audience value (must match token generation)
            ValidAudience = jwt["Audience"],

            // Secret key used to validate HMAC signature
            IssuerSigningKey =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
        };
    });

// Enable authorization system ([Authorize] attributes work)
builder.Services.AddAuthorization();

var app = builder.Build();

//
// -------------------- Middleware --------------------
//
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();