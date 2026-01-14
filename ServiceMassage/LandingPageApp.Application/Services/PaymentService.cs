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
/// Service xử lý logic nghiệp vụ cho thanh toán.
/// Quản lý việc tạo, cập nhật trạng thái và xóa thanh toán.
/// Tự động cập nhật trạng thái booking/order khi thanh toán hoàn thành.
/// </summary>
public class PaymentService : IPaymentService
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
    private readonly ILogger<PaymentService> _logger;

    /// <summary>
    /// Khởi tạo PaymentService với dependency injection.
    /// </summary>
    /// <param name="uow">Unit of Work.</param>
    /// <param name="mapper">AutoMapper instance.</param>
    /// <param name="logger">Logger instance.</param>
    public PaymentService(
        IUnitOfWork uow,
        IMapper mapper,
        ILogger<PaymentService> logger)
    {
        _uow = uow;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Lấy danh sách tất cả thanh toán.
    /// Bao gồm thông tin người thanh toán, booking và order liên quan.
    /// </summary>
    /// <param name="ct">Token hủy bỏ thao tác.</param>
    /// <returns>Danh sách thanh toán.</returns>
    public async Task<IEnumerable<PaymentDto>> GetAllAsync(CancellationToken ct = default)
    {
        var payments = await _uow.payments.Query()
            .Include(p => p.Personal)
            .Include(p => p.Booking)
            .Include(p => p.Order)
            .AsNoTracking()
            .ToListAsync(ct);

        return _mapper.Map<IEnumerable<PaymentDto>>(payments);
    }

    /// <summary>
    /// Lấy thông tin thanh toán theo ID.
    /// </summary>
    /// <param name="id">ID của thanh toán.</param>
    /// <param name="ct">Token hủy bỏ thao tác.</param>
    /// <returns>Thông tin thanh toán hoặc null nếu không tìm thấy.</returns>
    public async Task<PaymentDto?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        var payment = await _uow.payments.Query()
            .Include(p => p.Personal)
            .Include(p => p.Booking)
            .Include(p => p.Order)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

        return payment is null ? null : _mapper.Map<PaymentDto>(payment);
    }

    /// <summary>
    /// Lấy danh sách thanh toán theo ID booking.
    /// </summary>
    /// <param name="bookingId">ID của booking.</param>
    /// <param name="ct">Token hủy bỏ thao tác.</param>
    /// <returns>Danh sách thanh toán của booking.</returns>
    public async Task<IEnumerable<PaymentDto>> GetByBookingIdAsync(long bookingId, CancellationToken ct = default)
    {
        var payments = await _uow.payments.Query()
            .Include(p => p.Personal)
            .Include(p => p.Booking)
            .Include(p => p.Order)
            .Where(p => p.BookingId == bookingId)
            .AsNoTracking()
            .ToListAsync(ct);

        return _mapper.Map<IEnumerable<PaymentDto>>(payments);
    }

    /// <summary>
    /// Lấy danh sách thanh toán theo ID đơn hàng.
    /// </summary>
    /// <param name="orderId">ID của đơn hàng.</param>
    /// <param name="ct">Token hủy bỏ thao tác.</param>
    /// <returns>Danh sách thanh toán của đơn hàng.</returns>
    public async Task<IEnumerable<PaymentDto>> GetByOrderIdAsync(long orderId, CancellationToken ct = default)
    {
        var payments = await _uow.payments.Query()
            .Include(p => p.Personal)
            .Include(p => p.Booking)
            .Include(p => p.Order)
            .Where(p => p.OrderId == orderId)
            .AsNoTracking()
            .ToListAsync(ct);

        return _mapper.Map<IEnumerable<PaymentDto>>(payments);
    }

    /// <summary>
    /// Tạo thanh toán mới.
    /// Tự động lấy số tiền từ booking hoặc order tương ứng.
    /// </summary>
    /// <param name="dto">Thông tin thanh toán cần tạo.</param>
    /// <param name="ct">Token hủy bỏ thao tác.</param>
    /// <returns>Thông tin thanh toán vừa tạo.</returns>
    /// <exception cref="BusinessException">Khi thiếu BookingId/OrderId hoặc booking/order đã hủy.</exception>
    /// <exception cref="NotFoundException">Khi không tìm thấy booking/order.</exception>
    public async Task<PaymentDto> CreateAsync(CreatePaymentDto dto, CancellationToken ct = default)
    {
        // Xác thực loại thanh toán và tham chiếu
        decimal amount = 0;

        // Xử lý thanh toán cho Booking
        if (dto.PaymentType == Payment_type.Booking)
        {
            if (!dto.BookingId.HasValue)
            {
                throw new BusinessException("BookingId là bắt buộc cho thanh toán booking.");
            }

            var booking = await _uow.bookings.GetByIdAsync(dto.BookingId.Value, ct)
                ?? throw new NotFoundException($"Không tìm thấy Booking với Id: {dto.BookingId}");

            // Không cho phép thanh toán booking đã hủy
            if (booking.Status == StatusBooking.Cancelled)
            {
                throw new BusinessException("Không thể thanh toán cho booking đã hủy.");
            }

            amount = booking.TotalAmount ?? 0;
        }
        // Xử lý thanh toán cho Order
        else if (dto.PaymentType == Payment_type.Order)
        {
            if (!dto.OrderId.HasValue)
            {
                throw new BusinessException("OrderId là bắt buộc cho thanh toán đơn hàng.");
            }

            var order = await _uow.orders.GetByIdAsync(dto.OrderId.Value, ct)
                ?? throw new NotFoundException($"Không tìm thấy Order với Id: {dto.OrderId}");

            // Không cho phép thanh toán đơn hàng đã hủy
            if (order.Status == OrderStatus.cancelled)
            {
                throw new BusinessException("Không thể thanh toán cho đơn hàng đã hủy.");
            }

            amount = order.TotalAmount ?? 0;
        }

        await _uow.BeginTransactionAsync(ct);

        try
        {
            var payment = new Payment
            {
                PaymentType = dto.PaymentType,
                BookingId = dto.BookingId,
                OrderId = dto.OrderId,
                PersonalId = dto.PersonalId,
                Amount = amount,
                Method = dto.Method,
                Status = PaymentStatus.Pending,
                PaymentTime = DateTime.UtcNow,
                // CreatedBy = dto.CreatedBy
            };

            await _uow.payments.AddAsync(payment, ct);
            await _uow.CommitTransactionAsync(ct);

            _logger.LogInformation("Created payment: {Id} for {Type}: {RefId}, amount: {Amount}",
                payment.Id, dto.PaymentType, dto.BookingId ?? dto.OrderId, amount);

            return await GetByIdAsync(payment.Id, ct) ?? throw new BusinessException("Lỗi khi tạo thanh toán.");
        }
        catch
        {
            await _uow.RollbackTransactionAsync(ct);
            throw;
        }
    }

    /// <summary>
    /// Cập nhật trạng thái thanh toán.
    /// Tự động cập nhật trạng thái booking/order khi thanh toán hoàn thành.
    /// </summary>
    /// <param name="id">ID của thanh toán.</param>
    /// <param name="dto">Thông tin trạng thái mới.</param>
    /// <param name="ct">Token hủy bỏ thao tác.</param>
    /// <returns>Thông tin thanh toán sau khi cập nhật.</returns>
    /// <exception cref="NotFoundException">Khi không tìm thấy thanh toán.</exception>
    public async Task<PaymentDto> UpdateStatusAsync(long id, UpdatePaymentStatusDto dto, CancellationToken ct = default)
    {
        var payment = await _uow.payments.Query()
            .Include(p => p.Booking)
            .Include(p => p.Order)
            .FirstOrDefaultAsync(p => p.Id == id, ct)
            ?? throw new NotFoundException($"Không tìm thấy Payment với Id: {id}");

        await _uow.BeginTransactionAsync(ct);

        try
        {
            payment.Status = dto.Status;
            payment.UpdatedAt = DateTime.UtcNow;
            _uow.payments.Update(payment);

            // Cập nhật trạng thái booking/order nếu thanh toán hoàn thành
            if (dto.Status == PaymentStatus.Completed)
            {
                // Cập nhật booking thành Confirmed
                if (payment.PaymentType == Payment_type.Booking && payment.Booking != null)
                {
                    payment.Booking.Status = StatusBooking.Confirmed;
                    payment.Booking.UpdatedAt = DateTime.UtcNow;
                    _uow.bookings.Update(payment.Booking);
                }
                // Cập nhật order thành Confirmed
                else if (payment.PaymentType == Payment_type.Order && payment.Order != null)
                {
                    payment.Order.Status = OrderStatus.confirmed;
                    payment.Order.UpdatedAt = DateTime.UtcNow;
                    _uow.orders.Update(payment.Order);
                }
            }

            await _uow.CommitTransactionAsync(ct);

            _logger.LogInformation("Updated payment status: {Id} to {Status}", payment.Id, dto.Status);

            return await GetByIdAsync(payment.Id, ct) ?? throw new BusinessException("Lỗi khi cập nhật thanh toán.");
        }
        catch
        {
            await _uow.RollbackTransactionAsync(ct);
            throw;
        }
    }

    /// <summary>
    /// Xóa thanh toán theo ID.
    /// Không thể xóa thanh toán đã hoàn thành.
    /// </summary>
    /// <param name="id">ID của thanh toán cần xóa.</param>
    /// <param name="ct">Token hủy bỏ thao tác.</param>
    /// <returns>True nếu xóa thành công, false nếu không tìm thấy.</returns>
    /// <exception cref="BusinessException">Khi thanh toán đã hoàn thành.</exception>
    public async Task<bool> DeleteAsync(long id, CancellationToken ct = default)
    {
        var payment = await _uow.payments.GetByIdAsync(id, ct);

        if (payment is null)
            return false;

        // Không cho phép xóa thanh toán đã hoàn thành
        if (payment.Status == PaymentStatus.Completed)
        {
            throw new BusinessException("Không thể xóa thanh toán đã hoàn thành.");
        }

        _uow.payments.Delete(payment);
        await _uow.SaveChangesAsync(ct);

        _logger.LogInformation("Deleted payment: {Id}", id);

        return true;
    }
}
