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

/// <summary>
/// Service xử lý logic nghiệp vụ cho đơn hàng.
/// Quản lý việc tạo, cập nhật trạng thái và xóa đơn hàng.
/// Tự động xử lý tồn kho sản phẩm khi tạo/hủy đơn hàng.
/// </summary>
public class OrderService : IOrderService
{
    /// <summary>
    /// Unit of Work để quản lý transaction và repositories.
    /// </summary>
    private readonly IUnitOfWork _uow;

    /// <summary>
    /// AutoMapper để chuyển đổi giữa Entity và DTO.
    /// </summary>
    private readonly IMapper _mapper;

    /// <summary>
    /// Logger để ghi log hoạt động.
    /// </summary>
    private readonly ILogger<OrderService> _logger;

    /// <summary>
    /// Khởi tạo OrderService với dependency injection.
    /// </summary>
    /// <param name="uow">Unit of Work.</param>
    /// <param name="mapper">AutoMapper instance.</param>
    /// <param name="logger">Logger instance.</param>
    public OrderService(
        IUnitOfWork uow,
        IMapper mapper,
        ILogger<OrderService> logger)
    {
        _uow = uow;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Lấy danh sách tất cả đơn hàng.
    /// Bao gồm thông tin khách hàng và chi tiết sản phẩm.
    /// </summary>
    /// <param name="ct">Token hủy bỏ thao tác.</param>
    /// <returns>Danh sách đơn hàng.</returns>
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

    /// <summary>
    /// Lấy thông tin đơn hàng theo ID.
    /// Bao gồm thông tin khách hàng và chi tiết sản phẩm.
    /// </summary>
    /// <param name="id">ID của đơn hàng.</param>
    /// <param name="ct">Token hủy bỏ thao tác.</param>
    /// <returns>Thông tin đơn hàng hoặc null nếu không tìm thấy.</returns>
    public async Task<OrderDto?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        var order = await _uow.orders.Query()
            .Include(o => o.Customer)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.Id == id, ct);

        return order is null ? null : _mapper.Map<OrderDto>(order);
    }

    /// <summary>
    /// Lấy danh sách đơn hàng theo ID khách hàng.
    /// </summary>
    /// <param name="customerId">ID của khách hàng.</param>
    /// <param name="ct">Token hủy bỏ thao tác.</param>
    /// <returns>Danh sách đơn hàng của khách hàng.</returns>
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

    /// <summary>
    /// Tạo đơn hàng mới.
    /// Tự động kiểm tra và trừ tồn kho sản phẩm.
    /// Có thể tạo thanh toán kèm theo nếu được yêu cầu.
    /// </summary>
    /// <param name="dto">Thông tin đơn hàng cần tạo.</param>
    /// <param name="ct">Token hủy bỏ thao tác.</param>
    /// <returns>Thông tin đơn hàng vừa tạo.</returns>
    /// <exception cref="BusinessException">Khi đơn hàng không có sản phẩm hoặc không đủ tồn kho.</exception>
    public async Task<OrderDto> CreateAsync(CreateOrderDto dto, CancellationToken ct = default)
    {
        // Kiểm tra đơn hàng phải có ít nhất một sản phẩm
        if (!dto.Items.Any())
        {
            throw new BusinessException("Đơn hàng phải có ít nhất một sản phẩm.");
        }

        // Lấy danh sách sản phẩm và kiểm tra tồn kho
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

    /// <summary>
    /// Cập nhật trạng thái đơn hàng.
    /// Tự động hoàn lại tồn kho nếu hủy đơn hàng.
    /// </summary>
    /// <param name="id">ID của đơn hàng.</param>
    /// <param name="dto">Thông tin trạng thái mới.</param>
    /// <param name="ct">Token hủy bỏ thao tác.</param>
    /// <returns>Thông tin đơn hàng sau khi cập nhật.</returns>
    /// <exception cref="NotFoundException">Khi không tìm thấy đơn hàng.</exception>
    public async Task<OrderDto> UpdateStatusAsync(long id, UpdateOrderStatusDto dto, CancellationToken ct = default)
    {
        var order = await _uow.orders.GetByIdAsync(id, ct)
            ?? throw new NotFoundException($"Không tìm thấy Order với Id: {id}");

        // Nếu hủy đơn hàng, hoàn lại tồn kho
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

    /// <summary>
    /// Xóa đơn hàng theo ID.
    /// Không thể xóa đơn hàng đã hoàn thành hoặc đang giao.
    /// </summary>
    /// <param name="id">ID của đơn hàng cần xóa.</param>
    /// <param name="ct">Token hủy bỏ thao tác.</param>
    /// <returns>True nếu xóa thành công, false nếu không tìm thấy.</returns>
    /// <exception cref="BusinessException">Khi đơn hàng đã hoàn thành hoặc đang giao.</exception>
    public async Task<bool> DeleteAsync(long id, CancellationToken ct = default)
    {
        var order = await _uow.orders.GetByIdAsync(id, ct);

        if (order is null)
            return false;

        // Không cho phép xóa đơn hàng đã hoàn thành hoặc đang giao
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
