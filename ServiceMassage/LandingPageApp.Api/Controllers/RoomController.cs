using LandingPageApp.Application.Dtos;
using LandingPageApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LandingPageApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "ADMIN")]
public class RoomController : ControllerBase
{
    private readonly IRoomService _roomService;

    public RoomController(IRoomService roomService)
    {
        _roomService = roomService;
    }

    /// <summary>
    /// Get all rooms
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<RoomDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<RoomDto>>> GetAll(CancellationToken ct)
    {
        var rooms = await _roomService.GetAllAsync(ct);
        return Ok(rooms);
    }

    /// <summary>
    /// Get room by ID
    /// </summary>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(RoomDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RoomDto>> GetById(long id, CancellationToken ct)
    {
        var room = await _roomService.GetByIdAsync(id, ct);
        if (room is null)
            return NotFound(new { message = $"Không tìm thấy phòng với Id: {id}" });
        return Ok(room);
    }

    /// <summary>
    /// Check room availability
    /// </summary>
    [HttpGet("{id:long}/availability")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<ActionResult> CheckAvailability(
        long id, 
        [FromQuery] DateTime startTime, 
        [FromQuery] DateTime endTime,
        [FromQuery] long? excludeBookingId,
        CancellationToken ct)
    {
        var isAvailable = await _roomService.IsAvailableAsync(id, startTime, endTime, excludeBookingId, ct);
        return Ok(new { roomId = id, startTime, endTime, isAvailable });
    }

    /// <summary>
    /// Create a new room
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(RoomDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RoomDto>> Create([FromBody] CreateRoomDto dto, CancellationToken ct)
    {
        var room = await _roomService.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = room.Id }, room);
    }

    /// <summary>
    /// Update an existing room
    /// </summary>
    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(RoomDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RoomDto>> Update(long id, [FromBody] UpdateRoomDto dto, CancellationToken ct)
    {
        var room = await _roomService.UpdateAsync(id, dto, ct);
        return Ok(room);
    }

    /// <summary>
    /// Delete a room
    /// </summary>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
    {
        var result = await _roomService.DeleteAsync(id, ct);
        if (!result)
            return NotFound(new { message = $"Không tìm thấy phòng với Id: {id}" });
        return NoContent();
    }
}
