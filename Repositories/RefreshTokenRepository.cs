using BlogFlow.API.Data;
using BlogFlow.API.Models;
using BlogFlow.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
namespace BlogFlow.API.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AppDbContext _context;

        public RefreshTokenRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<RefreshToken> CreateAsync(RefreshToken refreshToken)
        {
            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            return refreshToken;
        }
        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            return await _context.RefreshTokens
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Token == token);
        }
        public async Task RevokeAsync(RefreshToken token, string reason, string? replacedByToken)
        {
            token.RevokedAt = DateTime.UtcNow;
            token.RevokeReason = reason;
            token.ReplacedByToken = replacedByToken;

            await _context.SaveChangesAsync();
        }
        public async Task RemoveExpiredAsync(Guid userId)
        {
            var expired = await _context.RefreshTokens
                .Where(r => r.UserId == userId && r.ExpiresAt < DateTime.UtcNow)
                .ToListAsync();

            _context.RefreshTokens.RemoveRange(expired);
            await _context.SaveChangesAsync();
        }
        public async Task RevokeAllUserTokensAsync(Guid userId, string reason)
        {
            var tokens = await _context.RefreshTokens
                .Where(r => r.UserId == userId
                    && r.RevokedAt == null
                    && r.ExpiresAt > DateTime.UtcNow)
                .ToListAsync();

            var now = DateTime.UtcNow;

            foreach (var token in tokens)
            {
                token.RevokedAt = now;
                token.RevokeReason = reason;
            }

            await _context.SaveChangesAsync();
        }
        
    }
}
