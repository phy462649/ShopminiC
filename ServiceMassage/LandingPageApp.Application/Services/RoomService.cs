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
/// Service xử lý logic nghiệp vụ cho phòng.
/// Quản lý việc tạo, cập nhật, xóa phòng và kiểm tra tình trạng khả dụng.
/// </summary>
public class RoomService : IRoomService
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
    private readonly ILogger<RoomService> _logger;

    /// <summary>
    /// Khởi tạo RoomService với dependency injection.
    /// </summary>
    /// <param name="unitOfWork">Unit of Work.</param>
    /// <param name="mapper">AutoMapper instance.</param>
    /// <param name="logger">Logger instance.</param>
    public RoomService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<RoomService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Lấy danh sách tất cả phòng.
    /// </summary>
    /// <param name="ct">Token hủy bỏ thao tác.</param>
    /// <returns>Danh sách phòng.</returns>
    public async Task<IEnumerable<RoomDto>> GetAllAsync(CancellationToken ct = default)
    {
        var rooms = await _unitOfWork.room.GetAllAsync(ct);
        return _mapper.Map<IEnumerable<RoomDto>>(rooms);
    }

    /// <summary>
    /// Lấy thông tin phòng theo ID.
    /// </summary>
    /// <param name="id">ID của phòng.</param>
    /// <param name="ct">Token hủy bỏ thao tác.</param>
    /// <returns>Thông tin phòng hoặc null nếu không tìm thấy.</returns>
    public async Task<RoomDto?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        var room = await _unitOfWork.room.GetByIdAsync(id, ct);
        return room is null ? null : _mapper.Map<RoomDto>(room);
    }

    /// <summary>
    /// Tạo phòng mới.
    /// Kiểm tra trùng tên trước khi tạo.
    /// </summary>
    /// <param name="dto">Thông tin phòng cần tạo.</param>
    /// <param name="ct">Token hủy bỏ thao tác.</param>
    /// <returns>Thông tin phòng vừa tạo.</returns>
    /// <exception cref="BusinessException">Khi tên phòng đã tồn tại.</exception>
    public async Task<RoomDto> CreateAsync(CreateRoomDto dto, CancellationToken ct = default)
    {
        // Kiểm tra trùng tên phòng
        var existingRoom = await _unitOfWork.room
            .FindAsync(r => r.Name == dto.Name, ct);
        
        if (existingRoom.Any())
        {
            throw new BusinessException($"Phòng với tên '{dto.Name}' đã tồn tại.");
        }

        var room = _mapper.Map<Room>(dto);
        room.CreatedAt = DateTime.UtcNow;

        await _unitOfWork.room.AddAsync(room, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Created room: {Name} with Id: {Id}", room.Name, room.Id);

        return _mapper.Map<RoomDto>(room);
    }

    /// <summary>
    /// Cập nhật thông tin phòng.
    /// Kiểm tra trùng tên (loại trừ phòng hiện tại) trước khi cập nhật.
    /// </summary>
    /// <param name="id">ID của phòng cần cập nhật.</param>
    /// <param name="dto">Thông tin cập nhật.</param>
    /// <param name="ct">Token hủy bỏ thao tác.</param>
    /// <returns>Thông tin phòng sau khi cập nhật.</returns>
    /// <exception cref="NotFoundException">Khi không tìm thấy phòng.</exception>
    /// <exception cref="BusinessException">Khi tên phòng đã tồn tại.</exception>
    public async Task<RoomDto> UpdateAsync(long id, UpdateRoomDto dto, CancellationToken ct = default)
    {
        var room = await _unitOfWork.room.GetByIdAsync(id, ct)
            ?? throw new NotFoundException($"Không tìm thấy phòng với Id: {id}");

        // Kiểm tra trùng tên (loại trừ phòng hiện tại)
        var existingRoom = await _unitOfWork.room
            .FindAsync(r => r.Name == dto.Name && r.Id != id, ct);
        
        if (existingRoom.Any())
        {
            throw new BusinessException($"Phòng với tên '{dto.Name}' đã tồn tại.");
        }

        _mapper.Map(dto, room);
        room.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.room.Update(room);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Updated room: {Id}", room.Id);

        return _mapper.Map<RoomDto>(room);
    }

    /// <summary>
    /// Xóa phòng theo ID.
    /// Không thể xóa phòng đang có booking liên quan.
    /// </summary>
    /// <param name="id">ID của phòng cần xóa.</param>
    /// <param name="ct">Token hủy bỏ thao tác.</param>
    /// <returns>True nếu xóa thành công, false nếu không tìm thấy.</returns>
    /// <exception cref="BusinessException">Khi phòng đang có booking liên quan.</exception>
    public async Task<bool> DeleteAsync(long id, CancellationToken ct = default)
    {
        var room = await _unitOfWork.room.GetByIdAsync(id, ct);

        if (room is null)
            return false;

        // Kiểm tra phòng có booking liên quan không
        var hasBookings = await _unitOfWork.bookings
            .ExistsAsync(b => b.RoomId == id, ct);

        if (hasBookings)
        {
            throw new BusinessException($"Không thể xóa phòng '{room.Name}' vì đang có booking liên quan.");
        }

        _unitOfWork.room.Delete(room);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Deleted room: {Id} - {Name}", room.Id, room.Name);

        return true;
    }

    /// <summary>
    /// Kiểm tra tình trạng khả dụng của phòng trong khoảng thời gian.
    /// Dùng để kiểm tra xem phòng có thể đặt được hay không.
    /// </summary>
    /// <param name="roomId">ID của phòng cần kiểm tra.</param>
    /// <param name="startTime">Thời gian bắt đầu.</param>
    /// <param name="endTime">Thời gian kết thúc.</param>
    /// <param name="excludeBookingId">ID booking cần loại trừ (dùng khi cập nhật booking).</param>
    /// <param name="ct">Token hủy bỏ thao tác.</param>
    /// <returns>True nếu phòng khả dụng, false nếu đã có booking trùng.</returns>
    public async Task<bool> IsAvailableAsync(long roomId, DateTime startTime, DateTime endTime, long? excludeBookingId = null, CancellationToken ct = default)
    {
        // Tìm các booking trùng thời gian (không tính booking đã hủy)
        var query = _unitOfWork.bookings.Query()
            .Where(b => b.RoomId == roomId)
            .Where(b => b.Status != StatusBooking.Cancelled)
            .Where(b => b.StartTime < endTime && b.EndTime > startTime);

        // Loại trừ booking hiện tại nếu đang cập nhật
        if (excludeBookingId.HasValue)
        {
            query = query.Where(b => b.Id != excludeBookingId.Value);
        }

        var hasConflict = await query.AnyAsync(ct);
        return !hasConflict;
    }
}
