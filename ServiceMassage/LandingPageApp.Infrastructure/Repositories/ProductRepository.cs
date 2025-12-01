using LandingPageApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using LandingPageApp.Domain.Entities;
using LandingPageApp.Domain.Repositories;


namespace Landingpage.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ServicemassageContext _context;
        private readonly DbSet<Product> _dbSet;
        public ProductRepository(ServicemassageContext context, DbSet<Product> dbSet)
        {
            _context = context;
            _dbSet = dbSet;
        }
        public async Task<Product?> GetByIdAsync(int productId)
        {
            return await _dbSet.FindAsync(productId);
        }
        public async Task AddProductAsync(Product product)
        {
            await _dbSet.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateProductAsync(Product product)
        {
            _dbSet.Update(product);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteProductAsync(Product product)
        {
            _dbSet.Remove(product);
            await _context.SaveChangesAsync();
        }
        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _dbSet.ToListAsync();
        }
        public async Task<int> GetProductCountAsync()
        {
            return await _dbSet.CountAsync();
        }
        public async Task<bool> ProductExistsAsync(long productId)
        {
            return await _dbSet.AnyAsync(p => p.Id == productId);
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
        public async Task<bool> ExistsAsync(System.Linq.Expressions.Expression<System.Func<Product, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }
        //public Task<Product?> GetByIdAsync(long id)
        //{
        //    return _dbSet.FindAsync(id);
        //}
        public async Task AddAsync(Product entity)
        {
            await _dbSet.AddAsync(entity);
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
        public async Task<IEnumerable<Product>> FindAsync(System.Linq.Expressions.Expression<System.Func<Product, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }


    }
}