using LandingPageApp.Domain.Entities;

namespace LandingPageApp.Domain.Repositories
{
    public interface IReportRepository
    {
        public Task<List<ViewTopSellingProduct>> GetAllTopSellingProductsAsync();
    }
}
