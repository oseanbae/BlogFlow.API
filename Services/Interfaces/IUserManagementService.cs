using BlogFlow.API.DTOs.Admin;
using BlogFlow.API.DTOs.Common;
using BlogFlow.API.DTOs.User;
using BlogFlow.API.Models; 

namespace BlogFlow.API.Services.Interfaces
{
    public interface IUserManagementService
    {
        Task<PaginatedResultDTO<AdminUserReadDTO>> GetUsersAsync(UserQueryParams filter, CancellationToken cancellationToken);
        Task ChangeRoleAsync(Guid userId, UserUpdateRoleDTO dto, CancellationToken cancellationToken);
        Task SoftDeleteUserAsync(Guid userId, CancellationToken cancellationToken);
        Task RestoreUserAsync(Guid userId, CancellationToken cancellationToken);
        Task<AdminStatsDTO> GetStatisticsAsync(CancellationToken cancellationToken);
    }
}
