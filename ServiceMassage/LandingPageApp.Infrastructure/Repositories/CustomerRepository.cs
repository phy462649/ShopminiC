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
        private readonly DbSet<Customer> dbSet;

        public CustomerRepository(ServicemassageContext context)
        {
            servicemassageContext = context;
            dbSet = servicemassageContext.Set<Customer>();
        }
        public IQueryable<Customer> Query()
        {
            return dbSet.AsQueryable();
        }
        public async Task<Customer?> GetByIdAsync(int id)
        {
            return await dbSet.FindAsync(id);
        }
        public async Task AddAsync(Customer entity)
        {
            await dbSet.AddAsync(entity);
        }
        public void Update(Customer entity)
        {
            dbSet.Update(entity);
        }
        public void Delete(Customer entity)
        {
            dbSet.Remove(entity);
        }
        public async Task<bool> ExistsAsync(Expression<Func<Customer, bool>> predicate)
        {
            return await dbSet.AnyAsync(predicate);
        }
        public async Task<IEnumerable<Customer>> FindAsync(Expression<Func<Customer, bool>> predicate)
        {
            return await dbSet.Where(predicate).ToListAsync();
        }

    }
}
