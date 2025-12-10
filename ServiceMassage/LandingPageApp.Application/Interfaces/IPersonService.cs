using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LandingPageApp.Application.Interfaces
{
    public interface IPersonService
    {
        Task<IEnumerable<object>> GetAllAsync();
        Task<object> GetByIdAsync(int id);
        Task<object> CreateAsync(object createDto);
        Task<object> UpdateAsync(long id, object updateDto);
        Task<bool> DeleteAsync(long id);
    }
}
