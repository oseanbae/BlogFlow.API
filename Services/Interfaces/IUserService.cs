using BlogFlow.API.DTOs.User;

namespace BlogFlow.API.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserReadDTO> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken);
        Task UpdateProfileAsync(UserUpdateDTO dto, Guid userId, CancellationToken cancellationToken);
        Task DeleteOwnAccountAsync(Guid userId, CancellationToken cancellationToken); // Soft Delete
        Task ChangePasswordAsync(Guid userId, UserChangePasswordDTO dto, CancellationToken cancellationToken);
    }
}