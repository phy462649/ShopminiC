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

    /// <summary>
    /// Get all booking services
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<BookingServiceItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<BookingServiceItemDto>>> GetAll(CancellationToken ct)
    {
        var items = await _bookingServiceService.GetAllAsync(ct);
        return Ok(items);
    }

    /// <summary>
    /// Get booking service by ID
    /// </summary>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(BookingServiceItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookingServiceItemDto>> GetById(long id, CancellationToken ct)
    {
        var item = await _bookingServiceService.GetByIdAsync(id, ct);
        if (item is null)
            return NotFound(new { message = $"Không tìm thấy booking service với Id: {id}" });
        return Ok(item);
    }

    /// <summary>
    /// Get booking services by booking ID
    /// </summary>
    [HttpGet("booking/{bookingId:long}")]
    [ProducesResponseType(typeof(IEnumerable<BookingServiceItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<BookingServiceItemDto>>> GetByBookingId(long bookingId, CancellationToken ct)
    {
        var items = await _bookingServiceService.GetByBookingIdAsync(bookingId, ct);
        return Ok(items);
    }

    /// <summary>
    /// Create a new booking service
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(BookingServiceItemDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BookingServiceItemDto>> Create([FromBody] CreateBookingServiceItemDto dto, CancellationToken ct)
    {
        var item = await _bookingServiceService.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    /// <summary>
    /// Delete a booking service
    /// </summary>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
    {
        var result = await _bookingServiceService.DeleteAsync(id, ct);
        if (!result)
            return NotFound(new { message = $"Không tìm thấy booking service với Id: {id}" });
        return NoContent();
    }
}
