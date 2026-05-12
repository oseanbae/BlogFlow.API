using BlogFlow.API.DTOs.User;
using BlogFlow.API.Models;
using BlogFlow.API.Repositories.Interfaces;
using BlogFlow.API.Services.Interfaces;


namespace BlogFlow.API.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;
        public UserService(IUserRepository repo) => _repo = repo;
        public async Task ChangePasswordAsync(Guid id, UserChangePasswordDTO dto)
        {
            var user = await _repo.GetTrackedByIdAsync(id)
                ?? throw new KeyNotFoundException("User does not exist");

            bool isCurrentPasswordValid = BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash);

            if (!isCurrentPasswordValid)
                throw new UnauthorizedAccessException("The current password provided is incorrect.");

            if (dto.CurrentPassword == dto.NewPassword)
            {
                throw new Exception("New password cannot be the same as the current password.");
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);

            user.ChangePassword(hashedPassword);

            await _repo.SaveChangesAsync();
        }

        public async Task DeleteOwnAccountAsync(Guid userId)
        {
            var user = await _repo.GetTrackedByIdAsync(userId) 
                ?? throw new KeyNotFoundException("User does not exist");

            user.SoftDelete();
            await _repo.SaveChangesAsync();
        }

        public async Task<UserReadDTO> GetUserByIdAsync(Guid id)
        {
            var existingUser = await _repo.GetTrackedByIdAsync(id)
                ?? throw new KeyNotFoundException("User does not exist");

            return new UserReadDTO
            {
                Id = existingUser.Id,
                Username = existingUser.Username,
                Email = existingUser.Email,
                Role = existingUser.Role,
                CreatedAt = existingUser.CreatedAt,
            };
        }

        public async Task UpdateProfileAsync(UserUpdateDTO dto, Guid userId)
        {
            var existingUser = await _repo.GetTrackedByIdAsync(userId)
                ?? throw new KeyNotFoundException("User does not exist");

            existingUser.UpdateIdentity(dto.Username, dto.Email);

            await _repo.SaveChangesAsync();
        }
    }
}
