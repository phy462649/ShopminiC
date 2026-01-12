using LandingPageApp.Application.Dtos;

namespace LandingPageApp.Application.Interfaces;

public interface IServicesService
{
    Task<IEnumerable<ServiceDto>> GetAllAsync(CancellationToken ct = default);
    Task<ServiceDto?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<ServiceDto> CreateAsync(CreateServiceDto dto, CancellationToken ct = default);
    Task<ServiceDto> UpdateAsync(long id, UpdateServiceDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(long id, CancellationToken ct = default);
}
