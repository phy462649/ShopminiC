using LandingPageApp.Application.Dtos;
using LandingPageApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LandingPageApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "ADMIN")]
public class BookingServiceController : ControllerBase
{
    private readonly IBookingServiceService _bookingServiceService;

    public BookingServiceController(IBookingServiceService bookingServiceService)
    {
        _bookingServiceService = bookingServiceService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookingServiceItemDto>>> GetAll(CancellationToken ct)
        => Ok(await _bookingServiceService.GetAllAsync(ct));

    [HttpGet("{id:long}")]
    public async Task<ActionResult<BookingServiceItemDto>> GetById(long id, CancellationToken ct)
    {
        var item = await _bookingServiceService.GetByIdAsync(id, ct);
        return item is null ? NotFound(new { message = $"Không tìm thấy booking service với Id: {id}" }) : Ok(item);
    }

    [HttpGet("booking/{bookingId:long}")]
    public async Task<ActionResult<IEnumerable<BookingServiceItemDto>>> GetByBookingId(long bookingId, CancellationToken ct)
        => Ok(await _bookingServiceService.GetByBookingIdAsync(bookingId, ct));

    [HttpPost]
    public async Task<ActionResult<BookingServiceItemDto>> Create([FromBody] CreateBookingServiceItemDto dto, CancellationToken ct)
    {
        var item = await _bookingServiceService.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
        => await _bookingServiceService.DeleteAsync(id, ct) ? NoContent() : NotFound(new { message = $"Không tìm thấy booking service với Id: {id}" });
}
