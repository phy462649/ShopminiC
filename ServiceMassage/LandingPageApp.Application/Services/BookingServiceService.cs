using AutoMapper;
using LandingPageApp.Application.Dtos;
using LandingPageApp.Application.Exceptions;
using LandingPageApp.Application.Interfaces;
using LandingPageApp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using BookingServiceEntity = LandingPageApp.Domain.Entities.BookingService;

namespace LandingPageApp.Application.Services;

public class BookingServiceService : IBookingServiceService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<BookingServiceService> _logger;

    public BookingServiceService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<BookingServiceService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<BookingServiceItemDto>> GetAllAsync(CancellationToken ct = default)
    {
        var items = await _unitOfWork.bookingservices.Query()
            .Include(bs => bs.Service)
            .ToListAsync(ct);
        return _mapper.Map<IEnumerable<BookingServiceItemDto>>(items);
    }

    public async Task<BookingServiceItemDto?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        var item = await _unitOfWork.bookingservices.Query()
            .Include(bs => bs.Service)
            .FirstOrDefaultAsync(bs => bs.Id == id, ct);
        return item is null ? null : _mapper.Map<BookingServiceItemDto>(item);
    }

    public async Task<IEnumerable<BookingServiceItemDto>> GetByBookingIdAsync(long bookingId, CancellationToken ct = default)
    {
        var items = await _unitOfWork.bookingservices.Query()
            .Include(bs => bs.Service)
            .Where(bs => bs.BookingId == bookingId)
            .ToListAsync(ct);
        return _mapper.Map<IEnumerable<BookingServiceItemDto>>(items);
    }

    public async Task<BookingServiceItemDto> CreateAsync(CreateBookingServiceItemDto dto, CancellationToken ct = default)
    {
        var service = await _unitOfWork.services.GetByIdAsync(dto.ServiceId, ct)
            ?? throw new NotFoundException($"Không tìm thấy dịch vụ với Id: {dto.ServiceId}");

        var bookingService = new BookingServiceEntity
        {
            ServiceId = dto.ServiceId,
            Quantity = dto.Quantity,
            Price = service.Price
        };

        await _unitOfWork.bookingservices.AddAsync(bookingService, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Created booking service for service {ServiceId}", dto.ServiceId);

        return await GetByIdAsync(bookingService.Id, ct) ?? _mapper.Map<BookingServiceItemDto>(bookingService);
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken ct = default)
    {
        var bookingService = await _unitOfWork.bookingservices.GetByIdAsync(id, ct);

        if (bookingService is null)
            return false;

        _unitOfWork.bookingservices.Delete(bookingService);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Deleted booking service {Id}", id);

        return true;
    }
}
