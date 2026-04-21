using BlogFlow.API.Models;

namespace BlogFlow.API.Repositories.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByHashedTokenAsync(string hashedToken);

        Task<RefreshToken> CreateAsync(RefreshToken refreshToken);

        Task RevokeAsync(
            RefreshToken token,
            string reason,
            string? replacedByToken = null);

        Task RevokeAllUserTokensAsync(Guid userId, string reason);

        Task RemoveExpiredAsync(Guid userId);
    }

}
