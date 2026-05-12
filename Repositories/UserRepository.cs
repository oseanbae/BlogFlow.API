using BlogFlow.API.Data;
using BlogFlow.API.Models;
using BlogFlow.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlogFlow.API.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        public UserRepository(AppDbContext context)
        {
            _context = context;
        }
        
        public async Task CreateAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }
        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);
        }
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetTrackedByIdAsync(Guid id)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id);
        }
        public async Task<User?> GetByUsernameOrEmailAsync(string value)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => 
                    u.Username == value || u.Email == value);
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
