using AutoMapper;
using LandingPageApp.Application.Dtos;
using LandingPageApp.Application.Exceptions;
using LandingPageApp.Application.Interfaces;
using LandingPageApp.Domain.Entities;
using LandingPageApp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LandingPageApp.Application.Services;

public class OrderItemService : IOrderItemService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<OrderItemService> _logger;

    public OrderItemService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<OrderItemService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<OrderItemDto>> GetAllAsync(CancellationToken ct = default)
    {
        var items = await _unitOfWork.orderItem.Query()
            .Include(oi => oi.Product)
            .ToListAsync(ct);
        return _mapper.Map<IEnumerable<OrderItemDto>>(items);
    }

    public async Task<OrderItemDto?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        var item = await _unitOfWork.orderItem.Query()
            .Include(oi => oi.Product)
            .FirstOrDefaultAsync(oi => oi.Id == id, ct);
        return item is null ? null : _mapper.Map<OrderItemDto>(item);
    }

    public async Task<IEnumerable<OrderItemDto>> GetByOrderIdAsync(long orderId, CancellationToken ct = default)
    {
        var items = await _unitOfWork.orderItem.Query()
            .Include(oi => oi.Product)
            .Where(oi => oi.OrderId == orderId)
            .ToListAsync(ct);
        return _mapper.Map<IEnumerable<OrderItemDto>>(items);
    }

    public async Task<OrderItemDto> CreateAsync(CreateOrderItemDto dto, CancellationToken ct = default)
    {
        var product = await _unitOfWork.products.GetByIdAsync(dto.ProductId, ct)
            ?? throw new NotFoundException($"Không tìm thấy sản phẩm với Id: {dto.ProductId}");

        if ((product.Stock ?? 0) < dto.Quantity)
        {
            throw new BusinessException($"Không đủ tồn kho. Hiện có: {product.Stock ?? 0}, yêu cầu: {dto.Quantity}");
        }

        var orderItem = new OrderItem
        {
            ProductId = dto.ProductId,
            Quantity = dto.Quantity,
            Price = product.Price
        };

        await _unitOfWork.orderItem.AddAsync(orderItem, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Created order item for product {ProductId}", dto.ProductId);

        return await GetByIdAsync(orderItem.Id, ct) ?? _mapper.Map<OrderItemDto>(orderItem);
    }

    public async Task<OrderItemDto> UpdateAsync(long id, UpdateOrderItemDto dto, CancellationToken ct = default)
    {
        var orderItem = await _unitOfWork.orderItem.Query()
            .Include(oi => oi.Product)
            .FirstOrDefaultAsync(oi => oi.Id == id, ct)
            ?? throw new NotFoundException($"Không tìm thấy order item với Id: {id}");

        var quantityDiff = dto.Quantity - orderItem.Quantity;
        
        if (quantityDiff > 0 && (orderItem.Product?.Stock ?? 0) < quantityDiff)
        {
            throw new BusinessException($"Không đủ tồn kho để tăng số lượng.");
        }

        orderItem.Quantity = dto.Quantity;

        _unitOfWork.orderItem.Update(orderItem);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Updated order item {Id}", id);

        return _mapper.Map<OrderItemDto>(orderItem);
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken ct = default)
    {
        var orderItem = await _unitOfWork.orderItem.GetByIdAsync(id, ct);

        if (orderItem is null)
            return false;

        _unitOfWork.orderItem.Delete(orderItem);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Deleted order item {Id}", id);

        return true;
    }
}
