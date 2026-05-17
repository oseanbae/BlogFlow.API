using BlogFlow.API.Data;
using BlogFlow.API.Data.Seeding;
using BlogFlow.API.Infrastructure;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
        .UseSnakeCaseNamingConvention());

// Controllers & JSON
builder.Services.AddControllers(options =>
    {
        options.SuppressAsyncSuffixInActionNames = false;
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

// OpenAPI / Scalar
builder.Services.AddOpenApi();

// Repositories & Services 
builder.Services.AddApplicationServices();

// HTTP CONTEXT
builder.Services.AddHttpContextAccessor();

// Authentication & Authorization
builder.Services.AddJwtAuthentication(builder.Configuration);

// Rate Limiting
builder.Services.AddAuthRateLimiting(builder.Configuration);

//Fluent Validation for DTOs
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Global Exception Handler
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
    .WriteTo.Console()
    .WriteTo.File(new JsonFormatter(),
        "Logs/log-.json", 
        rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .CreateLogger();


builder.Host.UseSerilog();

var app = builder.Build();

// Always run migrations
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

// Development
if (app.Environment.IsDevelopment())
{
    //API Testing
    app.MapOpenApi();
    app.MapScalarApiReference();

    //Seeding
    using var scope = app.Services.CreateScope();
    var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
    await seeder.SeedAsync();

}

// Middleware
app.UseSerilogRequestLogging(options =>
{
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("TraceId", httpContext.TraceIdentifier);
        diagnosticContext.Set("Path", httpContext.Request.Path);
        diagnosticContext.Set("Method", httpContext.Request.Method);
        diagnosticContext.Set("IP", httpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown");
    };
});

app.UseExceptionHandler();

app.UseHttpsRedirection(); 
app.UseRouting();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();