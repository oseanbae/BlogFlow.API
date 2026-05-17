using BlogFlow.API.Models;

namespace BlogFlow.API.Repositories.Interfaces
{
    public interface IUserRepository
    {
        IQueryable<User> GetUsersQuery();
        Task<User?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken);
        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);
        Task<User?> GetByUsernameOrEmailAsync(string value, CancellationToken cancellationToken);
        Task CreateAsync(User user, CancellationToken cancellationToken);
        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
