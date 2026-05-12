using BlogFlow.API.Models;

namespace BlogFlow.API.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetTrackedByIdAsync(Guid id);
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByEmailAsync(string email);
        Task CreateAsync(User user);
        Task SaveChangesAsync();
    }
}
