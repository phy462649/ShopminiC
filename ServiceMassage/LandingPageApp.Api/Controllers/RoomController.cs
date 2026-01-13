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

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RoomDto>>> GetAll(CancellationToken ct)
        => Ok(await _roomService.GetAllAsync(ct));

    [HttpGet("{id:long}")]
    public async Task<ActionResult<RoomDto>> GetById(long id, CancellationToken ct)
    {
        var room = await _roomService.GetByIdAsync(id, ct);
        return room is null ? NotFound(new { message = $"Không tìm thấy phòng với Id: {id}" }) : Ok(room);
    }

    [HttpGet("{id:long}/availability")]
    public async Task<ActionResult> CheckAvailability(long id, [FromQuery] DateTime startTime, [FromQuery] DateTime endTime, [FromQuery] long? excludeBookingId, CancellationToken ct)
        => Ok(new { roomId = id, startTime, endTime, isAvailable = await _roomService.IsAvailableAsync(id, startTime, endTime, excludeBookingId, ct) });

    [HttpPost]
    public async Task<ActionResult<RoomDto>> Create([FromBody] CreateRoomDto dto, CancellationToken ct)
    {
        var room = await _roomService.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = room.Id }, room);
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<RoomDto>> Update(long id, [FromBody] UpdateRoomDto dto, CancellationToken ct)
        => Ok(await _roomService.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
        => await _roomService.DeleteAsync(id, ct) ? NoContent() : NotFound(new { message = $"Không tìm thấy phòng với Id: {id}" });
}
