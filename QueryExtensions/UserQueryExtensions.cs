using BlogFlow.API.DTOs.Admin;
using BlogFlow.API.DTOs.User;
using BlogFlow.API.Models;
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
        public static IQueryable<UserReadDTO> AsDTO(this IQueryable<User> query)
        {
            return query
                .AsNoTracking()
                .Select(u => new UserReadDTO
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    Role = u.Role,
                    CreatedAt = u.CreatedAt,
                });
        }
    }
}
