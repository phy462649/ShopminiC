using LandingPageApp.Domain.Entities;

namespace LandingPageApp.Domain.Repositories
{
    public interface IPersonRepository : IGenericRepository<Person>
    {
        public Task<Person> FindByUsernameAsync(string username, CancellationToken cancellation = default);
        public Task<string> GetRoleNameById(long id, CancellationToken cancellation = default);
    }
}
