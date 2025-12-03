using LandingPageApp.Infrastructure.Data;
using LandingPageApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using LandingPageApp.Domain.Repositories;

namespace LandingPageApp.Infrastructure.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ServicemassageContext servicemassageContext;
        private readonly DbSet<Customer> _dbSet;

        public CustomerRepository(ServicemassageContext context)
        {
            servicemassageContext = context;
            _dbSet = servicemassageContext.Set<Customer>();
        }
        public IQueryable<Customer> Query()
        {
            return _dbSet.AsQueryable();
        }
        public IQueryable<Customer> QueryNoTracking()
        {
            return _dbSet.AsNoTracking();
        }
        public async Task<Customer?> GetByIdAsync(int id,CancellationToken cancellationToken = default)
        {
            return await _dbSet.FindAsync(new object?[] {id} ,cancellationToken );
        }
        public async Task AddAsync(Customer entity, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(entity, default);
        }
        public void Update(Customer entity)
        {
            _dbSet.Update(entity);
        }
        public void Delete(Customer entity)
        {
            _dbSet.Remove(entity);
        }
        public async Task<bool> ExistsAsync(Expression<Func<Customer, bool>> predicate,CancellationToken cancellation = default)
        {
            return await _dbSet.AnyAsync(predicate,cancellation);
        }
        public async Task<IEnumerable<Customer>> FindAsync(Expression<Func<Customer, bool>> predicate, CancellationToken cancellation = default)
        {
            return await _dbSet.Where(predicate).ToListAsync(cancellation);
        }
        public async Task<IEnumerable<Customer?>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking().ToListAsync(cancellationToken);
        }


    }
}
