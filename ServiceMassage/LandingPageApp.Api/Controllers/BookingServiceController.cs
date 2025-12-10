using LandingPageApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LandingPageApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
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
        /// <returns>List of booking services</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> GetAllBookingServices()
        {
            var bookingServices = await _bookingServiceService.GetAllAsync();
            return Ok(bookingServices);
        }

        /// <summary>
        /// Get booking service by ID
        /// </summary>
        /// <param name="id">Booking service ID</param>
        /// <returns>Booking service details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetBookingServiceById(int id)
        {
            var bookingService = await _bookingServiceService.GetByIdAsync(id);
            if (bookingService == null)
                return NotFound(new { message = "Booking service not found" });
            return Ok(bookingService);
        }

        /// <summary>
        /// Create a new booking service
        /// </summary>
        /// <param name="createDto">Booking service creation data</param>
        /// <returns>Created booking service</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> CreateBookingService([FromBody] object createDto)
        {
            try
            {
                var result = await _bookingServiceService.CreateAsync(createDto);
                return CreatedAtAction(nameof(GetBookingServiceById), new { id = 0 }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Update an existing booking service
        /// </summary>
        /// <param name="id">Booking service ID</param>
        /// <param name="updateDto">Booking service update data</param>
        /// <returns>Updated booking service</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdateBookingService(int id, [FromBody] object updateDto)
        {
            try
            {
                var result = await _bookingServiceService.UpdateAsync(id, updateDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Delete a booking service
        /// </summary>
        /// <param name="id">Booking service ID</param>
        /// <returns>Deletion result</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> DeleteBookingService(int id)
        {
            try
            {
                var result = await _bookingServiceService.DeleteAsync(id);
                return Ok(new { success = result, message = result ? "Booking service deleted successfully" : "Failed to delete booking service" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
