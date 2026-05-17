using BlogFlow.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogFlow.API.Data.Seeding.Seeds
{
    public class UserSeed : ISeeder
    {
        public int Order => 1;

        public async Task SeedAsync(AppDbContext db)
        {
            if (await db.Users.AnyAsync())
                return;

            var adminId = Guid.Parse("a1b1c1d1-0000-0000-0000-000000000001");
            var author1Id = Guid.Parse("a1b1c1d1-0000-0000-0000-000000000002");
            var author2Id = Guid.Parse("a1b1c1d1-0000-0000-0000-000000000003");
            var reader1Id = Guid.Parse("a1b1c1d1-0000-0000-0000-000000000004");
            var reader2Id = Guid.Parse("a1b1c1d1-0000-0000-0000-000000000005");

            await db.Users.AddRangeAsync(
                new User(adminId, "admin", "admin@example.com", BCrypt.Net.BCrypt.HashPassword("test123"), UserRole.Admin),
                new User(author1Id, "author1", "author1@example.com", BCrypt.Net.BCrypt.HashPassword("test123"), UserRole.Author),
                new User(author2Id, "author2", "author2@example.com", BCrypt.Net.BCrypt.HashPassword("test123"), UserRole.Author),
                new User(reader1Id, "reader1", "reader1@example.com", BCrypt.Net.BCrypt.HashPassword("test123"), UserRole.Reader),
                new User(reader2Id, "reader2", "reader2@example.com", BCrypt.Net.BCrypt.HashPassword("test123"), UserRole.Reader)
            );

            await db.SaveChangesAsync();
        }
    }
}

//when using the fks, use _db.(dbname).FirstOrDefaultAsync
//add seed to serivce