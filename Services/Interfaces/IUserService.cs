using BlogFlow.API.DTOs.User;
using BlogFlow.API.Models;

namespace BlogFlow.API.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserReadDTO> GetUserByIdAsync(Guid id);
        Task UpdateProfileAsync(UserUpdateDTO dto, UserContext user);
        Task DeleteOwnAccountAsync(UserContext user); // Soft Delete
        Task ChangePasswordAsync(Guid id, UserChangePasswordDTO dto);
    }
}