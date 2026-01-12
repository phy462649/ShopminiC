using LandingPageApp.Application.Dtos;

namespace LandingPageApp.Application.Interfaces;

public interface IBookingServiceService
{
    Task<IEnumerable<BookingServiceItemDto>> GetAllAsync(CancellationToken ct = default);
    Task<BookingServiceItemDto?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<IEnumerable<BookingServiceItemDto>> GetByBookingIdAsync(long bookingId, CancellationToken ct = default);
    Task<BookingServiceItemDto> CreateAsync(CreateBookingServiceItemDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(long id, CancellationToken ct = default);
}
