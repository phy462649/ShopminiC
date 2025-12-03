using LandingPageApp.Domain.Entities;
using LandingPageApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using LandingPageApp.Domain.Repositories;
namespace LandingPageApp.Infrastructure.Repositories
{

    public class BookingRepository : IBookingRepository
    {
        private readonly ServicemassageContext _context;
        private readonly DbSet<Booking> _dbSet;

        public BookingRepository(ServicemassageContext context)
        {
            _context = context;
            _dbSet = _context.Set<Booking>();
        }

        public IQueryable<Booking> Query()
        {
            return _dbSet.AsQueryable();
        }
        public IQueryable<Booking> QueryNoTracking()
        {
            return _dbSet.AsNoTracking();
        }

        public async Task<Booking?> GetByIdAsync(int id, CancellationToken cancellation = default)
        {
            return await _dbSet.FindAsync(new object?[] {id}, cancellation);
        }

        public async Task AddAsync(Booking entity, CancellationToken cancellation = default)
        {
            await _dbSet.AddAsync(entity, cancellation);
        }

        public void Update(Booking entity)
        {
            _dbSet.Update(entity);
        }

        public void Delete(Booking entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task<bool> ExistsAsync(Expression<Func<Booking, bool>> predicate, CancellationToken cancellation = default)
        {
            return await _dbSet.AnyAsync(predicate, cancellation);
        }
        public async Task<IEnumerable<Booking>> FindAsync(Expression<Func<Booking, bool>> predicate, CancellationToken cancellation = default)
        {
            return await _dbSet.Where(predicate).ToListAsync(cancellation);
        }
        public async Task<IEnumerable<Booking?>> GetAllAsync(CancellationToken cancellation = default)
        {
            return await _dbSet.AsNoTracking().ToListAsync(cancellation);
        }
    }
}
