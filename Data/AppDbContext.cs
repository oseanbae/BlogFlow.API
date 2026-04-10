using Microsoft.EntityFrameworkCore;
using BlogFlow.API.Models;

namespace BlogFlow.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : 
            base(options) { }
        public DbSet<User> Users { get; set; } = default!;
    }
}
