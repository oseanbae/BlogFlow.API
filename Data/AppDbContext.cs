using Microsoft.EntityFrameworkCore;
using BlogFlow.API.Models;

namespace BlogFlow.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : 
            base(options) { }
        public DbSet<User> Users { get; set; } = default!;

        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasIndex(r => r.Token).IsUnique();
                entity
                    .HasOne(r => r.User)
                    .WithMany(u => u.RefreshTokens)
                    .HasForeignKey(r => r.UserId);

            });
        }
    }
}
