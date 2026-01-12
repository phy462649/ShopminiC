using LandingPageApp.Application.Dtos;

namespace LandingPageApp.Application.Interfaces;

public interface IStaffScheduleService
{
    Task<IEnumerable<StaffScheduleDto>> GetAllAsync(CancellationToken ct = default);
    Task<StaffScheduleDto?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<IEnumerable<StaffScheduleDto>> GetByStaffIdAsync(long staffId, CancellationToken ct = default);
    Task<StaffWeeklyScheduleDto> GetWeeklyScheduleAsync(long staffId, CancellationToken ct = default);
    Task<StaffScheduleDto> CreateAsync(CreateStaffScheduleDto dto, CancellationToken ct = default);
    Task<IEnumerable<StaffScheduleDto>> CreateBulkAsync(CreateBulkStaffScheduleDto dto, CancellationToken ct = default);
    Task<StaffScheduleDto> UpdateAsync(long id, UpdateStaffScheduleDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(long id, CancellationToken ct = default);
    Task<bool> DeleteByStaffIdAsync(long staffId, CancellationToken ct = default);
}
