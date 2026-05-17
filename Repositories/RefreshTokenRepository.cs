using BlogFlow.API.Data;
using BlogFlow.API.Models;
using BlogFlow.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlogFlow.API.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AppDbContext _context;

        public RefreshTokenRepository(AppDbContext context)
        {
            _context = context;
        }

        // CREATE
        public async Task<RefreshToken> CreateAsync(RefreshToken refreshToken, CancellationToken cancellationToken)
        {
            await _context.RefreshTokens.AddAsync(refreshToken, cancellationToken);
            return refreshToken;
        }

        // GET (HASHED TOKEN LOOKUP)
        public async Task<RefreshToken?> GetByHashedTokenAsync(string hashedToken, CancellationToken cancellationToken)
        {
            return await _context.RefreshTokens
                .Include(r => r.User)
                .FirstOrDefaultAsync(
                    r => r.Token == hashedToken,
                    cancellationToken);
        }

        // REVOKE SINGLE TOKEN
        public Task RevokeAsync(RefreshToken token, string reason, string? replacedByToken = null, CancellationToken cancellationToken = default)
        {
            token.RevokedAt = DateTime.UtcNow;
            token.RevokeReason = reason;
            token.ReplacedByToken = replacedByToken;

            return Task.CompletedTask;
        }

        // CLEAN EXPIRED TOKENS
        public async Task RemoveExpiredAsync(Guid userId, CancellationToken cancellationToken)
        {
            var expiredTokens = await _context.RefreshTokens
                .Where(r => r.UserId == userId && r.ExpiresAt < DateTime.UtcNow)
                .ToListAsync(cancellationToken);

            _context.RefreshTokens.RemoveRange(expiredTokens);
        }

        // REVOKE ALL USER TOKENS (SECURITY EVENT)
        public async Task RevokeAllUserTokensAsync(Guid userId, string reason, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;

            var activeTokens = await _context.RefreshTokens
                .Where(r =>
                    r.UserId == userId &&
                    r.RevokedAt == null &&
                    r.ExpiresAt > now)
                .ToListAsync(cancellationToken);

            foreach (var token in activeTokens)
            {
                token.RevokedAt = now;
                token.RevokeReason = reason;
            }
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}