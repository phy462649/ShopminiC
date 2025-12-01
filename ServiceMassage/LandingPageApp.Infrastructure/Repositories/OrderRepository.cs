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
        public async Task<Order?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }
        public async Task AddAsync(Order entity)
        {
            await _dbSet.AddAsync(entity);
        }
        public void Update(Order entity)
        {
            _dbSet.Update(entity);
        }

        public void Delete(Order entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task<bool> ExistsAsync(Expression<Func<Order, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }
        public async Task<IEnumerable<Order>> FindAsync(Expression<Func<Order, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }
    }
}