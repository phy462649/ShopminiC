
using LandingPageApp.Domain.Entities;

namespace LandingPageApp.Domain.Repositories
{
    public interface IRoleRepository : IGenericRepository<Role>
    {
        Task<int> SaveChangesAsync(CancellationToken cancellation = default);
        Task<bool> HasAnyPersonAsync(long roleId, CancellationToken cancellationToken);

    }
}
