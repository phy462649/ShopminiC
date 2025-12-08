using LandingPageApp.Domain.Entities;

namespace LandingPageApp.Domain.Repositories
{
    public interface IAccountRepository
    {
        Task<Account> FindByUsernameAsync(string username, CancellationToken cancellationToken = default);
        Task<Account> UserDetailDTO(string username, CancellationToken cancellationToken = default);
        Task<Account> GetByUsernameAsync(string username);
        Task<Account> GetByIdAsync(long id, CancellationToken cancellationToken = default);
        Task<Account> FindByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task AddAsync(Account account,CancellationToken cancellationToken = default);
        Task UpdateAsync(Account account, CancellationToken cancellationToken = default);
    }
}
