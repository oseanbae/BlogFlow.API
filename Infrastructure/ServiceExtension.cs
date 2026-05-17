using BlogFlow.API.Repositories;
using BlogFlow.API.Repositories.Interfaces;
using BlogFlow.API.Services;
using BlogFlow.API.Services.Interfaces;
using BlogFlow.API.Data.Seeding;
using BlogFlow.API.Data.Seeding.Seeds;

namespace BlogFlow.API.Infrastructure
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services) 
        {
            // --- Repositories ---
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ITagRepository, TagRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();
            
            // --- Services ---
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ITagService, TagService>();
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<IUserManagementService, UserManagementService>();

            // --- Seeder Services ---
            services.AddScoped<ISeeder, UserSeed>();
            services.AddScoped<ISeeder, CategorySeed>();
            services.AddScoped<ISeeder, TagSeed>();
            services.AddScoped<ISeeder, PostSeed>();
            services.AddScoped<ISeeder, CommentSeed>();
            services.AddScoped<DatabaseSeeder>();

            return services;
        }
    }
}
