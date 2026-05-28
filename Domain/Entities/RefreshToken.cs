using BlogFlow.API.Exceptions;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogFlow.API.Domain.Entities;

public class RefreshToken
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;
    public string Token { get; private set; } = null!;
    public DateTime ExpiresAt { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? RevokedAt { get; private set; }
    public string? RevokeReason { get; private set; }
    public string? ReplacedByToken { get; private set; }

    private RefreshToken() { } // EF Core
    public RefreshToken(Guid userId, string token, DateTime expiresAt)
    {
        UserId = userId;
        Token = token;
        ExpiresAt = expiresAt;
    }

    [NotMapped]
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

    [NotMapped]
    public bool IsRevoked => RevokedAt != null;

    [NotMapped]
    public bool IsActive => !IsExpired && !IsRevoked;

    public void Revoke(string reason, string? replacedByToken = null)
    {
        if (IsRevoked)
            throw new ConflictException("Token is already revoked.", "TOKEN_ALREADY_REVOKED");

        RevokedAt = DateTime.UtcNow;
        RevokeReason = reason;
        ReplacedByToken = replacedByToken;
    }
}