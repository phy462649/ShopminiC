using LandingPageApp.Application.Dtos;

namespace LandingPageApp.Application.Interfaces;

public interface IBookingService
{
    Task<IEnumerable<BookingDto>> GetAllAsync(CancellationToken ct = default);
    Task<BookingDto?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<IEnumerable<BookingDto>> GetByCustomerIdAsync(long customerId, CancellationToken ct = default);
    Task<IEnumerable<BookingDto>> GetByStaffIdAsync(long staffId, CancellationToken ct = default);
    Task<BookingDto> CreateAsync(CreateBookingDto dto, CancellationToken ct = default);
    Task<BookingDto> UpdateAsync(long id, UpdateBookingDto dto, CancellationToken ct = default);
    Task<BookingDto> UpdateStatusAsync(long id, UpdateBookingStatusDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(long id, CancellationToken ct = default);
    Task<bool> IsStaffAvailableAsync(long staffId, DateTime startTime, DateTime endTime, long? excludeBookingId = null, CancellationToken ct = default);
    Task<bool> IsRoomAvailableAsync(long roomId, DateTime startTime, DateTime endTime, long? excludeBookingId = null, CancellationToken ct = default);
}
