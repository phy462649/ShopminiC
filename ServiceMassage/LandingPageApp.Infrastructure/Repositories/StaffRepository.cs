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
        public async Task<Staff?> GetByIdAsync(int staffId)
        {
            return await _dbSet.FindAsync(staffId);
        }
        public async Task AddAsync(Staff staff)
        {
            await _dbSet.AddAsync(staff);
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
        public async Task<bool> ExistsAsync(Expression<Func<Staff, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }
        public async Task<IEnumerable<Staff>> FindAsync(Expression<Func<Staff, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

    }
}
