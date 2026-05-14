using BlogFlow.API.DTOs.User;
using BlogFlow.API.Exceptions;
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
                ?? throw new NotFoundException("User", id); ;

            bool isCurrentPasswordValid = BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash);

            if (!isCurrentPasswordValid)
                throw new UnauthorizedException("The current password provided is incorrect.", "INVALID_CURRENT_PASSWORD");

            if (dto.CurrentPassword == dto.NewPassword)
            {
                throw new ConflictException(
                    "The new password must be different from your current password.",
                    "PASSWORD_ALREADY_IN_USE"
                );
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);

            user.ChangePassword(hashedPassword);

            await _repo.SaveChangesAsync();
        }

        public async Task DeleteOwnAccountAsync(Guid userId)
        {
            var user = await _repo.GetTrackedByIdAsync(userId) 
                ?? throw new NotFoundException("User", userId);

            user.SoftDelete();
            await _repo.SaveChangesAsync();
        }

        public async Task<UserReadDTO> GetUserByIdAsync(Guid id)
        {
            var existingUser = await _repo.GetTrackedByIdAsync(id)
                ?? throw new NotFoundException("User", id);

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
                ?? throw new NotFoundException("User", userId);

            existingUser.UpdateIdentity(dto.Username, dto.Email);

            await _repo.SaveChangesAsync();
        }
    }
}
