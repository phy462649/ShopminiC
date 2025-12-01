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

        public async Task<Booking?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task AddAsync(Booking entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Update(Booking entity)
        {
            _dbSet.Update(entity);
        }

        public void Delete(Booking entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task<bool> ExistsAsync(Expression<Func<Booking, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }
        public async Task<IEnumerable<Booking>> FindAsync(Expression<Func<Booking, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }
    }
}
