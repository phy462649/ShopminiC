using LandingPageApp.Application.Dtos;

namespace LandingPageApp.Application.Interfaces;

public interface IPaymentService
{
    Task<IEnumerable<PaymentDto>> GetAllAsync(CancellationToken ct = default);
    Task<PaymentDto?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<IEnumerable<PaymentDto>> GetByBookingIdAsync(long bookingId, CancellationToken ct = default);
    Task<IEnumerable<PaymentDto>> GetByOrderIdAsync(long orderId, CancellationToken ct = default);
    Task<PaymentDto> CreateAsync(CreatePaymentDto dto, CancellationToken ct = default);
    Task<PaymentDto> UpdateStatusAsync(long id, UpdatePaymentStatusDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(long id, CancellationToken ct = default);
}
