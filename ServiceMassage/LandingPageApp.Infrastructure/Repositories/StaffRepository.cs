using LandingPageApp.Infrastructure.Data;
using LandingPageApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using LandingPageApp.Domain.Repositories;

namespace LandingPageApp.Infrastructure.Repositories
{
    public class StaffRepository : IStaffRepository
    {
        private readonly ServicemassageContext _context;
        private readonly DbSet<Staff> _dbSet;
        public StaffRepository(ServicemassageContext context)
        {
            _context = context;
            _dbSet = context.Set<Staff>();
        }
        public async Task<Staff?> GetByIdAsync(int staffId, CancellationToken cancellation = default)
        {
            return await _dbSet.FindAsync( new object?[] { staffId }, cancellation);
        }
        public async Task AddAsync(Staff staff, CancellationToken cancellation = default)
        {
            await _dbSet.AddAsync(staff, cancellation);
        }
        public void Update(Staff staff)
        {
            _dbSet.Update(staff);
        }
        public void Delete(Staff staff)
        {
            _dbSet.Remove(staff);
        }
        public IQueryable<Staff> Query()
        {
            return _dbSet.AsQueryable();
        }
        public IQueryable<Staff> QueryNoTracking()
        {
            return _dbSet.AsNoTracking();
        }
        public async Task<IEnumerable<Staff>> GetAllAsync(CancellationToken cancellation = default)
        {
            return await _dbSet.AsNoTracking().ToListAsync(cancellation);
        }
        public async Task<bool> ExistsAsync(Expression<Func<Staff, bool>> predicate , CancellationToken cancellation = default)
        {
            return await _dbSet.AnyAsync(predicate,cancellation);
        }
        public async Task<IEnumerable<Staff>> FindAsync(Expression<Func<Staff, bool>> predicate,CancellationToken cancellation = default)
        {
            return await _dbSet.Where(predicate).ToListAsync(default);
        }

    }
}
