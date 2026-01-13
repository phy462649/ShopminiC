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
/// Quản lý việc tạo, cập nhật, xóa dịch vụ.
/// </summary>
public class ServicesService : IServicesService
{
    /// <summary>
    /// Unit of Work để quản lý transaction và repositories.
    /// </summary>
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// AutoMapper để chuyển đổi giữa Entity và DTO.
    /// </summary>
    private readonly IMapper _mapper;

    /// <summary>
    /// Logger để ghi log hoạt động.
    /// </summary>
    private readonly ILogger<ServicesService> _logger;

    /// <summary>
    /// Khởi tạo ServicesService với dependency injection.
    /// </summary>
    /// <param name="unitOfWork">Unit of Work.</param>
    /// <param name="mapper">AutoMapper instance.</param>
    /// <param name="logger">Logger instance.</param>
    public ServicesService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<ServicesService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Lấy danh sách tất cả dịch vụ.
    /// </summary>
    /// <param name="ct">Token hủy bỏ thao tác.</param>
    /// <returns>Danh sách dịch vụ.</returns>
    public async Task<IEnumerable<ServiceDto>> GetAllAsync(CancellationToken ct = default)
    {
        var services = await _unitOfWork.services.GetAllAsync(ct);
        return _mapper.Map<IEnumerable<ServiceDto>>(services);
    }

    /// <summary>
    /// Lấy thông tin dịch vụ theo ID.
    /// </summary>
    /// <param name="id">ID của dịch vụ.</param>
    /// <param name="ct">Token hủy bỏ thao tác.</param>
    /// <returns>Thông tin dịch vụ hoặc null nếu không tìm thấy.</returns>
    public async Task<ServiceDto?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        var service = await _unitOfWork.services.GetByIdAsync(id, ct);
        return service is null ? null : _mapper.Map<ServiceDto>(service);
    }

    /// <summary>
    /// Tạo dịch vụ mới.
    /// Kiểm tra trùng tên, thời gian và giá hợp lệ trước khi tạo.
    /// </summary>
    /// <param name="dto">Thông tin dịch vụ cần tạo.</param>
    /// <param name="ct">Token hủy bỏ thao tác.</param>
    /// <returns>Thông tin dịch vụ vừa tạo.</returns>
    /// <exception cref="BusinessException">Khi tên đã tồn tại hoặc dữ liệu không hợp lệ.</exception>
    public async Task<ServiceDto> CreateAsync(CreateServiceDto dto, CancellationToken ct = default)
    {
        // Kiểm tra trùng tên dịch vụ
        var existingService = await _unitOfWork.services
            .FindAsync(s => s.Name == dto.Name, ct);
        
        if (existingService.Any())
        {
            throw new BusinessException($"Dịch vụ với tên '{dto.Name}' đã tồn tại.");
        }

        // Kiểm tra thời gian dịch vụ hợp lệ
        if (dto.DurationMinutes <= 0)
        {
            throw new BusinessException("Thời gian dịch vụ phải lớn hơn 0 phút.");
        }

        // Kiểm tra giá dịch vụ hợp lệ
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

    /// <summary>
    /// Cập nhật thông tin dịch vụ.
    /// Kiểm tra trùng tên (loại trừ dịch vụ hiện tại) trước khi cập nhật.
    /// </summary>
    /// <param name="id">ID của dịch vụ cần cập nhật.</param>
    /// <param name="dto">Thông tin cập nhật.</param>
    /// <param name="ct">Token hủy bỏ thao tác.</param>
    /// <returns>Thông tin dịch vụ sau khi cập nhật.</returns>
    /// <exception cref="NotFoundException">Khi không tìm thấy dịch vụ.</exception>
    /// <exception cref="BusinessException">Khi tên đã tồn tại hoặc dữ liệu không hợp lệ.</exception>
    public async Task<ServiceDto> UpdateAsync(long id, UpdateServiceDto dto, CancellationToken ct = default)
    {
        var service = await _unitOfWork.services.GetByIdAsync(id, ct)
            ?? throw new NotFoundException($"Không tìm thấy dịch vụ với Id: {id}");

        // Kiểm tra trùng tên (loại trừ dịch vụ hiện tại)
        var existingService = await _unitOfWork.services
            .FindAsync(s => s.Name == dto.Name && s.Id != id, ct);
        
        if (existingService.Any())
        {
            throw new BusinessException($"Dịch vụ với tên '{dto.Name}' đã tồn tại.");
        }

        // Kiểm tra thời gian dịch vụ hợp lệ
        if (dto.DurationMinutes <= 0)
        {
            throw new BusinessException("Thời gian dịch vụ phải lớn hơn 0 phút.");
        }

        // Kiểm tra giá dịch vụ hợp lệ
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

    /// <summary>
    /// Xóa dịch vụ theo ID.
    /// Không thể xóa dịch vụ đang có booking liên quan.
    /// </summary>
    /// <param name="id">ID của dịch vụ cần xóa.</param>
    /// <param name="ct">Token hủy bỏ thao tác.</param>
    /// <returns>True nếu xóa thành công, false nếu không tìm thấy.</returns>
    /// <exception cref="BusinessException">Khi dịch vụ đang có booking liên quan.</exception>
    public async Task<bool> DeleteAsync(long id, CancellationToken ct = default)
    {
        var service = await _unitOfWork.services.GetByIdAsync(id, ct);

        if (service is null)
            return false;

        // Kiểm tra dịch vụ có booking liên quan không
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
