using LandingPageApp.Domain.Entities;
using LandingPageApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using LandingPageApp.Domain.Repositories;


namespace LandingPageApp.Infrastructure.Repositories
{
    public class ServiceRepository : IServiceRepository
    {
        private readonly ServicemassageContext _context;
        private readonly DbSet<Service> _dbSet;
        public ServiceRepository(ServicemassageContext context)
        {
            _context = context;
            _dbSet = _context.Set<Service>();
        }
        public IQueryable<Service> Query()
        {
            return _dbSet.AsQueryable();
        }
        public IQueryable<Service> QueryNoTracking()
        {
            return _dbSet.AsNoTracking().AsQueryable();
        }
        public async Task<IEnumerable<Service>> GetAllAsync(CancellationToken cancellation = default)
        {
            return await _dbSet.AsNoTracking().ToListAsync(cancellation);
        }
        public async Task<Service?> GetByIdAsync(int id, CancellationToken cancellation = default)
        {
            return await _dbSet.FindAsync( new object?[] {id} , cancellation);
        }
        public async Task AddAsync(Service entity,CancellationToken cancellation = default)
        {
            await _dbSet.AddAsync(entity,cancellation);
        }
        public void Update(Service entity)
        {
            _dbSet.Update(entity);
        }
        public void Delete(Service entity)
        {
            _dbSet.Remove(entity);
        }
        public async Task<bool> ExistsAsync(Expression<Func<Service, bool>> predicate,CancellationToken cancellation = default)
        {
            return await _dbSet.AnyAsync(predicate,cancellation);
        }
        public async Task<IEnumerable<Service>> FindAsync(Expression<Func<Service, bool>> predicate , CancellationToken cancellation = default)
        {
            return await _dbSet.Where(predicate).ToListAsync(cancellation);
        }
        public async Task<int> CountAsync(Expression<Func<Service, bool>>? predicate = null, CancellationToken cancellation = default)
        {
            if (predicate != null)
            {
                return await _dbSet.CountAsync(predicate, cancellation);
            }
            return await _dbSet.CountAsync(cancellation);
        }
       
    }
}
