using Microsoft.EntityFrameworkCore;
using BlogFlow.API.Models;
using System.IO.Compression;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BlogFlow.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<PostTag> PostTags { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            // USER
            builder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);

                entity.Property(u => u.Id)
                    .ValueGeneratedNever();

                entity.Property(u => u.Username)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasIndex(u => u.Username).IsUnique();

                entity.ToTable(t =>
                    t.HasCheckConstraint("CK_Username_MinLength", "length(\"Username\") >= 3")
                );

                entity.Property(u => u.Email)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasIndex(u => u.Email).IsUnique();

                entity.Property(u => u.PasswordHash)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(u => u.Role)
                    .IsRequired();

                entity.Property(u => u.CreatedAt)
                    .IsRequired();

                entity.HasQueryFilter(u => u.DeletedAt == null);
                entity.HasIndex(u => u.DeletedAt);
            });

            // REFRESH TOKEN
            builder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(r => r.Id);

                entity.Property(r => r.Id)
                    .ValueGeneratedNever();

                entity.Property(r => r.Token)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.HasIndex(r => r.Token).IsUnique();
                entity.HasIndex(r => r.UserId);

                entity.Property(r => r.ExpiresAt)
                    .IsRequired()
                    .HasColumnType("timestamp with time zone");

                entity.Property(r => r.CreatedAt)
                    .IsRequired();

                entity.Property(r => r.RevokeReason)
                    .HasMaxLength(200);

                entity.ToTable(t =>
                {
                    t.HasCheckConstraint(
                        "CK_Token_Expiration_Future",
                        "\"ExpiresAt\" > \"CreatedAt\""
                    );

                    t.HasCheckConstraint(
                        "CK_RevokedAt_Valid",
                        "\"RevokedAt\" IS NULL OR \"RevokedAt\" >= \"CreatedAt\""
                    );
                });

                entity.HasIndex(r => new { r.UserId, r.RevokedAt, r.ExpiresAt });

                entity.HasOne(r => r.User)
                    .WithMany(u => u.RefreshTokens)
                    .HasForeignKey(r => r.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // POST
            builder.Entity<Post>(entity =>
            {
                entity.HasKey(p => p.Id);

                entity.Property(p => p.Id)
                    .ValueGeneratedNever();

                entity.Property(p => p.Title)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(p => p.Body)
                    .IsRequired()
                    .HasMaxLength(10000);

                entity.Property(p => p.CreatedAt)
                    .IsRequired();

                entity.Property(p => p.UpdatedAt)
                    .IsRequired(false);

                entity.HasQueryFilter(p => p.DeletedAt == null);

                entity.HasIndex(p => p.DeletedAt);
                entity.HasIndex(p => new { p.AuthorId, p.DeletedAt });
                entity.HasIndex(p => p.CategoryId);

                entity.HasOne(p => p.Author)
                    .WithMany(u => u.Posts)
                    .HasForeignKey(p => p.AuthorId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.Category)
                    .WithMany(c => c.Posts)
                    .HasForeignKey(p => p.CategoryId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // CATEGORY
            builder.Entity<Category>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.Property(c => c.Id)
                    .ValueGeneratedNever();

                entity.Property(c => c.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasIndex(c => c.Name).IsUnique();

                entity.HasMany(c => c.Posts)
                    .WithOne(p => p.Category)
                    .HasForeignKey(p => p.CategoryId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // TAG
            builder.Entity<Tag>(entity =>
            {
                entity.HasKey(t => t.Id);

                entity.Property(t => t.Id)
                    .ValueGeneratedNever();

                entity.Property(t => t.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasIndex(t => t.Name).IsUnique();

                entity.HasMany(t => t.PostTags)
                    .WithOne(pt => pt.Tag)
                    .HasForeignKey(pt => pt.TagId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // POST TAG
            builder.Entity<PostTag>(entity =>
            {
                entity.HasKey(pt => new { pt.PostId, pt.TagId });

                entity.HasIndex(pt => new { pt.TagId, pt.PostId });

                entity.HasOne(pt => pt.Post)
                    .WithMany(p => p.PostTags)
                    .HasForeignKey(pt => pt.PostId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(pt => pt.Tag)
                    .WithMany(t => t.PostTags)
                    .HasForeignKey(pt => pt.TagId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    } 
}