using LandingPageApp.Domain.Repositories;
using System.Linq.Expressions;
using LandingPageApp.Domain.Entities;
using LandingPageApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


namespace LandingPageApp.Infrastructure.Repositories
{
    public class BookingServiceRepository : IBookingServiceRepository
    {
        private readonly ServicemassageContext _context;
        private readonly DbSet<BookingService> _dbSet;
        public BookingServiceRepository(ServicemassageContext context)
        {
            _context = context;
            _dbSet = _context.Set<BookingService>();
        }
        public IQueryable<BookingService> Query()
        {
            return _dbSet.AsQueryable();
        }
        public IQueryable<BookingService> QueryNoTracking()
        {
            return _dbSet.AsNoTracking();
        }
        public async Task<BookingService?> GetByIdAsync(int id,CancellationToken cancellation = default)
        {
            return await _dbSet.FindAsync(new object?[] {id},cancellation);
        }
        public async Task AddAsync(BookingService entity,CancellationToken cancellation = default)
        {
            await _dbSet.AddAsync(entity,cancellation);
        }
        public void Update(BookingService entity)
        {
            _dbSet.Update(entity);
        }
        public void Delete(BookingService entity)
        {
            _dbSet.Remove(entity);
        }
        public async Task<bool> ExistsAsync(Expression<Func<BookingService, bool>> predicate, CancellationToken cancellation = default)
        {
            return await _dbSet.AnyAsync(predicate,cancellation);
        }
    
        public async Task<IEnumerable<BookingService>> FindAsync(Expression<Func<BookingService, bool>> predicate,CancellationToken cancellation = default)
        {
            return await _dbSet.Where(predicate).ToListAsync(cancellation);
        }
        public async Task<IEnumerable<BookingService?>> GetAllAsync(CancellationToken cancellation = default)
        {
            return await _dbSet.AsNoTracking().ToListAsync(cancellation);
        }
    }
}
