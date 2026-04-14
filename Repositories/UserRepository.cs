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
            await _context.SaveChangesAsync();
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        Task<User?> IUserRepository.GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<User?> GetByUsernameOrEmailAsync(string username, string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username|| u.Email == email);
        }

        Task<User> IUserRepository.UpdateAsync(User user)
        {
            throw new NotImplementedException();
        }
    }
}
