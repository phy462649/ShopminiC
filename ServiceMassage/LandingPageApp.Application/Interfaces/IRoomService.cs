using LandingPageApp.Application.Dtos;

namespace LandingPageApp.Application.Interfaces;

public interface IRoomService
{
    Task<IEnumerable<RoomDto>> GetAllAsync(CancellationToken ct = default);
    Task<RoomDto?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<RoomDto> CreateAsync(CreateRoomDto dto, CancellationToken ct = default);
    Task<RoomDto> UpdateAsync(long id, UpdateRoomDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(long id, CancellationToken ct = default);
    Task<bool> IsAvailableAsync(long roomId, DateTime startTime, DateTime endTime, long? excludeBookingId = null, CancellationToken ct = default);
}
