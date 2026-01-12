using AutoMapper;
using LandingPageApp.Application.Dtos;
using LandingPageApp.Application.Exceptions;
using LandingPageApp.Application.Interfaces;
using LandingPageApp.Domain.Entities;
using LandingPageApp.Domain.Enums;
using LandingPageApp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LandingPageApp.Application.Services;

public class OrderService : IOrderService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ILogger<OrderService> _logger;

    public OrderService(
        IUnitOfWork uow,
        IMapper mapper,
        ILogger<OrderService> logger)
    {
        _uow = uow;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<OrderDto>> GetAllAsync(CancellationToken ct = default)
    {
        var orders = await _uow.orders.Query()
            .Include(o => o.Customer)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .AsNoTracking()
            .ToListAsync(ct);

        return _mapper.Map<IEnumerable<OrderDto>>(orders);
    }

    public async Task<OrderDto?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        var order = await _uow.orders.Query()
            .Include(o => o.Customer)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.Id == id, ct);

        return order is null ? null : _mapper.Map<OrderDto>(order);
    }

    public async Task<IEnumerable<OrderDto>> GetByCustomerIdAsync(long customerId, CancellationToken ct = default)
    {
        var orders = await _uow.orders.Query()
            .Include(o => o.Customer)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .Where(o => o.CustomerId == customerId)
            .AsNoTracking()
            .ToListAsync(ct);

        return _mapper.Map<IEnumerable<OrderDto>>(orders);
    }

    public async Task<OrderDto> CreateAsync(CreateOrderDto dto, CancellationToken ct = default)
    {
        if (!dto.Items.Any())
        {
            throw new BusinessException("Đơn hàng phải có ít nhất một sản phẩm.");
        }

        // Get products and validate stock
        var productIds = dto.Items.Select(i => i.ProductId).ToList();
        var products = await _uow.products.Query()
            .Where(p => productIds.Contains(p.Id))
            .ToListAsync(ct);

        if (products.Count != productIds.Count)
        {
            throw new BusinessException("Một hoặc nhiều sản phẩm không tồn tại.");
        }

        // Check stock availability
        foreach (var item in dto.Items)
        {
            var product = products.First(p => p.Id == item.ProductId);
            if (product.Stock < item.Quantity)
            {
                throw new BusinessException($"Sản phẩm '{product.Name}' không đủ số lượng trong kho. Còn lại: {product.Stock}");
            }
        }

        await _uow.BeginTransactionAsync(ct);

        try
        {
            // Create order
            var order = new Order
            {
                CustomerId = dto.CustomerId,
                OrderTime = DateTime.UtcNow,
                Status = OrderStatus.pending,
                CreatedAt = DateTime.UtcNow
            };

            await _uow.orders.AddAsync(order, ct);
            await _uow.SaveChangesAsync(ct);

            // Create order items and update stock
            decimal totalAmount = 0;
            foreach (var itemDto in dto.Items)
            {
                var product = products.First(p => p.Id == itemDto.ProductId);

                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = itemDto.ProductId,
                    Quantity = itemDto.Quantity,
                    Price = product.Price
                };

                totalAmount += product.Price * itemDto.Quantity;
                await _uow.orderItem.AddAsync(orderItem, ct);

                // Update product stock
                product.Stock -= itemDto.Quantity;
                product.UpdatedAt = DateTime.UtcNow;
                _uow.products.Update(product);
            }

            // Update total amount
            order.TotalAmount = totalAmount;
            _uow.orders.Update(order);

            // Create payment if requested
            if (dto.CreatePayment && dto.PaymentMethod.HasValue)
            {
                var payment = new Payment
                {
                    PaymentType = Payment_type.Order,
                    OrderId = order.Id,
                    PersonalId = dto.CustomerId,
                    Amount = totalAmount,
                    Method = dto.PaymentMethod,
                    Status = PaymentStatus.Pending,
                    PaymentTime = DateTime.UtcNow
                };

                await _uow.payments.AddAsync(payment, ct);
            }

            await _uow.CommitTransactionAsync(ct);

            _logger.LogInformation("Created order: {Id} for customer: {CustomerId}, total: {Total}", 
                order.Id, dto.CustomerId, totalAmount);

            return await GetByIdAsync(order.Id, ct) ?? throw new BusinessException("Lỗi khi tạo đơn hàng.");
        }
        catch
        {
            await _uow.RollbackTransactionAsync(ct);
            throw;
        }
    }

    public async Task<OrderDto> UpdateStatusAsync(long id, UpdateOrderStatusDto dto, CancellationToken ct = default)
    {
        var order = await _uow.orders.GetByIdAsync(id, ct)
            ?? throw new NotFoundException($"Không tìm thấy Order với Id: {id}");

        // If cancelling, restore stock
        if (dto.Status == OrderStatus.cancelled && order.Status != OrderStatus.cancelled)
        {
            await _uow.BeginTransactionAsync(ct);

            try
            {
                var orderItems = await _uow.orderItem.Query()
                    .Where(oi => oi.OrderId == id)
                    .ToListAsync(ct);

                foreach (var item in orderItems)
                {
                    var product = await _uow.products.GetByIdAsync(item.ProductId, ct);
                    if (product != null)
                    {
                        product.Stock += item.Quantity;
                        product.UpdatedAt = DateTime.UtcNow;
                        _uow.products.Update(product);
                    }
                }

                order.Status = dto.Status;
                order.UpdatedAt = DateTime.UtcNow;
                _uow.orders.Update(order);

                await _uow.CommitTransactionAsync(ct);
            }
            catch
            {
                await _uow.RollbackTransactionAsync(ct);
                throw;
            }
        }
        else
        {
            order.Status = dto.Status;
            order.UpdatedAt = DateTime.UtcNow;
            _uow.orders.Update(order);
            await _uow.SaveChangesAsync(ct);
        }

        _logger.LogInformation("Updated order status: {Id} to {Status}", order.Id, dto.Status);

        return await GetByIdAsync(order.Id, ct) ?? throw new BusinessException("Lỗi khi cập nhật đơn hàng.");
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken ct = default)
    {
        var order = await _uow.orders.GetByIdAsync(id, ct);

        if (order is null)
            return false;

        if (order.Status == OrderStatus.completed || order.Status == OrderStatus.shipped)
        {
            throw new BusinessException("Không thể xóa đơn hàng đã hoàn thành hoặc đang giao.");
        }

        _uow.orders.Delete(order);
        await _uow.SaveChangesAsync(ct);

        _logger.LogInformation("Deleted order: {Id}", id);

        return true;
    }
}
