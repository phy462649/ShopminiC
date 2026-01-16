using AutoMapper;
using LandingPageApp.Application.Dtos;
using LandingPageApp.Application.Exceptions;
using LandingPageApp.Application.Interfaces;
using LandingPageApp.Domain.Entities;
using LandingPageApp.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace LandingPageApp.Application.Services;

/// <summary>
/// Service xử lý logic nghiệp vụ cho dịch vụ massage/spa.
/// Sử dụng Redis cache để tối ưu hiệu suất.
/// </summary>
public class ServicesService : IServicesService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<ServicesService> _logger;
    private readonly ICacheRediservice _cache;

    // Cache keys
    private const string AllServicesCacheKey = "services:all";
    private const string ServiceByIdCacheKey = "services:id:{0}";
    private static readonly TimeSpan CacheExpiry = TimeSpan.FromMinutes(10);

    public ServicesService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<ServicesService> logger,
        ICacheRediservice cache)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
        _cache = cache;
    }

    /// <summary>
    /// Lấy danh sách tất cả dịch vụ từ cache hoặc DB.
    /// </summary>
    public async Task<IEnumerable<ServiceDto>> GetAllAsync(CancellationToken ct = default)
    {
        // Try get from cache first
        var cached = await _cache.GetAsync<List<ServiceDto>>(AllServicesCacheKey);
        if (cached != null)
        {
            _logger.LogDebug("Services loaded from cache");
            return cached;
        }

        // Load from database
        var services = await _unitOfWork.services.GetAllAsync(ct);
        var result = _mapper.Map<List<ServiceDto>>(services);
        
        // Store in cache
        await _cache.SetAsync(AllServicesCacheKey, result, CacheExpiry);
        _logger.LogDebug("Services cached: {Count} items", result.Count);
        
        return result;
    }

    /// <summary>
    /// Lấy thông tin dịch vụ theo ID từ cache hoặc DB.
    /// </summary>
    public async Task<ServiceDto?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        var cacheKey = string.Format(ServiceByIdCacheKey, id);
        
        // Try get from cache first
        var cached = await _cache.GetAsync<ServiceDto>(cacheKey);
        if (cached != null)
        {
            _logger.LogDebug("Service {Id} loaded from cache", id);
            return cached;
        }

        // Load from database
        var service = await _unitOfWork.services.GetByIdAsync(id, ct);
        if (service is null) return null;
        
        var result = _mapper.Map<ServiceDto>(service);
        
        // Store in cache
        await _cache.SetAsync(cacheKey, result, CacheExpiry);
        
        return result;
    }

    /// <summary>
    /// Tạo dịch vụ mới. Invalidate cache TRƯỚC khi thao tác DB.
    /// </summary>
    public async Task<ServiceDto> CreateAsync(CreateServiceDto dto, CancellationToken ct = default)
    {
        // Kiểm tra trùng tên dịch vụ
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

        // Invalidate cache TRƯỚC khi thêm vào DB
        await InvalidateServiceCacheAsync();

        var service = _mapper.Map<Service>(dto);
        service.CreatedAt = DateTime.UtcNow;

        await _unitOfWork.services.AddAsync(service, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Created service: {Name} with Id: {Id}", service.Name, service.Id);

        // Load fresh data and cache it
        var result = _mapper.Map<ServiceDto>(service);
        await _cache.SetAsync(string.Format(ServiceByIdCacheKey, service.Id), result, CacheExpiry);
        
        return result;
    }

    /// <summary>
    /// Cập nhật thông tin dịch vụ. Invalidate cache TRƯỚC khi thao tác DB.
    /// </summary>
    public async Task<ServiceDto> UpdateAsync(long id, UpdateServiceDto dto, CancellationToken ct = default)
    {
        var service = await _unitOfWork.services.GetByIdAsync(id, ct)
            ?? throw new NotFoundException($"Không tìm thấy dịch vụ với Id: {id}");

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

        // Invalidate cache TRƯỚC khi cập nhật DB
        await InvalidateServiceCacheAsync(id);

        _mapper.Map(dto, service);
        service.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.services.Update(service);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Updated service: {Id}", service.Id);

        // Load fresh data and cache it
        var result = _mapper.Map<ServiceDto>(service);
        await _cache.SetAsync(string.Format(ServiceByIdCacheKey, id), result, CacheExpiry);
        
        return result;
    }

    /// <summary>
    /// Xóa dịch vụ theo ID. Invalidate cache TRƯỚC khi thao tác DB.
    /// </summary>
    public async Task<bool> DeleteAsync(long id, CancellationToken ct = default)
    {
        var service = await _unitOfWork.services.GetByIdAsync(id, ct);

        if (service is null)
            return false;

        var hasBookingServices = await _unitOfWork.bookingservices
            .ExistsAsync(bs => bs.ServiceId == id, ct);

        if (hasBookingServices)
        {
            throw new BusinessException($"Không thể xóa dịch vụ '{service.Name}' vì đang có booking liên quan.");
        }

        // Invalidate cache TRƯỚC khi xóa khỏi DB
        await InvalidateServiceCacheAsync(id);

        _unitOfWork.services.Delete(service);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Deleted service: {Id} - {Name}", service.Id, service.Name);

        return true;
    }

    #region Private Cache Helpers

    private async Task InvalidateServiceCacheAsync(long? serviceId = null)
    {
        await _cache.RemoveAsync(AllServicesCacheKey);
        
        if (serviceId.HasValue)
        {
            await _cache.RemoveAsync(string.Format(ServiceByIdCacheKey, serviceId.Value));
        }
        
        _logger.LogDebug("Service cache invalidated");
    }

    #endregion
}
