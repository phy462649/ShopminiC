using LandingPageApp.Domain.Entities;

namespace LandingPageApp.Domain.Repositories;

public interface ICategoryRepository : IGenericRepository<Category>
{
    Task<int> SaveChangesAsync(CancellationToken cancellation = default);
    Task<Category?> GetByNameAsync(string name, CancellationToken cancellation = default);
    Task<bool> HasAnyProductAsync(long categoryId, CancellationToken cancellationToken);
}