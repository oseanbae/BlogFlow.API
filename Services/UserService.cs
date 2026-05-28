using BlogFlow.API.DTOs.User;
using BlogFlow.API.Exceptions;
using BlogFlow.API.Domain.Entities;
using BlogFlow.API.QueryExtensions;
using BlogFlow.API.Repositories.Interfaces;
using BlogFlow.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlogFlow.API.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;
        private readonly ILogger<UserService> _logger;

        public UserService(
            IUserRepository repo,
            ILogger<UserService> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task ChangePasswordAsync(
            Guid id,
            UserChangePasswordDTO dto,
            CancellationToken cancellationToken)
        {
            if (dto.CurrentPassword == dto.NewPassword)
            {
                _logger.LogWarning(
                    "User {UserId} attempted to reuse current password",
                    id);

                throw new ConflictException(
                    "The new password must be different from your current password.",
                    "PASSWORD_ALREADY_IN_USE");
            }

            var user = await _repo.GetByIdAsync(id, includeDeleted: false, cancellationToken)
                ?? throw new NotFoundException("User", id);

            if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
            {
                _logger.LogWarning(
                    "Failed password change attempt for user {UserId} due to invalid current password",
                    id);

                throw new UnauthorizedException(
                    "The current password provided is incorrect.",
                    "INVALID_CURRENT_PASSWORD");
            }

            user.ChangePassword(BCrypt.Net.BCrypt.HashPassword(dto.NewPassword));

            await _repo.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Password changed successfully for user {UserId}",
                id);
        }

        public async Task DeleteOwnAccountAsync(
            Guid userId,
            CancellationToken cancellationToken)
        {
            var user = await _repo.GetByIdAsync(userId, includeDeleted: false, cancellationToken)
                ?? throw new NotFoundException("User", userId);

            user.SoftDelete();

            await _repo.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "User account soft deleted: {UserId}",
                userId);
        }

        public async Task<UserProfileDTO> GetMyProfileAsync(
            Guid userId,
            CancellationToken cancellationToken)
        {
            return await _repo.GetUsersQuery()
                .Where(u => u.Id == userId)
                .AsProfileDTO()
                .FirstOrDefaultAsync(cancellationToken)
                ?? throw new NotFoundException("User", userId);
        }

        public async Task<UserPublicProfileDTO> GetPublicProfileAsync(
            Guid userId,
            CancellationToken cancellationToken)
        {
            return await _repo.GetUsersQuery()
                .Where(u => u.Id == userId)
                .AsPublicProfileDTO()
                .FirstOrDefaultAsync(cancellationToken)
                ?? throw new NotFoundException("User", userId);
        }

        public async Task UpdateProfileAsync(
            UserUpdateDTO dto,
            Guid userId,
            CancellationToken cancellationToken)
        {
            var normalizedUsername = dto.Username.ToLowerInvariant();
            var normalizedEmail = dto.Email.ToLowerInvariant();

            var existingUsername = await _repo.GetByUsernameAsync(normalizedUsername, cancellationToken);
            if (existingUsername != null && existingUsername.Id != userId)
                throw new ConflictException(
                    $"Username '{dto.Username}' is already taken.",
                    "USERNAME_ALREADY_EXISTS");

            var existingEmail = await _repo.GetByEmailAsync(normalizedEmail, cancellationToken);
            if (existingEmail != null && existingEmail.Id != userId)
                throw new ConflictException(
                    $"Email '{dto.Email}' is already taken.",
                    "EMAIL_ALREADY_EXISTS");

            var user = await _repo.GetByIdAsync(userId, includeDeleted: false, cancellationToken)
                ?? throw new NotFoundException("User", userId);

            user.UpdateIdentity(normalizedUsername, normalizedEmail);

            await _repo.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "User profile updated: {UserId}",
                userId);
        }
    }
}