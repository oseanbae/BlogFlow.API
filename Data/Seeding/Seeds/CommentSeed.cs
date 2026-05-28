using BlogFlow.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlogFlow.API.Data.Seeding.Seeds
{
    public class CommentSeed : ISeeder
    {
        public int Order => 5;

        public async Task SeedAsync(AppDbContext db)
        {
            if (await db.Comments.AnyAsync()) return;

            // Batch fetch
            var users = await db.Users.ToDictionaryAsync(u => u.Username, u => u.Id);
            var posts = await db.Posts.ToDictionaryAsync(p => p.Title, p => p.Id);

            await db.Comments.AddRangeAsync(
                // Future of AI
                new Comment(Guid.Parse("e1a1c1d1-0000-0000-0000-000000000001"), "This is a great overview of where AI is headed!", users["reader1"], posts["The Future of AI in 2025"]),
                new Comment(Guid.Parse("e1a1c1d1-0000-0000-0000-000000000002"), "I wonder how this will affect job markets.", users["reader2"], posts["The Future of AI in 2025"]),

                // .NET 9
                new Comment(Guid.Parse("e1a1c1d1-0000-0000-0000-000000000003"), "Finally a clear guide on .NET 9, thanks!", users["reader1"], posts["Getting Started with .NET 9"]),
                new Comment(Guid.Parse("e1a1c1d1-0000-0000-0000-000000000004"), "Would love a follow-up on minimal APIs.", users["reader2"], posts["Getting Started with .NET 9"]),

                // Cloud Computing
                new Comment(Guid.Parse("e1a1c1d1-0000-0000-0000-000000000005"), "We applied these practices at work and it made a huge difference.", users["reader1"], posts["Cloud Computing Best Practices"]),

                // Minimalism
                new Comment(Guid.Parse("e1a1c1d1-0000-0000-0000-000000000006"), "Minimalism changed my life too, great read.", users["reader2"], posts["Minimalism: Less is More"]),
                new Comment(Guid.Parse("e1a1c1d1-0000-0000-0000-000000000007"), "I struggle with letting go of things, this was inspiring.", users["reader1"], posts["Minimalism: Less is More"]),

                // Productivity
                new Comment(Guid.Parse("e1a1c1d1-0000-0000-0000-000000000008"), "Habit stacking is underrated, glad you mentioned it.", users["reader2"], posts["10 Productivity Habits That Changed My Life"]),

                // Athletic Training
                new Comment(Guid.Parse("e1a1c1d1-0000-0000-0000-000000000009"), "Periodization is key, most people skip this entirely.", users["reader1"], posts["The Science of Athletic Training"]),

                // Investing
                new Comment(Guid.Parse("e1a1c1d1-0000-0000-0000-000000000010"), "Index funds are the way to go for beginners, solid advice.", users["reader2"], posts["Investing for Beginners"]),
                new Comment(Guid.Parse("e1a1c1d1-0000-0000-0000-000000000011"), "What about ETFs? Are they worth considering too?", users["reader1"], posts["Investing for Beginners"]),

                // Esports
                new Comment(Guid.Parse("e1a1c1d1-0000-0000-0000-000000000012"), "The growth of esports is insane, I remember when it was niche.", users["reader2"], posts["The Rise of Esports"]),

                // Mental Health
                new Comment(Guid.Parse("e1a1c1d1-0000-0000-0000-000000000013"), "Exercise genuinely saved my mental health, thank you for writing this.", users["reader1"], posts["Mental Health and Exercise"]),
                new Comment(Guid.Parse("e1a1c1d1-0000-0000-0000-000000000014"), "More people need to read this, sharing it now.", users["reader2"], posts["Mental Health and Exercise"])
            );

            await db.SaveChangesAsync();
        }
    }
}