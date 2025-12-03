using LandingPageApp.Infrastructure.Data;
using LandingPageApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using LandingPageApp.Domain.Repositories;

namespace LandingPageApp.Infrastructure.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly ServicemassageContext _context;
        private readonly DbSet<Role> _dbSet;
        public RoleRepository(ServicemassageContext context)
        {
            _context = context;
            _dbSet = context.Set<Role>();
        }
        public async Task<Role?> GetByIdAsync(int roleId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FindAsync(new object?[] { roleId }, cancellationToken);
        }

        public async Task AddAsync(Role role, CancellationToken cancellation = default)
        {
            await _dbSet.AddAsync(role,cancellation);
        }
        public void Update(Role role)
        {
            _dbSet.Update(role);

        }
        public void Delete(Role role)
        {
            _dbSet.Remove(role);
        }
        public IQueryable<Role> Query()
        {
            return _dbSet.AsQueryable();
        }
        public async Task<bool> ExistsAsync(Expression<Func<Role, bool>> predicate, CancellationToken cancellation = default)
        {
            return await _dbSet.AnyAsync(predicate, cancellation);
        }
        public async Task<IEnumerable<Role>> FindAsync(Expression<Func<Role, bool>> predicate,CancellationToken cancellation = default)
        {
            return await _dbSet.Where(predicate).ToListAsync(cancellation);
        }
        public async Task<IEnumerable<Role?>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking().ToListAsync(cancellationToken);
        }
        public  IQueryable<Role?> QueryNoTracking()
        {
            return _dbSet.AsNoTracking();
        }                              
    }
}
