using LandingPageApp.Application.Dtos;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace LandingPageApp.Application.Interfaces
{
    public interface IStaffService
    {
        Task<IEnumerable<StaffDTO>> GetAllAsync();
        Task<StaffDTO> GetByIdAsync(int id);
        Task<StaffDTO> CreateAsync(StaffDTO createDto);
        Task<StaffDTO> UpdateAsync(long id, StaffDTO updateDto);
        Task<bool> DeleteAsync(long id);
    }
}
