using LandingPageApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LandingPageApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class StaffScheduleController : ControllerBase
    {
        private readonly IStaffScheduleService _staffScheduleService;

        public StaffScheduleController(IStaffScheduleService staffScheduleService)
        {
            _staffScheduleService = staffScheduleService;
        }

        /// <summary>
        /// Get all staff schedules
        /// </summary>
        /// <returns>List of staff schedules</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> GetAllStaffSchedules()
        {
            var staffSchedules = await _staffScheduleService.GetAllAsync();
            return Ok(staffSchedules);
        }

        /// <summary>
        /// Get staff schedule by ID
        /// </summary>
        /// <param name="id">Staff schedule ID</param>
        /// <returns>Staff schedule details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetStaffScheduleById(int id)
        {
            var staffSchedule = await _staffScheduleService.GetByIdAsync(id);
            if (staffSchedule == null)
                return NotFound(new { message = "Staff schedule not found" });
            return Ok(staffSchedule);
        }

        /// <summary>
        /// Create a new staff schedule
        /// </summary>
        /// <param name="createDto">Staff schedule creation data</param>
        /// <returns>Created staff schedule</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> CreateStaffSchedule([FromBody] object createDto)
        {
            try
            {
                var result = await _staffScheduleService.CreateAsync(createDto);
                return CreatedAtAction(nameof(GetStaffScheduleById), new { id = 0 }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Update an existing staff schedule
        /// </summary>
        /// <param name="id">Staff schedule ID</param>
        /// <param name="updateDto">Staff schedule update data</param>
        /// <returns>Updated staff schedule</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdateStaffSchedule(int id, [FromBody] object updateDto)
        {
            try
            {
                var result = await _staffScheduleService.UpdateAsync(id, updateDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Delete a staff schedule
        /// </summary>
        /// <param name="id">Staff schedule ID</param>
        /// <returns>Deletion result</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> DeleteStaffSchedule(int id)
        {
            try
            {
                var result = await _staffScheduleService.DeleteAsync(id);
                return Ok(new { success = result, message = result ? "Staff schedule deleted successfully" : "Failed to delete staff schedule" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
