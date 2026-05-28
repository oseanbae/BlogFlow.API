using BlogFlow.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlogFlow.API.Data.Seeding.Seeds
{
    public class PostSeed : ISeeder
    {
        public int Order => 4;

        public async Task SeedAsync(AppDbContext db)
        {
            if (await db.Posts.AnyAsync()) return;

            // Batch fetch
            var users = await db.Users.ToDictionaryAsync(u => u.Username, u => u.Id);
            var categories = await db.Categories.ToDictionaryAsync(c => c.DisplayName, c => c.Id);
            var tags = await db.Tags.ToDictionaryAsync(t => t.DisplayName, t => t.Id);

            // Post IDs
            var post1Id = Guid.Parse("d1a1c1d1-0000-0000-0000-000000000001");
            var post2Id = Guid.Parse("d1a1c1d1-0000-0000-0000-000000000002");
            var post3Id = Guid.Parse("d1a1c1d1-0000-0000-0000-000000000003");
            var post4Id = Guid.Parse("d1a1c1d1-0000-0000-0000-000000000004");
            var post5Id = Guid.Parse("d1a1c1d1-0000-0000-0000-000000000005");
            var post6Id = Guid.Parse("d1a1c1d1-0000-0000-0000-000000000006");
            var post7Id = Guid.Parse("d1a1c1d1-0000-0000-0000-000000000007");
            var post8Id = Guid.Parse("d1a1c1d1-0000-0000-0000-000000000008");
            var post9Id = Guid.Parse("d1a1c1d1-0000-0000-0000-000000000009");


            //use normalized version for category and tag
            var posts = new List<Post>
            {
                // Technology — author1
                new (post1Id, "The Future of AI in 2025", "AI is rapidly transforming every industry...", users["author1"], categories["Technology"]),
                new (post2Id, "Getting Started with .NET 9", "Here is everything you need to know about .NET 9...", users["author1"], categories["Technology"]),
                new (post3Id, "Cloud Computing Best Practices", "Scaling your infrastructure in the cloud requires...", users["author1"], categories["Technology"]),

                // Lifestyle — author2
                new (post4Id, "Minimalism: Less is More", "Decluttering your life starts with your mindset...", users["author2"], categories["Lifestyle"]),
                new (post5Id, "10 Productivity Habits That Changed My Life", "Small daily habits compound into massive results...", users["author2"], categories["Lifestyle"]),

                // Sports — author1
                new (post6Id, "The Science of Athletic Training", "Elite athletes follow structured periodization...", users["author1"], categories["Sports"]),

                // Finance — author2
                new (post7Id, "Investing for Beginners", "Start with index funds before picking individual stocks...", users["author2"], categories["Finance"]),

                // Gaming — author1
                new (post8Id, "The Rise of Esports", "Competitive gaming is now a billion dollar industry...", users["author1"], categories["Gaming"]),

                // Health — author2
                new (post9Id, "Mental Health and Exercise", "Regular exercise has profound effects on mental wellbeing...", users["author2"], categories["Health"]),
            };

            await db.Posts.AddRangeAsync(posts);
            await db.SaveChangesAsync();

            // Wire up tags
            posts[0].SetTags([tags["AI"], tags["Machine Learning"], tags["Web Development"]]);
            posts[1].SetTags([tags[".NET"], tags["Programming"]]);
            posts[2].SetTags([tags["Cloud Computing"], tags["Web Development"]]);
            posts[3].SetTags([tags["Minimalism"], tags["Self Improvement"]]);
            posts[4].SetTags([tags["Productivity"], tags["Self Improvement"]]);
            posts[5].SetTags([tags["Training"], tags["Athletics"]]);
            posts[6].SetTags([tags["Investing"], tags["Personal Finance"]]);
            posts[7].SetTags([tags["Esports"], tags["PC Gaming"]]);
            posts[8].SetTags([tags["Mental Health"], tags["Fitness"]]);

            await db.SaveChangesAsync();

            // Published posts
            posts[0].Publish();
            posts[1].Publish();
            posts[2].Publish();
            posts[3].Publish();
            posts[4].Publish();
            posts[5].Publish();

            posts[6].Publish();
            posts[6].Archive();

            posts[7].Publish();

            posts[8].Publish();
            posts[8].Archive();

            await db.SaveChangesAsync();
        }
    }
}