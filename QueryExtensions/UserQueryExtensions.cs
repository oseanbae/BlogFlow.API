using BlogFlow.API.DTOs.Admin;
using BlogFlow.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogFlow.API.QueryExtensions
{
    public static class UserQueryExtensions
    {
        public static IQueryable<AdminUserReadDTO> AsDTO(this IQueryable<User> query)
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
    }
}
