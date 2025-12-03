using LandingPageApp.Domain.Entities;
using LandingPageApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using LandingPageApp.Domain.Repositories;

namespace LandingPageApp.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ServicemassageContext _context;
        private readonly DbSet<Order> _dbSet;
        public OrderRepository(ServicemassageContext context)
        {
            _context = context;
            _dbSet = _context.Set<Order>();
        }
        public IQueryable<Order> Query()
        {
            return _dbSet.AsQueryable();
        }
        public async Task<IEnumerable<Order>> GetAllAsync(CancellationToken cancellation = default)
        {
            return await _dbSet.ToListAsync(cancellation);
        }
        public IQueryable<Order> QueryNoTracking()
        {
            return _dbSet.AsNoTracking();
        }
        public async Task<Order?> GetByIdAsync(int id, CancellationToken cancellation = default)
        {
            return await _dbSet.FindAsync(new object?[] {id} ,cancellation);
        }
        public async Task AddAsync(Order entity, CancellationToken cancellation = default)
        {
            await _dbSet.AddAsync(entity , cancellation);
        }
        public void Update(Order entity)
        {
            _dbSet.Update(entity);
        }

        public void Delete(Order entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task<bool> ExistsAsync(Expression<Func<Order, bool>> predicate, CancellationToken cancellation = default)
        {
            return await _dbSet.AnyAsync(predicate, cancellation);
        }
        public async Task<IEnumerable<Order>> FindAsync(Expression<Func<Order, bool>> predicate, CancellationToken cancellation = default)
        {
            return await _dbSet.Where(predicate).ToListAsync(cancellation);
        }
    }
}