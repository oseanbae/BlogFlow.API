using Microsoft.EntityFrameworkCore;

namespace BlogFlow.API.Data.Seeding
{
    public class DatabaseSeeder
    {
        private readonly AppDbContext _context;
        private readonly ILogger<DatabaseSeeder> _logger;
        private readonly IEnumerable<ISeeder> _seeders;

        public DatabaseSeeder(AppDbContext context, ILogger<DatabaseSeeder> logger, IEnumerable<ISeeder> seeders)
        {
            _context = context;
            _logger = logger;
            _seeders = seeders;
        }

        public async Task SeedAsync()
        {
            foreach (var seeder in _seeders.OrderBy(s => s.Order))
            {
                var name = seeder.GetType().Name;

                try
                {
                    await seeder.SeedAsync(_context);
                    _logger.LogInformation("Seeder completed: {Seeder}", name);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Seeder failed: {Seeder}", name);
                    throw;
                }
            }
        }
    }
}