using Microsoft.EntityFrameworkCore;
using BlogFlow.API.Models;

namespace BlogFlow.API.Data.Seeding.Seeds
{
    public class CategorySeed : ISeeder
    {
        public int Order => 2;

        public async Task SeedAsync(AppDbContext db)
        {
            if (await db.Categories.AnyAsync()) return;

            var techId = Guid.Parse("b1a1c1d1-0000-0000-0000-000000000001");
            var lifeId = Guid.Parse("b1a1c1d1-0000-0000-0000-000000000002");
            var sportId = Guid.Parse("b1a1c1d1-0000-0000-0000-000000000003");
            var travelId = Guid.Parse("b1a1c1d1-0000-0000-0000-000000000004");
            var foodId = Guid.Parse("b1a1c1d1-0000-0000-0000-000000000005");

            var financeId = Guid.Parse("b1a1c1d1-0000-0000-0000-000000000006");
            var gamingId = Guid.Parse("b1a1c1d1-0000-0000-0000-000000000007");
            var educationId = Guid.Parse("b1a1c1d1-0000-0000-0000-000000000008");
            var healthId = Guid.Parse("b1a1c1d1-0000-0000-0000-000000000009");

            await db.Categories.AddRangeAsync(
                new Category(techId, "Technology"),
                new Category(lifeId, "Lifestyle"),
                new Category(sportId, "Sports"),
                new Category(travelId, "Travel"),
                new Category(foodId, "Food"),
                new Category(financeId, "Finance"),
                new Category(gamingId, "Gaming"),
                new Category(educationId, "Education"),
                new Category(healthId, "Health")
            );

            await db.SaveChangesAsync();
        }
    }
}