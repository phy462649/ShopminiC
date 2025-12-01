using LandingPageApp.Domain.Entities;
using LandingPageApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using LandingPageApp.Domain.Repositories;

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
        public async Task<Payment?> GetByIdAsync(int paymentId)
        {
            return await _dbSet.FindAsync(paymentId);

        }
        public async Task AddAsync(Payment payment)
        {
            await _dbSet.AddAsync(payment);
        }
        public async void Update(Payment payment)
        {
            _dbSet.Update(payment);
        }
        public async void Delete(Payment payment)
        {
            _dbSet.Remove(payment);
        }
        public async Task<List<Payment>> GetAllPaymentsAsync()
        {
            return await _dbSet.ToListAsync();
        }
        //public async Task<List<Payment>> GetPaymentsByUserIdAsync(Guid userId)
        //{
        //    return await _dbSet.Where(p => p.UserId == userId).ToListAsync();
        //}
        public async Task<int> GetPaymentCountAsync()
        {
            return await _dbSet.CountAsync();
        }
        public IQueryable<Payment> Query()
        {
            return _dbSet.AsQueryable();
        }
        public Task<bool> ExistsAsync(System.Linq.Expressions.Expression<Func<Payment, bool>> predicate)
        {
            return _dbSet.AnyAsync(predicate);
        }
        public Task<IEnumerable<Payment>> FindAsync(System.Linq.Expressions.Expression<Func<Payment, bool>> predicate)
        {
            return _dbSet.Where(predicate).ToListAsync().ContinueWith(t => (IEnumerable<Payment>)t.Result);
        }

    }
}
