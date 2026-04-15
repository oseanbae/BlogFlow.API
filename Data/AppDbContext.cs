using Microsoft.EntityFrameworkCore;
using BlogFlow.API.Models;

namespace BlogFlow.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : 
            base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<PostTag> PostTags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email.ToLower().Trim())
                .IsUnique();

            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasIndex(r => r.Token).IsUnique();

                entity
                    .HasOne(r => r.User)
                    .WithMany(u => u.RefreshTokens)
                    .HasForeignKey(r => r.UserId);
                entity
                    .Property(x => x.Token)
                    .HasColumnType("nvarchar(max)");

            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasIndex(c => c.Name).IsUnique();

            });

            modelBuilder.Entity<PostTag>(entity =>
            {
                entity.HasKey(pt => new { pt.PostId, pt.TagId });

                entity.HasOne(pt => pt.Post)
                    .WithMany(p => p.PostTags)
                    .HasForeignKey(pt => pt.PostId);

                entity.HasOne(pt => pt.Tag)
                    .WithMany(t => t.PostTags)
                    .HasForeignKey(pt => pt.TagId);
            });
        }
    }
}

/*
 * Got it. You want the **complete mapping only** so you can implement it directly.

Here’s your **full OnModelCreating blueprint (no questions, no fluff)**.

---

# 🧠 EF Core Mapping Plan (Your Project)

## 1. User mapping

* Email must be unique
* Username must be unique
* One User → many Posts
* One User → many RefreshTokens

---

## 2. Post mapping

* One Post → one Author (User)
* One Post → one Category
* One Post → many PostTags

---

## 3. Category mapping

* Name must be unique
* One Category → many Posts

---

## 4. Tag mapping

* Name must be unique
* One Tag → many PostTags

---

## 5. PostTag mapping (JOIN TABLE — CRITICAL)

* Composite key: PostId + TagId
* One PostTag → one Post
* One PostTag → one Tag
* Prevent duplicate pairs via composite key

---

## 6. RefreshToken mapping

* Token must be unique
* One RefreshToken → one User
* One User → many RefreshTokens

---

# ⚙️ Delete behavior rules (apply consistently)

Use this logic:

* User → Posts: CASCADE
* User → RefreshTokens: CASCADE
* Post → PostTags: CASCADE
* Tag → PostTags: CASCADE
* Category → Posts: RESTRICT (or CASCADE if you want aggressive cleanup)

---

# 🧱 Execution order (important for implementation)

Implement in this order inside OnModelCreating:

1. Keys (only PostTag composite key)
2. Indexes (User, Tag, Category, RefreshToken)
3. Relationships (User → Post → Category → Tag)
4. PostTag relationships (last)
5. Delete behaviors (final pass)

---

# 🔥 Minimal rule summary

* Everything uses single primary key EXCEPT PostTag
* All names (Email, Username, Tag.Name, Category.Name) must be unique
* PostTag is the only composite key entity
* Relationships always define FK owner explicitly
* Delete behavior must be intentional, not default

---

If you implement exactly this structure, your EF Core model is **production-level clean and consistent**.

If you want next step after this, I can help you split this into **clean separate IEntityTypeConfiguration files (professional architecture style)**.

*/