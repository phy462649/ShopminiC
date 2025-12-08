
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

        public ReportRepository(ServicemassageContext context)
        {
            _context = context;
            _dbSet = context.Set<ViewTopSellingProduct>();
        }
        public async Task<List<ViewTopSellingProduct>> GetAllTopSellingProductsAsync()
        {
            return await _dbSet.ToListAsync();
        }
    }
}
