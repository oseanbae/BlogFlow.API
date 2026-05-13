using BlogFlow.API.DTOs.Admin;
using BlogFlow.API.DTOs.Common;
using BlogFlow.API.DTOs.User;
using BlogFlow.API.Models; 

namespace BlogFlow.API.Services.Interfaces
{
    public interface IUserManagementService
    {
        Task<PaginatedResultDTO<AdminUserReadDTO>> GetUsersAsync(UserQueryParams filter);
        Task ChangeRoleAsync(Guid userId, UserUpdateRoleDTO dto);
        Task SoftDeleteUserAsync(Guid userId);
        Task RestoreUserAsync(Guid userId);
        Task<AdminStatsDTO> GetStatisticsAsync();
    }
}
