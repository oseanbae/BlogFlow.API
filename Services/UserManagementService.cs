using BlogFlow.API.DTOs.Admin;
using BlogFlow.API.DTOs.Common;
using BlogFlow.API.DTOs.User;
using BlogFlow.API.Exceptions;
using BlogFlow.API.Models;
using BlogFlow.API.QueryExtensions;
using BlogFlow.API.Repositories.Interfaces;
using BlogFlow.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlogFlow.API.Services
{
    public class UserManagementService : IUserManagementService
    {
        private readonly IUserRepository _repo;
        private readonly ILogger<UserManagementService> _logger;

        public UserManagementService(
            IUserRepository repo,
            ILogger<UserManagementService> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task ChangeRoleAsync(
            Guid userId,
            UserUpdateRoleDTO dto)
        {
            var user = await _repo.GetTrackedByIdAsync(userId)
                ?? throw new NotFoundException("User", userId);

            var previousRole = user.Role;

            user.UpdateRole(dto.Role);

            await _repo.SaveChangesAsync();

            _logger.LogInformation(
                "User role changed: {UserId} from {OldRole} to {NewRole}",
                user.Id,
                previousRole,
                dto.Role);
        }

        public async Task<PaginatedResultDTO<AdminUserReadDTO>> GetUsersAsync(UserQueryParams p)
        {
            var query = _repo.GetUsersQuery();

            query = ApplyFilters(query, p);

            return await ExecutePagedQueryAsync(query, p.Page, p.PageSize);
        }

        public async Task RestoreUserAsync(Guid userId)
        {
            var user = await _repo.GetTrackedByIdAsync(userId)
                ?? throw new NotFoundException("User", userId);

            user.Restore();

            await _repo.SaveChangesAsync();

            _logger.LogInformation(
                "User restored: {UserId}",
                user.Id);
        }

        public async Task SoftDeleteUserAsync(Guid userId)
        {
            var user = await _repo.GetTrackedByIdAsync(userId)
                ?? throw new NotFoundException("User", userId);

            user.SoftDelete();

            await _repo.SaveChangesAsync();

            _logger.LogInformation(
                "User soft deleted: {UserId}",
                user.Id);
        }

        public async Task<AdminStatsDTO> GetStatisticsAsync()
        {
            DateTime weekBeforeUTC = DateTime.UtcNow.AddDays(-7);
            DateTime monthBeforeUTC = DateTime.UtcNow.AddMonths(-1);

           return await _repo
                .GetUsersQuery()
                // Project only required field
                .Select(u => new
                {
                    u.DeletedAt,
                    u.Role,
                    u.CreatedAt
                })
                // Group all rows into a single group so multiple aggregate functions
                .GroupBy(_ => 1)
                .Select(g => new AdminStatsDTO
                {
                    TotalUsers = g.Count(),
                    ActiveUsers = g.Count(x => x.DeletedAt == null),
                    DeletedUsers = g.Count(x => x.DeletedAt != null),
                    AdminCount = g.Count(x => x.Role == UserRole.Admin),
                    AuthorCount = g.Count(x => x.Role == UserRole.Author),
                    ReaderCount = g.Count(x => x.Role == UserRole.Reader),
                    NewUsersThisWeek = g.Count(x => x.CreatedAt >= weekBeforeUTC),
                    NewUsersThisMonth = g.Count(x => x.CreatedAt >= monthBeforeUTC)
                })
                // Since GroupBy(u => 1) produces exactly one group, we take the single result from the query
                // FirstAsync ensures execution of the query and returns that single aggregated result
                .FirstAsync();
        }

        private async Task<PaginatedResultDTO<AdminUserReadDTO>> ExecutePagedQueryAsync(
            IQueryable<User> query,
            int page,
            int pageSize)
        {
            var pagedResult = await query
                .OrderByDescending(p => p.CreatedAt)
                .AsDTO()
                .ToPaginatedResultAsync(page, pageSize);

            return new PaginatedResultDTO<AdminUserReadDTO>
            {
                TotalCount = pagedResult.TotalCount,
                Page = pagedResult.Page,
                PageSize = pagedResult.PageSize,
                Items = pagedResult.Items
            };
        }

        private static IQueryable<User> ApplyFilters(IQueryable<User> query, UserQueryParams p)
        {
            if (!string.IsNullOrWhiteSpace(p.Search))
            {
                query = query.Where(u =>
                    u.Username.Contains(p.Search) ||
                    u.Email.Contains(p.Search));
            }

            if (p.Role is UserRole role)
                query = query.Where(u => u.Role == role);

            if (!p.IsDeleted.HasValue)
            {
                query = query.Where(u => u.DeletedAt == null);
            }
            else
            {
                query = p.IsDeleted.Value
                    ? query.Where(u => u.DeletedAt != null)
                    : query.Where(u => u.DeletedAt == null);
            }

            if (p.CreatedAfter.HasValue)
                query = query.Where(u => u.CreatedAt >= p.CreatedAfter.Value);

            return query;
        }
    }
}