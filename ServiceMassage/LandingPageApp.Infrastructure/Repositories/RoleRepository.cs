using LandingPageApp.Infrastructure.Data;
using LandingPageApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using LandingPageApp.Domain.Repositories;

namespace LandingPageApp.Infrastructure.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly ServicemassageContext _context;
        private readonly DbSet<Role> _dbSet;
        public RoleRepository(ServicemassageContext context)
        {
            _context = context;
            _dbSet = context.Set<Role>();
        }
        public async Task<Role?> GetByIdAsync(int roleId)
        {
            return await _dbSet.FindAsync(roleId);
        }
        public async Task AddAsync(Role role)
        {
            await _dbSet.AddAsync(role);
        }
        public async void Update(Role role)
        {
            _dbSet.Update(role);

        }
        public async void Delete(Role role)
        {
            _dbSet.Remove(role);
            //await _context.SaveChangesAsync();
        }
        public IQueryable<Role> Query()
        {
            return _dbSet.AsQueryable();
        }
        public async Task<bool> ExistsAsync(Expression<Func<Role, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }
        public async Task<IEnumerable<Role>> FindAsync(Expression<Func<Role, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }
    }
}
