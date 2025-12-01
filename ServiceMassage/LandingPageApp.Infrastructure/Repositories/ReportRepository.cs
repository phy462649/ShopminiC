
using LandingPageApp.Infrastructure.Data;
using LandingPageApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using LandingPageApp.Domain.Repositories;

namespace LandingPageApp.Infrastructure.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly ServicemassageContext _context;
        private readonly DbSet<ViewTopSellingProduct> _dbSet;

        public ReportRepository(ServicemassageContext context, DbSet<ViewTopSellingProduct> dbSet)
        {
            _context = context;
            _dbSet = dbSet;
        }
        public async Task<List<ViewTopSellingProduct>> GetAllTopSellingProductsAsync()
        {
            return await _dbSet.ToListAsync();
        }
    }
}
