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

public class RoomService : IRoomService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<RoomService> _logger;

    public RoomService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<RoomService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<RoomDto>> GetAllAsync(CancellationToken ct = default)
    {
        var rooms = await _unitOfWork.room.GetAllAsync(ct);
        return _mapper.Map<IEnumerable<RoomDto>>(rooms);
    }

    public async Task<RoomDto?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        var room = await _unitOfWork.room.GetByIdAsync(id, ct);
        return room is null ? null : _mapper.Map<RoomDto>(room);
    }

    public async Task<RoomDto> CreateAsync(CreateRoomDto dto, CancellationToken ct = default)
    {
        // Check duplicate name
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

    public async Task<RoomDto> UpdateAsync(long id, UpdateRoomDto dto, CancellationToken ct = default)
    {
        var room = await _unitOfWork.room.GetByIdAsync(id, ct)
            ?? throw new NotFoundException($"Không tìm thấy phòng với Id: {id}");

        // Check duplicate name (exclude current room)
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

    public async Task<bool> DeleteAsync(long id, CancellationToken ct = default)
    {
        var room = await _unitOfWork.room.GetByIdAsync(id, ct);

        if (room is null)
            return false;

        // Check if room has any bookings
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

    public async Task<bool> IsAvailableAsync(long roomId, DateTime startTime, DateTime endTime, long? excludeBookingId = null, CancellationToken ct = default)
    {
        var query = _unitOfWork.bookings.Query()
            .Where(b => b.RoomId == roomId)
            .Where(b => b.Status != StatusBooking.Cancelled)
            .Where(b => b.StartTime < endTime && b.EndTime > startTime);

        if (excludeBookingId.HasValue)
        {
            query = query.Where(b => b.Id != excludeBookingId.Value);
        }

        var hasConflict = await query.AnyAsync(ct);
        return !hasConflict;
    }
}
