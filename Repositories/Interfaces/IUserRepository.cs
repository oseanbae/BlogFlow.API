using BlogFlow.API.Domain.Entities;

namespace BlogFlow.API.Repositories.Interfaces
{
    public interface IUserRepository
    {
        IQueryable<User> GetUsersQuery(bool includeDeleted = false);
        Task<User?> GetByIdAsync(Guid id, bool includeDeleted, CancellationToken cancellationToken);
        Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken);
        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);
        Task<User?> GetByUsernameOrEmailAsync(string value, CancellationToken cancellationToken);
        Task CreateAsync(User user, CancellationToken cancellationToken);
        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
