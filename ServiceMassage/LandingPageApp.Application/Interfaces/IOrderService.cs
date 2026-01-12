using LandingPageApp.Application.Dtos;

namespace LandingPageApp.Application.Interfaces;

public interface IOrderService
{
    Task<IEnumerable<OrderDto>> GetAllAsync(CancellationToken ct = default);
    Task<OrderDto?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<IEnumerable<OrderDto>> GetByCustomerIdAsync(long customerId, CancellationToken ct = default);
    Task<OrderDto> CreateAsync(CreateOrderDto dto, CancellationToken ct = default);
    Task<OrderDto> UpdateStatusAsync(long id, UpdateOrderStatusDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(long id, CancellationToken ct = default);
}
