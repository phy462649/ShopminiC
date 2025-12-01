using LandingPageApp.Domain.Entities;
using LandingPageApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using LandingPageApp.Domain.Repositories;

namespace LandingPageApp.Infrastructure.Repositories
{
    public class RoomRepository : IRoomRepository
    {
        // Implementation of RoomRepository methods would go here
        private readonly ServicemassageContext _context;
        private readonly DbSet<Room> _dbSet;
        public RoomRepository(ServicemassageContext context)
        {
            _context = context;
            _dbSet = _context.Set<Room>();
        }
        public IQueryable<Room> Query()
        {
            return _dbSet.AsQueryable();
        }
        public async Task<Room?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }
        public async Task AddAsync(Room entity)
        {
            await _dbSet.AddAsync(entity);
        }
        public void Update(Room entity)
        {
            _dbSet.Update(entity);
        }
        public void Delete(Room entity)
        {
            _dbSet.Remove(entity);
        }
        public async Task<bool> ExistsAsync(Expression<Func<Room, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task<List<Room>> GetAvailableRoomsAsync(DateTime startDate, DateTime endDate)
        {
            // Example implementation to get available rooms
            var bookedRoomIds = await _context.Bookings
                .Where(b => b.StartTime < endDate && b.EndTime > startDate)
                .Select(b => b.RoomId)
                .ToListAsync();
            return await _dbSet
                .Where(r => !bookedRoomIds.Contains(r.Id))
                .ToListAsync();
        }
        public async Task<int> GetRoomCountAsync()
        {
            return await _dbSet.CountAsync();
        }
        public Task<IEnumerable<Room>> FindAsync(Expression<Func<Room, bool>> predicate)
        {
            return Task.FromResult(_dbSet.Where(predicate).AsEnumerable());
        }

    }
}
