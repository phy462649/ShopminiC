using LandingPageApp.Application.Dtos;

namespace LandingPageApp.Application.Interfaces;

public interface IPersonService
{
    Task<IEnumerable<PersonDto>> GetAllAsync(CancellationToken ct = default);
    Task<PersonDto?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<PersonDetailDto?> GetDetailByIdAsync(long id, CancellationToken ct = default);
    Task<PersonDto> CreateAsync(CreatePersonDto dto, CancellationToken ct = default);
    Task<PersonDto> UpdateAsync(long id, UpdatePersonDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(long id, CancellationToken ct = default);
    Task<PersonSearchResponse> SearchAsync(PersonSearchRequest request, CancellationToken ct = default);
    Task<IEnumerable<PersonDto>> GetByRoleAsync(long roleId, CancellationToken ct = default);
    Task<int> CountAdminAsync(CancellationToken ct = default);
}
