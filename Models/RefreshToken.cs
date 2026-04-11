using BlogFlow.API.Models;
using System.ComponentModel.DataAnnotations.Schema;

public class RefreshToken
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public required string Token { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? RevokedAt { get; set; }
    public string? RevokeReason { get; set; }
    public string? ReplacedByTokenId { get; set; } // Consider changing this to Guid? ReplacedByTokenId

    //Computed Logic (Ignored by DB)
    [NotMapped]
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

    [NotMapped]
    public bool IsRevoked => RevokedAt != null;

    [NotMapped]
    public bool IsActive => !IsRevoked && !IsExpired;
}
