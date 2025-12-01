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
        public async Task<OrderItem?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }
        public async Task AddAsync(OrderItem entity)
        {
            await _dbSet.AddAsync(entity);
        }
        public void Update(OrderItem entity)
        {
            _dbSet.Update(entity);
        }
        public void Delete(OrderItem entity)
        {
            _dbSet.Remove(entity);
        }
        public async Task<bool> ExistsAsync(Expression<Func<OrderItem, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }
        public async Task<IEnumerable<OrderItem>> FindAsync(Expression<Func<OrderItem, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }
    }
}


