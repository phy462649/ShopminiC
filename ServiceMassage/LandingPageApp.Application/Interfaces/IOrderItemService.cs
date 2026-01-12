using LandingPageApp.Application.Dtos;

namespace LandingPageApp.Application.Interfaces;

public interface IOrderItemService
{
    Task<IEnumerable<OrderItemDto>> GetAllAsync(CancellationToken ct = default);
    Task<OrderItemDto?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<IEnumerable<OrderItemDto>> GetByOrderIdAsync(long orderId, CancellationToken ct = default);
    Task<OrderItemDto> CreateAsync(CreateOrderItemDto dto, CancellationToken ct = default);
    Task<OrderItemDto> UpdateAsync(long id, UpdateOrderItemDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(long id, CancellationToken ct = default);
}
