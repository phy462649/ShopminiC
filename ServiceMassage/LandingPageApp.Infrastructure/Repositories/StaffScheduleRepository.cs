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
        public async Task<StaffSchedule?> GetByIdAsync(long staffScheduleId, CancellationToken cancellation = default)
        {
            return await _dbSet.FindAsync(new object?[] { staffScheduleId }, staffScheduleId);
        }
        public async Task AddAsync(StaffSchedule staffSchedule, CancellationToken cancellation = default)
        {
            await _dbSet.AddAsync(staffSchedule, cancellation);
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
        public IQueryable<StaffSchedule> QueryNoTracking()
        {
            return _dbSet.AsNoTracking();
        }
        public async Task<bool> ExistsAsync(Expression<Func<StaffSchedule, bool>> predicate, CancellationToken cancellation = default)
        {
            return await _dbSet.AnyAsync(predicate, cancellation);
        }
        public async Task<IEnumerable<StaffSchedule>> FindAsync(Expression<Func<StaffSchedule, bool>> predicate, CancellationToken cancellation = default)
        {
            return await _dbSet.Where(predicate).ToListAsync(cancellation);
        }
        public async Task<IEnumerable<StaffSchedule>> GetAllAsync(CancellationToken cancellation = default)
        {
            return await _dbSet.AsNoTracking().ToListAsync(cancellation);
        }   
    }
}
