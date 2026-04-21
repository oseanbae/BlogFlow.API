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
        public async Task<RefreshToken> CreateAsync(RefreshToken refreshToken)
        {
            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();
            return refreshToken;
        }

        // GET (HASHED TOKEN LOOKUP)
        public async Task<RefreshToken?> GetByHashedTokenAsync(string hashedToken)
        {
            return await _context.RefreshTokens
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Token == hashedToken);
        }

        // REVOKE SINGLE TOKEN
        public async Task RevokeAsync(RefreshToken token, string reason, string? replacedByToken)
        {
            token.RevokedAt = DateTime.UtcNow;
            token.RevokeReason = reason;
            token.ReplacedByToken = replacedByToken;

            await _context.SaveChangesAsync();
        }

        // CLEAN EXPIRED TOKENS
        public async Task RemoveExpiredAsync(Guid userId)
        {
            var expiredTokens = await _context.RefreshTokens
                .Where(r => r.UserId == userId && r.ExpiresAt < DateTime.UtcNow)
                .ToListAsync();

            _context.RefreshTokens.RemoveRange(expiredTokens);
            await _context.SaveChangesAsync();
        }

        // REVOKE ALL USER TOKENS (SECURITY EVENT)
        public async Task RevokeAllUserTokensAsync(Guid userId, string reason)
        {
            var now = DateTime.UtcNow;

            var activeTokens = await _context.RefreshTokens
                .Where(r =>
                    r.UserId == userId &&
                    r.RevokedAt == null &&
                    r.ExpiresAt > now)
                .ToListAsync();

            foreach (var token in activeTokens)
            {
                token.RevokedAt = now;
                token.RevokeReason = reason;
            }

            await _context.SaveChangesAsync();
        }
    }
}