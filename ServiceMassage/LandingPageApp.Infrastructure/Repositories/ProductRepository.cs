using LandingPageApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using LandingPageApp.Domain.Entities;
using LandingPageApp.Domain.Repositories;
using System.Linq.Expressions;

namespace LandingPageApp.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ServicemassageContext _context;
        private readonly DbSet<Product> _dbSet;
        public ProductRepository(ServicemassageContext context)
        {
            _context = context;
            _dbSet = _context.Set<Product>();
        }
        public async Task<Product?> GetByIdAsync(int productId, CancellationToken cancellation = default)
        {
            return await _dbSet.FindAsync(new object?[] {productId},cancellation);
        }
        public async Task<IEnumerable<Product>> GetAllAsync(CancellationToken cancellation = default)
        {
            return await _dbSet.AsNoTracking().ToListAsync(cancellation);
        }
        public async Task<int> GetProductCountAsync()
        {
            return await _dbSet.CountAsync();
        }
        public async Task<bool> ExistsAsync(Expression<Func<Product,bool>> predicate ,CancellationToken cancellation = default)
        {
            return await _dbSet.AnyAsync(predicate,cancellation);
        }

        public async Task<List<Product>> SearchProductsAsync(string searchTerm)
        {
            return await _dbSet.Where(p => p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm)).ToListAsync();
        }

        public async Task<List<Product>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            return await _dbSet.Where(p => p.Price >= minPrice && p.Price <= maxPrice).ToListAsync();
        }
        public async Task<List<Product>> GetNewArrivalsAsync(DateTime sinceDate)
        {
            return await _dbSet.Where(p => p.CreatedAt >= sinceDate).ToListAsync();
        }
        public IQueryable<Product> Query()
        {
            return _dbSet.AsQueryable();
        }
        public IQueryable<Product> QueryNoTracking()
        {
            return _dbSet.AsNoTracking();
        }
        public async Task<bool> ExistsAsync(Expression<System.Func<Product, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }
   
        public async Task AddAsync(Product entity,CancellationToken cancellation = default)
        {
            await _dbSet.AddAsync(entity, cancellation);
            return;
        }
        public void Update(Product entity)
        {
            _dbSet.Update(entity);
        }
        public void Delete(Product entity)
        {
            _dbSet.Remove(entity);
        }
        public async Task<IEnumerable<Product>> FindAsync(Expression<System.Func<Product, bool>> predicate, CancellationToken cancellation = default)
        {
            return await _dbSet.Where(predicate).ToListAsync(cancellation);
        }


    }
}