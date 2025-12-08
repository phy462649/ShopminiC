using System.Collections.Generic;
using System.Threading.Tasks;

namespace LandingPageApp.Application.Interfaces
{
    public interface IServicesService
    {
        Task<IEnumerable<object>> GetAllAsync();
        Task<object> GetByIdAsync(int id);
        Task<object> CreateAsync(object createDto);
        Task<object> UpdateAsync(long id, object updateDto);
        Task<bool> DeleteAsync(long id);
    }
}
