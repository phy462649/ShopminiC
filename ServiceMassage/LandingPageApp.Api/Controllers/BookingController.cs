using LandingPageApp.Application.Dtos;
using LandingPageApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LandingPageApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "ADMIN,STAFF")]
public class BookingController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookingDto>>> GetAll(CancellationToken ct)
        => Ok(await _bookingService.GetAllAsync(ct));

    [HttpGet("{id:long}")]
    public async Task<ActionResult<BookingDto>> GetById(long id, CancellationToken ct)
    {
        var data = await _bookingService.GetByIdAsync(id, ct);
        return data is null ? NotFound(new { message = "Booking không tồn tại" }) : Ok(data);
    }

    [HttpGet("customer/{customerId:long}")]
    public async Task<ActionResult<IEnumerable<BookingDto>>> GetByCustomerId(long customerId, CancellationToken ct)
        => Ok(await _bookingService.GetByCustomerIdAsync(customerId, ct));

    [HttpGet("staff/{staffId:long}")]
    public async Task<ActionResult<IEnumerable<BookingDto>>> GetByStaffId(long staffId, CancellationToken ct)
        => Ok(await _bookingService.GetByStaffIdAsync(staffId, ct));

    [HttpPost]
    public async Task<ActionResult<BookingDto>> Create([FromBody] CreateBookingDto dto, CancellationToken ct)
    {
        var result = await _bookingService.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BookingDto>> Update(long id, [FromBody] UpdateBookingDto dto, CancellationToken ct)
        => Ok(await _bookingService.UpdateAsync(id, dto, ct));

    [HttpPatch("{id:long}/status")]
    public async Task<ActionResult<BookingDto>> UpdateStatus(long id, [FromBody] UpdateBookingStatusDto dto, CancellationToken ct)
        => Ok(await _bookingService.UpdateStatusAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
        => await _bookingService.DeleteAsync(id, ct) ? NoContent() : NotFound(new { message = "Booking không tồn tại" });

    [HttpGet("check-staff-available")]
    public async Task<ActionResult<bool>> CheckStaffAvailable([FromQuery] long staffId, [FromQuery] DateTime startTime, [FromQuery] DateTime endTime, CancellationToken ct)
        => Ok(new { available = await _bookingService.IsStaffAvailableAsync(staffId, startTime, endTime, null, ct) });

    [HttpGet("check-room-available")]
    public async Task<ActionResult<bool>> CheckRoomAvailable([FromQuery] long roomId, [FromQuery] DateTime startTime, [FromQuery] DateTime endTime, CancellationToken ct)
        => Ok(new { available = await _bookingService.IsRoomAvailableAsync(roomId, startTime, endTime, null, ct) });
}
