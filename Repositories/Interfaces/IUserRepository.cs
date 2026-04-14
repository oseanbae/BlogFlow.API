using BlogFlow.API.Models;

namespace BlogFlow.API.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid id);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByUsernameOrEmailAsync(string username, string email);
        Task CreateAsync(User user);
        Task<User> UpdateAsync(User user);
                         
    }
}
