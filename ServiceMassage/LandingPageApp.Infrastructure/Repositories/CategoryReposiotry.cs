using LandingPageApp.Domain.Repositories;
using LandingPageApp.Domain.Entities;
using LandingPageApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;


namespace LandingPageApp.Infrastructure.Repositories
{
    public class CategoryReposiotry : ICategoryRepository
    {
        private readonly ServicemassageContext _context;
        private readonly DbSet<Category> _dbSet;
        public CategoryReposiotry(ServicemassageContext context)
        {
            _context = context;
            _dbSet = _context.Set<Category>();
        }
        public IQueryable<Category> Query()
        {
            return _dbSet.AsQueryable();
        }
        public IQueryable<Category> QueryNoTracking()
        {
            return _dbSet.AsNoTracking();
        }
        public async Task<Category?> GetByIdAsync(long id, CancellationToken cancellation = default)
        {
            return await _dbSet.FindAsync(new object?[] { id }, cancellation);
        }
        public async Task AddAsync(Category entity, CancellationToken cancellation = default)
        {
            await _dbSet.AddAsync(entity, cancellation);
            
        }
        public void Update(Category entity)
        {
            _dbSet.Update(entity);
        }
        public void Delete(Category entity)
        {
            _dbSet.Remove(entity);
        }
        public async Task<bool> ExistsAsync(Expression<Func<Category, bool>> predicate, CancellationToken cancellation = default)
        {
            return await _dbSet.AnyAsync(predicate, cancellation);
        }

        public async Task<IEnumerable<Category>> FindAsync(Expression<Func<Category, bool>> predicate, CancellationToken cancellation = default)
        {
            return await _dbSet.Where(predicate).ToListAsync(cancellation);
        }
        public async Task<IEnumerable<Category>> GetAllAsync(CancellationToken cancellation = default)
        {
            return await _dbSet.AsNoTracking().ToListAsync(cancellation);
        }
        public async Task<bool> ExistsAsync(Expression<System.Func<Category, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }
        public async Task<Category?> GetByNameAsync(string name, CancellationToken cancellation = default)
        {
            return await _dbSet.FirstOrDefaultAsync(c => c.Name == name, cancellation);
        }
        public async Task<int> SaveChangesAsync(CancellationToken cancellation = default)
        {
            return await  _context.SaveChangesAsync(cancellation);
        }
        public async Task<bool> HasAnyProductAsync(long categoryId, CancellationToken cancellation = default)
        {
            return await _context.Products.AsNoTracking().AnyAsync(p => p.CategoryId == categoryId, cancellation);
        }

    } 
}
