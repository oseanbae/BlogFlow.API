using BlogFlow.API.DTOs.User;
using BlogFlow.API.Models;

namespace BlogFlow.API.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserReadDTO> GetUserByIdAsync(Guid userId);
        Task UpdateProfileAsync(UserUpdateDTO dto, Guid userId);
        Task DeleteOwnAccountAsync(Guid userId); // Soft Delete
        Task ChangePasswordAsync(Guid userId, UserChangePasswordDTO dto);
    }
}