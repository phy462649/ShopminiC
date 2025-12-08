using LandingPageApp.Application.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LandingPageApp.Application.Interfaces
{
    public interface ICustomerService
    {
        Task<IEnumerable<CustomerDTO>> GetAllAsync();
        Task<CustomerDTO> GetByIdAsync(int id);
        Task<CustomerDTO> CreateAsync(CustomerDTO createDto);
        Task<CustomerDTO> UpdateAsync(long id, CustomerDTO updateDto);
        Task<bool> DeleteAsync(long id);
    }
}
