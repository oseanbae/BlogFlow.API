using BlogFlow.API.Domain.Entities;

namespace BlogFlow.API.Repositories.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByHashedTokenAsync(string hashedToken, CancellationToken cancellationToken);

        Task<RefreshToken> CreateAsync(RefreshToken refreshToken, CancellationToken cancellationToken);

        Task RevokeAllUserTokensAsync(Guid userId, string reason, CancellationToken cancellationToken);

        Task RemoveExpiredAsync(Guid userId, CancellationToken cancellationToken);

        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
