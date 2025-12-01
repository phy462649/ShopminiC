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
        public async Task<Service?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }
        public async Task AddAsync(Service entity)
        {
            await _dbSet.AddAsync(entity);
        }
        public void Update(Service entity)
        {
            _dbSet.Update(entity);
        }
        public void Delete(Service entity)
        {
            _dbSet.Remove(entity);
        }
        public async Task<bool> ExistsAsync(Expression<Func<Service, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }
        public async Task<IEnumerable<Service>> FindAsync(Expression<Func<Service, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }
    }
}
