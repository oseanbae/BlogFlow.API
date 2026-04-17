using System.ComponentModel.DataAnnotations.Schema;

namespace BlogFlow.API.Models
{
    public class RefreshToken
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public required string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime? RevokedAt { get; set; }
        public string? RevokeReason { get; set; }
        public string? ReplacedByToken { get; set; }

        [NotMapped]
        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

        [NotMapped]
        public bool IsRevoked => RevokedAt != null;

        [NotMapped]
        public bool IsActive => !IsExpired && !IsRevoked;
    }
}