using LandingPageApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LandingPageApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
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
        /// <returns>List of rooms</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> GetAllRooms()
        {
            var rooms = await _roomService.GetAllAsync();
            return Ok(rooms);
        }

        /// <summary>
        /// Get room by ID
        /// </summary>
        /// <param name="id">Room ID</param>
        /// <returns>Room details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetRoomById(int id)
        {
            var room = await _roomService.GetByIdAsync(id);
            if (room == null)
                return NotFound(new { message = "Room not found" });
            return Ok(room);
        }

        /// <summary>
        /// Create a new room
        /// </summary>
        /// <param name="createDto">Room creation data</param>
        /// <returns>Created room</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> CreateRoom([FromBody] object createDto)
        {
            try
            {
                var result = await _roomService.CreateAsync(createDto);
                return CreatedAtAction(nameof(GetRoomById), new { id = 0 }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Update an existing room
        /// </summary>
        /// <param name="id">Room ID</param>
        /// <param name="updateDto">Room update data</param>
        /// <returns>Updated room</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdateRoom(int id, [FromBody] object updateDto)
        {
            try
            {
                var result = await _roomService.UpdateAsync(id, updateDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Delete a room
        /// </summary>
        /// <param name="id">Room ID</param>
        /// <returns>Deletion result</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> DeleteRoom(int id)
        {
            try
            {
                var result = await _roomService.DeleteAsync(id);
                return Ok(new { success = result, message = result ? "Room deleted successfully" : "Failed to delete room" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
