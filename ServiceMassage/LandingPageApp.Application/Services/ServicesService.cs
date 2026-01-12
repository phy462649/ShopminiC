using AutoMapper;
using LandingPageApp.Application.Dtos;
using LandingPageApp.Application.Exceptions;
using LandingPageApp.Application.Interfaces;
using LandingPageApp.Domain.Entities;
using LandingPageApp.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace LandingPageApp.Application.Services;

public class ServicesService : IServicesService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<ServicesService> _logger;

    public ServicesService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<ServicesService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<ServiceDto>> GetAllAsync(CancellationToken ct = default)
    {
        var services = await _unitOfWork.services.GetAllAsync(ct);
        return _mapper.Map<IEnumerable<ServiceDto>>(services);
    }

    public async Task<ServiceDto?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        var service = await _unitOfWork.services.GetByIdAsync(id, ct);
        return service is null ? null : _mapper.Map<ServiceDto>(service);
    }

    public async Task<ServiceDto> CreateAsync(CreateServiceDto dto, CancellationToken ct = default)
    {
        // Check duplicate name
        var existingService = await _unitOfWork.services
            .FindAsync(s => s.Name == dto.Name, ct);
        
        if (existingService.Any())
        {
            throw new BusinessException($"Dịch vụ với tên '{dto.Name}' đã tồn tại.");
        }

        if (dto.DurationMinutes <= 0)
        {
            throw new BusinessException("Thời gian dịch vụ phải lớn hơn 0 phút.");
        }

        if (dto.Price < 0)
        {
            throw new BusinessException("Giá dịch vụ không được âm.");
        }

        var service = _mapper.Map<Service>(dto);
        service.CreatedAt = DateTime.UtcNow;

        await _unitOfWork.services.AddAsync(service, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Created service: {Name} with Id: {Id}", service.Name, service.Id);

        return _mapper.Map<ServiceDto>(service);
    }

    public async Task<ServiceDto> UpdateAsync(long id, UpdateServiceDto dto, CancellationToken ct = default)
    {
        var service = await _unitOfWork.services.GetByIdAsync(id, ct)
            ?? throw new NotFoundException($"Không tìm thấy dịch vụ với Id: {id}");

        // Check duplicate name (exclude current service)
        var existingService = await _unitOfWork.services
            .FindAsync(s => s.Name == dto.Name && s.Id != id, ct);
        
        if (existingService.Any())
        {
            throw new BusinessException($"Dịch vụ với tên '{dto.Name}' đã tồn tại.");
        }

        if (dto.DurationMinutes <= 0)
        {
            throw new BusinessException("Thời gian dịch vụ phải lớn hơn 0 phút.");
        }

        if (dto.Price < 0)
        {
            throw new BusinessException("Giá dịch vụ không được âm.");
        }

        _mapper.Map(dto, service);
        service.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.services.Update(service);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Updated service: {Id}", service.Id);

        return _mapper.Map<ServiceDto>(service);
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken ct = default)
    {
        var service = await _unitOfWork.services.GetByIdAsync(id, ct);

        if (service is null)
            return false;

        // Check if service has any booking services
        var hasBookingServices = await _unitOfWork.bookingservices
            .ExistsAsync(bs => bs.ServiceId == id, ct);

        if (hasBookingServices)
        {
            throw new BusinessException($"Không thể xóa dịch vụ '{service.Name}' vì đang có booking liên quan.");
        }

        _unitOfWork.services.Delete(service);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Deleted service: {Id} - {Name}", service.Id, service.Name);

        return true;
    }
}
