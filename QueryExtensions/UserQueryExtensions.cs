using BlogFlow.API.Domain.Entities;
using BlogFlow.API.DTOs.Admin;
using BlogFlow.API.DTOs.User;
using Microsoft.EntityFrameworkCore;

namespace BlogFlow.API.QueryExtensions
{
    public static class UserQueryExtensions
    {
        // Admin projection — includes sensitive fields
        public static IQueryable<AdminUserReadDTO> AsAdminDTO(this IQueryable<User> query)
        {
            return query
                .AsNoTracking()
                .Select(u => new AdminUserReadDTO
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    Role = u.Role,
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt,
                    DeletedAt = u.DeletedAt,
                });
        }

        // Public projection — excludes sensitive fields
        public static IQueryable<UserProfileDTO> AsProfileDTO(this IQueryable<User> query)
        {
            return query
                .AsNoTracking()
                .Select(u => new UserProfileDTO
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    Role = u.Role,
                    CreatedAt = u.CreatedAt
                });
        }

        public static IQueryable<UserPublicProfileDTO> AsPublicProfileDTO(this IQueryable<User> query)
        {
            return query
                .AsNoTracking()
                .Select(u => new UserPublicProfileDTO
                {
                    Id = u.Id,
                    Username = u.Username,
                    Role = u.Role,
                    CreatedAt = u.CreatedAt
                });
        }
    }
}
