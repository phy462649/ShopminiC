using LandingPageApp.Domain.Entities;
using LandingPageApp.Domain.Repositories;
using LandingPageApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;



namespace LandingPageApp.Infrastructure.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ServicemassageContext _context;
        private readonly DbSet<Account> _dbSet;
        public AccountRepository(ServicemassageContext context)
        {
            _context = context;
            _dbSet = _context.Set<Account>();
        }
        public IQueryable<Account> Query()
        {
            return _dbSet.AsQueryable();
        }
        public IQueryable<Account> QueryNoTracking()
        {
            return _dbSet.AsNoTracking();
        }
        public async Task<Account?> GetByIdAsync(long id, CancellationToken cancellation = default)
        {
            return await _dbSet.FindAsync(new object?[] { id }, cancellation);
        }
        public async Task AddAsync(Account entity, CancellationToken cancellation = default)
        {
            await _dbSet.AddAsync(entity, cancellation);
        }
        
        public void Delete(Account entity)
        {
            _dbSet.Update(entity);
        }
        public async Task<bool> ExistsAsync(Expression<Func<Account, bool>> predicate, CancellationToken cancellation = default)
        {
            return await _dbSet.AnyAsync(predicate, cancellation);
        }
        public async Task<IEnumerable<Account>> FindAsync(Expression<Func<Account, bool>> predicate, CancellationToken cancellation = default)
        {
            return await _dbSet.Where(predicate).ToListAsync(cancellation);
        }
        public async Task<IEnumerable<Account?>> GetAllAsync(CancellationToken cancellation = default)
        {
            return await _dbSet.AsNoTracking().ToListAsync(cancellation);
        }
        public async Task<Account?> FindByUsernameAsync(string username, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FirstOrDefaultAsync(a => a.Username == username, cancellationToken);
        }
        public async Task<Account?> UserDetailDTO(string username, CancellationToken cancellation = default)
        {
            return await _dbSet
                        .Include(a => a.Customer)
                        .Include(a => a.Staff)
                            .ThenInclude(s => s.Roles) // <-- sửa từ StaffRole thành Roles
                        .FirstOrDefaultAsync(a => a.Username == username, cancellation);
        }
        public  async Task<Account?> GetByUsernameAsync(string username)
        {
            return await _dbSet.FirstOrDefaultAsync(a => a.Username == username);
        }

        public async Task<Account?> FindByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FirstOrDefaultAsync(a => a.Email == email, cancellationToken);
        }

        public async Task UpdateAsync(Account account, CancellationToken cancellationToken = default)
        {
            _dbSet.Update(account);
            await _context.SaveChangesAsync(cancellationToken);
        }

    }

}
