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

public class BookingService : IBookingService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ILogger<BookingService> _logger;

    public BookingService(IUnitOfWork uow, IMapper mapper, ILogger<BookingService> logger)
    {
        _uow = uow;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<BookingDto>> GetAllAsync(CancellationToken ct = default)
    {
        var bookings = await _uow.bookings.Query()
            .Include(b => b.Customer).Include(b => b.Staff).Include(b => b.Room)
            .Include(b => b.BookingServices).ThenInclude(bs => bs.Service)
            .AsNoTracking().ToListAsync(ct);
        return _mapper.Map<IEnumerable<BookingDto>>(bookings);
    }

    public async Task<BookingDto?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        var booking = await _uow.bookings.Query()
            .Include(b => b.Customer).Include(b => b.Staff).Include(b => b.Room)
            .Include(b => b.BookingServices).ThenInclude(bs => bs.Service)
            .FirstOrDefaultAsync(b => b.Id == id, ct);
        return booking is null ? null : _mapper.Map<BookingDto>(booking);
    }

    public async Task<IEnumerable<BookingDto>> GetByCustomerIdAsync(long customerId, CancellationToken ct = default)
    {
        var bookings = await _uow.bookings.Query()
            .Include(b => b.Customer).Include(b => b.Staff).Include(b => b.Room)
            .Include(b => b.BookingServices).ThenInclude(bs => bs.Service)
            .Where(b => b.CustomerId == customerId).AsNoTracking().ToListAsync(ct);
        return _mapper.Map<IEnumerable<BookingDto>>(bookings);
    }

    public async Task<IEnumerable<BookingDto>> GetByStaffIdAsync(long staffId, CancellationToken ct = default)
    {
        var bookings = await _uow.bookings.Query()
            .Include(b => b.Customer).Include(b => b.Staff).Include(b => b.Room)
            .Include(b => b.BookingServices).ThenInclude(bs => bs.Service)
            .Where(b => b.StaffId == staffId).AsNoTracking().ToListAsync(ct);
        return _mapper.Map<IEnumerable<BookingDto>>(bookings);
    }

    public async Task<BookingDto> CreateAsync(CreateBookingDto dto, CancellationToken ct = default)
    {
        if (!await IsStaffAvailableAsync(dto.StaffId, dto.StartTime, dto.EndTime, null, ct))
            throw new BusinessException("Nhân viên không khả dụng trong khoảng thời gian này.");

        if (!await IsRoomAvailableAsync(dto.RoomId, dto.StartTime, dto.EndTime, null, ct))
            throw new BusinessException("Phòng không khả dụng trong khoảng thời gian này.");

        var serviceIds = dto.Services.Select(s => s.ServiceId).ToList();
        var services = await _uow.services.Query().Where(s => serviceIds.Contains(s.Id)).ToListAsync(ct);

        if (services.Count != serviceIds.Count)
            throw new BusinessException("Một hoặc nhiều dịch vụ không tồn tại.");

        var customer = await _uow.bookings.Query().Include(b => b.Customer)
            .Select(b => b.Customer).FirstOrDefaultAsync(c => c.Id == dto.CustomerId, ct);

        await _uow.BeginTransactionAsync(ct);
        try
        {
            var booking = new Booking
            {
                CustomerId = dto.CustomerId, StaffId = dto.StaffId, RoomId = dto.RoomId,
                StartTime = dto.StartTime, EndTime = dto.EndTime,
                Status = StatusBooking.Pending, CreatedAt = DateTime.UtcNow
            };

            await _uow.bookings.AddAsync(booking, ct);
            await _uow.SaveChangesAsync(ct);

            decimal totalAmount = 0;
            foreach (var serviceDto in dto.Services)
            {
                var service = services.First(s => s.Id == serviceDto.ServiceId);
                var bookingService = new Domain.Entities.BookingService
                {
                    BookingId = booking.Id, ServiceId = serviceDto.ServiceId,
                    Price = service.Price, Quantity = serviceDto.Quantity
                };
                totalAmount += service.Price * serviceDto.Quantity;
                await _uow.bookingservices.AddAsync(bookingService, ct);
            }

            booking.TotalAmount = totalAmount;
            booking.RaiseBookingCreatedEvent(customer?.Email, customer?.Phone);
            _uow.bookings.Update(booking);

            if (dto.CreatePayment && dto.PaymentMethod.HasValue)
            {
                var payment = new Payment
                {
                    PaymentType = Payment_type.Booking, BookingId = booking.Id,
                    PersonalId = dto.CustomerId, Amount = totalAmount,
                    Method = dto.PaymentMethod, Status = PaymentStatus.Pending,
                    PaymentTime = DateTime.UtcNow
                };
                await _uow.payments.AddAsync(payment, ct);
            }

            await _uow.CommitTransactionAsync(ct);
            _logger.LogInformation("Created booking: {Id} for customer: {CustomerId}", booking.Id, dto.CustomerId);
            return await GetByIdAsync(booking.Id, ct) ?? throw new BusinessException("Lỗi khi tạo booking.");
        }
        catch
        {
            await _uow.RollbackTransactionAsync(ct);
            throw;
        }
    }

    public async Task<BookingDto> UpdateAsync(long id, UpdateBookingDto dto, CancellationToken ct = default)
    {
        var booking = await _uow.bookings.GetByIdAsync(id, ct)
            ?? throw new NotFoundException($"Không tìm thấy Booking với Id: {id}");

        if (!await IsStaffAvailableAsync(dto.StaffId, dto.StartTime, dto.EndTime, id, ct))
            throw new BusinessException("Nhân viên không khả dụng trong khoảng thời gian này.");

        if (!await IsRoomAvailableAsync(dto.RoomId, dto.StartTime, dto.EndTime, id, ct))
            throw new BusinessException("Phòng không khả dụng trong khoảng thời gian này.");

        var oldStatus = booking.Status;
        booking.StaffId = dto.StaffId;
        booking.RoomId = dto.RoomId;
        booking.StartTime = dto.StartTime;
        booking.EndTime = dto.EndTime;
        booking.Status = dto.Status;
        booking.UpdatedAt = DateTime.UtcNow;

        await RaiseStatusChangeEvents(booking, oldStatus, dto.Status, ct);
        _uow.bookings.Update(booking);
        await _uow.SaveChangesAsync(ct);

        _logger.LogInformation("Updated booking: {Id}", booking.Id);
        return await GetByIdAsync(booking.Id, ct) ?? throw new BusinessException("Lỗi khi cập nhật booking.");
    }

    public async Task<BookingDto> UpdateStatusAsync(long id, UpdateBookingStatusDto dto, CancellationToken ct = default)
    {
        var booking = await _uow.bookings.Query().Include(b => b.Customer)
            .FirstOrDefaultAsync(b => b.Id == id, ct)
            ?? throw new NotFoundException($"Không tìm thấy Booking với Id: {id}");

        var oldStatus = booking.Status;
        booking.Status = dto.Status;
        booking.UpdatedAt = DateTime.UtcNow;

        await RaiseStatusChangeEvents(booking, oldStatus, dto.Status, ct);
        _uow.bookings.Update(booking);
        await _uow.SaveChangesAsync(ct);

        _logger.LogInformation("Updated booking status: {Id} to {Status}", booking.Id, dto.Status);
        return await GetByIdAsync(booking.Id, ct) ?? throw new BusinessException("Lỗi khi cập nhật booking.");
    }

    private async Task RaiseStatusChangeEvents(Booking booking, StatusBooking? oldStatus, StatusBooking? newStatus, CancellationToken ct)
    {
        if (oldStatus == newStatus) return;

        string? customerEmail = booking.Customer?.Email;
        if (customerEmail == null)
        {
            var customer = await _uow.bookings.Query().Where(b => b.Id == booking.Id)
                .Select(b => b.Customer).FirstOrDefaultAsync(ct);
            customerEmail = customer?.Email;
        }

        switch (newStatus)
        {
            case StatusBooking.Confirmed:
                booking.RaiseBookingConfirmedEvent(customerEmail);
                break;
            case StatusBooking.Cancelled:
                booking.RaiseBookingCancelledEvent(customerEmail);
                break;
            case StatusBooking.Completed:
                booking.RaiseBookingCompletedEvent();
                break;
        }
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken ct = default)
    {
        var booking = await _uow.bookings.Query().Include(b => b.Customer)
            .FirstOrDefaultAsync(b => b.Id == id, ct);

        if (booking is null) return false;

        if (booking.Status == StatusBooking.Completed)
            throw new BusinessException("Không thể xóa booking đã hoàn thành.");

        booking.RaiseBookingCancelledEvent(booking.Customer?.Email, "Booking đã bị xóa");
        _uow.bookings.Delete(booking);
        await _uow.SaveChangesAsync(ct);

        _logger.LogInformation("Deleted booking: {Id}", id);
        return true;
    }

    public async Task<bool> IsStaffAvailableAsync(long staffId, DateTime startTime, DateTime endTime, long? excludeBookingId = null, CancellationToken ct = default)
    {
        var query = _uow.bookings.Query()
            .Where(b => b.StaffId == staffId)
            .Where(b => b.Status != StatusBooking.Cancelled)
            .Where(b => b.StartTime < endTime && b.EndTime > startTime);

        if (excludeBookingId.HasValue)
            query = query.Where(b => b.Id != excludeBookingId.Value);

        return !await query.AnyAsync(ct);
    }

    public async Task<bool> IsRoomAvailableAsync(long roomId, DateTime startTime, DateTime endTime, long? excludeBookingId = null, CancellationToken ct = default)
    {
        var query = _uow.bookings.Query()
            .Where(b => b.RoomId == roomId)
            .Where(b => b.Status != StatusBooking.Cancelled)
            .Where(b => b.StartTime < endTime && b.EndTime > startTime);

        if (excludeBookingId.HasValue)
            query = query.Where(b => b.Id != excludeBookingId.Value);

        return !await query.AnyAsync(ct);
    }
}
