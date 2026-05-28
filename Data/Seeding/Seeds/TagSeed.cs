using BlogFlow.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlogFlow.API.Data.Seeding.Seeds
{
    public class TagSeed : ISeeder
    {
        public int Order => 3;

        public async Task SeedAsync(AppDbContext db)
        {
            if (await db.Tags.AnyAsync()) return;

            // Technology
            var aiId = Guid.Parse("c1a1c1d1-0000-0000-0000-000000000001");
            var mlId = Guid.Parse("c1a1c1d1-0000-0000-0000-000000000002");
            var dotnetId = Guid.Parse("c1a1c1d1-0000-0000-0000-000000000003");
            var webDevId = Guid.Parse("c1a1c1d1-0000-0000-0000-000000000004");
            var cloudId = Guid.Parse("c1a1c1d1-0000-0000-0000-000000000005");

            // Lifestyle
            var productivityId = Guid.Parse("c1a1c1d1-0000-0000-0000-000000000006");
            var minimalismId = Guid.Parse("c1a1c1d1-0000-0000-0000-000000000007");
            var selfImprovementId = Guid.Parse("c1a1c1d1-0000-0000-0000-000000000008");

            // Sports
            var basketballId = Guid.Parse("c1a1c1d1-0000-0000-0000-000000000009");
            var trainingId = Guid.Parse("c1a1c1d1-0000-0000-0000-000000000010");
            var athleticsId = Guid.Parse("c1a1c1d1-0000-0000-0000-000000000011");

            // Travel
            var adventureId = Guid.Parse("c1a1c1d1-0000-0000-0000-000000000012");
            var backpackingId = Guid.Parse("c1a1c1d1-0000-0000-0000-000000000013");
            var travelTipsId = Guid.Parse("c1a1c1d1-0000-0000-0000-000000000014");

            // Food
            var recipesId = Guid.Parse("c1a1c1d1-0000-0000-0000-000000000015");
            var cookingId = Guid.Parse("c1a1c1d1-0000-0000-0000-000000000016");
            var foodCultureId = Guid.Parse("c1a1c1d1-0000-0000-0000-000000000017");

            // Finance
            var investingId = Guid.Parse("c1a1c1d1-0000-0000-0000-000000000018");
            var budgetingId = Guid.Parse("c1a1c1d1-0000-0000-0000-000000000019");
            var personalFinanceId = Guid.Parse("c1a1c1d1-0000-0000-0000-000000000020");

            // Gaming
            var esportsId = Guid.Parse("c1a1c1d1-0000-0000-0000-000000000021");
            var pcGamingId = Guid.Parse("c1a1c1d1-0000-0000-0000-000000000022");
            var gameReviewsId = Guid.Parse("c1a1c1d1-0000-0000-0000-000000000023");

            // Education
            var studyTipsId = Guid.Parse("c1a1c1d1-0000-0000-0000-000000000024");
            var programmingId = Guid.Parse("c1a1c1d1-0000-0000-0000-000000000025");
            var careerGrowthId = Guid.Parse("c1a1c1d1-0000-0000-0000-000000000026");

            // Health
            var fitnessId = Guid.Parse("c1a1c1d1-0000-0000-0000-000000000027");
            var nutritionId = Guid.Parse("c1a1c1d1-0000-0000-0000-000000000028");
            var mentalHealthId = Guid.Parse("c1a1c1d1-0000-0000-0000-000000000029");

            await db.Tags.AddRangeAsync(
                // Technology
                new Tag(aiId, "AI"),
                new Tag(mlId, "Machine Learning"),
                new Tag(dotnetId, ".NET"),
                new Tag(webDevId, "Web Development"),
                new Tag(cloudId, "Cloud Computing"),

                // Lifestyle
                new Tag(productivityId, "Productivity"),
                new Tag(minimalismId, "Minimalism"),
                new Tag(selfImprovementId, "Self Improvement"),

                // Sports
                new Tag(basketballId, "Basketball"),
                new Tag(trainingId, "Training"),
                new Tag(athleticsId, "Athletics"),

                // Travel
                new Tag(adventureId, "Adventure"),
                new Tag(backpackingId, "Backpacking"),
                new Tag(travelTipsId, "Travel Tips"),

                // Food
                new Tag(recipesId, "Recipes"),
                new Tag(cookingId, "Cooking"),
                new Tag(foodCultureId, "Food Culture"),

                // Finance
                new Tag(investingId, "Investing"),
                new Tag(budgetingId, "Budgeting"),
                new Tag(personalFinanceId, "Personal Finance"),

                // Gaming
                new Tag(esportsId, "Esports"),
                new Tag(pcGamingId, "PC Gaming"),
                new Tag(gameReviewsId, "Game Reviews"),

                // Education
                new Tag(studyTipsId, "Study Tips"),
                new Tag(programmingId, "Programming"),
                new Tag(careerGrowthId, "Career Growth"),

                // Health
                new Tag(fitnessId, "Fitness"),
                new Tag(nutritionId, "Nutrition"),
                new Tag(mentalHealthId, "Mental Health")
            );

            await db.SaveChangesAsync();
        }
    }
}