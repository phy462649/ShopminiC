using LandingPageApp.Domain.Entities;
using LandingPageApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using LandingPageApp.Domain.Repositories;
using System.Linq.Expressions;

namespace LandingPageApp.Infrastructure.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly ServicemassageContext _context;
        private readonly DbSet<Payment> _dbSet;
        public PaymentRepository(ServicemassageContext context)
        {
            _context = context;
            _dbSet = _context.Set<Payment>();
        }
        public async Task<Payment?> GetByIdAsync(long paymentId,CancellationToken cancellation = default)
        {
            return await _dbSet.FindAsync(new object?[] { paymentId }, default);

        }
        public IQueryable<Payment> QueryNoTracking()
        {
            return _dbSet.AsNoTracking();
        }
        public async Task AddAsync(Payment payment,CancellationToken cancellation = default)
        {
            await _dbSet.AddAsync(payment,cancellation);
        }
        public void Update(Payment payment)
        {
             _dbSet.Update(payment);
        }
        public void Delete(Payment payment)
        {
            _dbSet.Remove(payment);
        }
        public async Task<IEnumerable<Payment>> GetAllAsync(CancellationToken cancellation = default)
        {
            return await _dbSet.AsNoTracking().ToListAsync(cancellation);
        }
    
        public async Task<int> GetPaymentCountAsync()
        {
            return await _dbSet.CountAsync();
        }
        public IQueryable<Payment> Query()
        {
            return _dbSet.AsQueryable();
        }
        public Task<bool> ExistsAsync(Expression<Func<Payment, bool>> predicate, CancellationToken cancellation = default)
        {
            return _dbSet.AnyAsync(predicate,cancellation);
        }
        public async Task<IEnumerable<Payment>> FindAsync(Expression<Func<Payment, bool>> predicate, CancellationToken cancellation = default)
        {
            return await _dbSet.Where(predicate).ToListAsync(cancellation);
        }

    }
}
