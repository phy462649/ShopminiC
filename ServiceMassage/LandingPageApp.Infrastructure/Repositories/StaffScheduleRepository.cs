using LandingPageApp.Domain.Entities;
using LandingPageApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using LandingPageApp.Domain.Repositories;

namespace LandingPageApp.Infrastructure.Repositories
{
    public class StaffScheduleRepository : IStaffScheduleRepository
    {
        private readonly ServicemassageContext _context;
        private readonly DbSet<StaffSchedule> _dbSet;
        public StaffScheduleRepository(ServicemassageContext context)
        {
            _context = context;
            _dbSet = context.Set<StaffSchedule>();
        }
        public async Task<StaffSchedule?> GetByIdAsync(int staffScheduleId)
        {
            return await _dbSet.FindAsync(staffScheduleId);
        }
        public async Task AddAsync(StaffSchedule staffSchedule)
        {
            await _dbSet.AddAsync(staffSchedule);
        }
        public void Update(StaffSchedule staffSchedule)
        {
            _dbSet.Update(staffSchedule);
        }
        public void Delete(StaffSchedule staffSchedule)
        {
            _dbSet.Remove(staffSchedule);
        }
        public IQueryable<StaffSchedule> Query()
        {
            return _dbSet.AsQueryable();
        }
        public async Task<bool> ExistsAsync(Expression<Func<StaffSchedule, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }
        public async Task<IEnumerable<StaffSchedule>> FindAsync(Expression<Func<StaffSchedule, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }
    }
}
