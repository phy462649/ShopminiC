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

public class PaymentService : IPaymentService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ILogger<PaymentService> _logger;

    public PaymentService(
        IUnitOfWork uow,
        IMapper mapper,
        ILogger<PaymentService> logger)
    {
        _uow = uow;
        _mapper = mapper;
        _logger = logger;
    }

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

    public async Task<PaymentDto?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        var payment = await _uow.payments.Query()
            .Include(p => p.Personal)
            .Include(p => p.Booking)
            .Include(p => p.Order)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

        return payment is null ? null : _mapper.Map<PaymentDto>(payment);
    }

    public async Task<IEnumerable<PaymentDto>> GetByBookingIdAsync(long bookingId, CancellationToken ct = default)
    {
        var payments = await _uow.payments.Query()
            .Include(p => p.Personal)
            .Where(p => p.BookingId == bookingId)
            .AsNoTracking()
            .ToListAsync(ct);

        return _mapper.Map<IEnumerable<PaymentDto>>(payments);
    }

    public async Task<IEnumerable<PaymentDto>> GetByOrderIdAsync(long orderId, CancellationToken ct = default)
    {
        var payments = await _uow.payments.Query()
            .Include(p => p.Personal)
            .Where(p => p.OrderId == orderId)
            .AsNoTracking()
            .ToListAsync(ct);

        return _mapper.Map<IEnumerable<PaymentDto>>(payments);
    }

    public async Task<PaymentDto> CreateAsync(CreatePaymentDto dto, CancellationToken ct = default)
    {
        // Validate payment type and reference
        decimal amount = 0;

        if (dto.PaymentType == Payment_type.Booking)
        {
            if (!dto.BookingId.HasValue)
            {
                throw new BusinessException("BookingId là bắt buộc cho thanh toán booking.");
            }

            var booking = await _uow.bookings.GetByIdAsync(dto.BookingId.Value, ct)
                ?? throw new NotFoundException($"Không tìm thấy Booking với Id: {dto.BookingId}");

            if (booking.Status == StatusBooking.Cancelled)
            {
                throw new BusinessException("Không thể thanh toán cho booking đã hủy.");
            }

            amount = booking.TotalAmount ?? 0;
        }
        else if (dto.PaymentType == Payment_type.Order)
        {
            if (!dto.OrderId.HasValue)
            {
                throw new BusinessException("OrderId là bắt buộc cho thanh toán đơn hàng.");
            }

            var order = await _uow.orders.GetByIdAsync(dto.OrderId.Value, ct)
                ?? throw new NotFoundException($"Không tìm thấy Order với Id: {dto.OrderId}");

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
                CreatedBy = dto.CreatedBy
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

            // Update booking/order status if payment completed
            if (dto.Status == PaymentStatus.Completed)
            {
                if (payment.PaymentType == Payment_type.Booking && payment.Booking != null)
                {
                    payment.Booking.Status = StatusBooking.Confirmed;
                    payment.Booking.UpdatedAt = DateTime.UtcNow;
                    _uow.bookings.Update(payment.Booking);
                }
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

    public async Task<bool> DeleteAsync(long id, CancellationToken ct = default)
    {
        var payment = await _uow.payments.GetByIdAsync(id, ct);

        if (payment is null)
            return false;

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
