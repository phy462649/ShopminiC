using LandingPageApp.Infrastructure.Data;
using LandingPageApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using LandingPageApp.Domain.Repositories;
using System.Linq.Expressions;
using System.Threading.Tasks;
namespace LandingPageApp.Infrastructure.Repositories
{
    public class PersonRepository : IPersonRepository
    {
        private readonly ServicemassageContext _context;
        private readonly DbSet<Person> _dbSet;
        public PersonRepository(ServicemassageContext context)
        {
            _context = context;
            _dbSet = _context.Set<Person>();
        }
        
        public async Task<Person?> GetByIdAsync(long personId, CancellationToken cancellation = default)
        {
            return await _dbSet.FindAsync(new object?[] {personId},cancellation);
        }
        public async Task<IEnumerable<Person>> GetAllAsync(CancellationToken cancellation = default)
        {
            return await _dbSet.AsNoTracking().ToListAsync(cancellation);
        }
        public async Task<bool> ExistsAsync(Expression<Func<Person,bool>> predicate ,CancellationToken cancellation = default)
        {
            return await _dbSet.AnyAsync(predicate,cancellation);
        }
        public IQueryable<Person> Query()
        {
            return _dbSet.AsQueryable();
        }
        public IQueryable<Person> QueryNoTracking()
        {
            return _dbSet.AsNoTracking();
        }
        public async Task<bool> ExistsAsync(Expression<System.Func<Person, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }
        //public async Task<object> Del

        public async Task AddAsync(Person person, CancellationToken cancellation = default)
        {
            await _dbSet.AddAsync(person, cancellation);
           
        }
        public void Update(Person person)
        {
             _dbSet.Update(person);
        }
        public void Delete(Person person)
        {
            _dbSet.Remove(person);
        }
        public async Task<Person?> GetByUsernameAsync(string username, CancellationToken cancellation = default)
        {
            return await _dbSet.Include(p => p.Role).FirstOrDefaultAsync(p => p.Username == username, cancellation);
        }
        public async Task<IEnumerable<Person>> FindAsync(Expression<Func<Person, bool>> predicate, CancellationToken cancellation = default)
        {
             return await _dbSet.Where(predicate).ToListAsync(cancellation);
        }
        public async Task<Person> FindByUsernameAsync(string username, CancellationToken cancellation = default)
        {
            var person = await _dbSet.Include(p => p.Role).FirstOrDefaultAsync(p => p.Username == username, cancellation);
             if (person != null)
             {
                _context.Entry(person).State = EntityState.Detached;
             }
             return person!;
        }

        public async Task<string> GetRoleNameById(long id, CancellationToken cancellation = default)
        {
            var person = await _dbSet.FindAsync(new object?[] {id}, cancellation);
            return person?.Name ?? string.Empty;
        }
        public async Task SaveChangesAsync(CancellationToken cancellation = default)
        {
            await _context.SaveChangesAsync(cancellation);
        }
    }
}
