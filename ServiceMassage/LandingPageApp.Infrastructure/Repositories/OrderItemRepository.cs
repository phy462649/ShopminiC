using LandingPageApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using LandingPageApp.Domain.Entities;
using System.Linq.Expressions;
using LandingPageApp.Domain.Repositories;

namespace LandingPageApp.Infrastructure.Repositories
{
    public class OrderItemRepository : IOrderItemRepository
    {
        private readonly ServicemassageContext _context;
        private readonly DbSet<OrderItem> _dbSet;

        public OrderItemRepository(ServicemassageContext context)
        {
            _context = context;
            _dbSet = _context.Set<OrderItem>();
        }
        public IQueryable<OrderItem> Query()
        {
            return _dbSet.AsQueryable();
        }
        public IQueryable<OrderItem> QueryNoTracking()
        {
            return _dbSet.AsNoTracking();
        }
        public async Task<IEnumerable<OrderItem>> GetAllAsync(CancellationToken  cancellation = default)
        {
            return await _dbSet.AsNoTracking().ToListAsync(default);
        }
        public async Task<OrderItem?> GetByIdAsync(long id,CancellationToken cancellation = default)
        {
            return await _dbSet.FindAsync(new object?[] {id}, cancellation);
        }
        public async Task AddAsync(OrderItem entity, CancellationToken cancellation = default)
        {
            await _dbSet.AddAsync(entity,cancellation);
        }
        public void Update(OrderItem entity)
        {
            _dbSet.Update(entity);
        }
        public void Delete(OrderItem entity)
        {
            _dbSet.Remove(entity);
        }
        public async Task<bool> ExistsAsync(Expression<Func<OrderItem, bool>> predicate, CancellationToken cancellation = default)
        {
            return await _dbSet.AnyAsync(predicate,cancellation);
        }
        public async Task<IEnumerable<OrderItem>> FindAsync(Expression<Func<OrderItem, bool>> predicate, CancellationToken cancellation = default)
        {
            return await _dbSet.Where(predicate).ToListAsync(cancellation);
        }
    }
}


