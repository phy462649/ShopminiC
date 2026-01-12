using LandingPageApp.Application.Dtos;
using LandingPageApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LandingPageApp.Api.Controllers;

/// <summary>
/// Controller quản lý phòng trong hệ thống.
/// Cung cấp các API để thực hiện CRUD và kiểm tra tình trạng phòng.
/// Yêu cầu quyền ADMIN để truy cập.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "ADMIN")]
public class RoomController : ControllerBase
{
    /// <summary>
    /// Service xử lý logic nghiệp vụ phòng.
    /// </summary>
    private readonly IRoomService _roomService;

    /// <summary>
    /// Khởi tạo RoomController với dependency injection.
    /// </summary>
    /// <param name="roomService">Service quản lý phòng.</param>
    public RoomController(IRoomService roomService)
    {
        _roomService = roomService;
    }

    /// <summary>
    /// Lấy danh sách tất cả các phòng trong hệ thống.
    /// </summary>
    /// <param name="ct">Token hủy bỏ thao tác.</param>
    /// <returns>Danh sách phòng.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<RoomDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<RoomDto>>> GetAll(CancellationToken ct)
    {
        var rooms = await _roomService.GetAllAsync(ct);
        return Ok(rooms);
    }

    /// <summary>
    /// Lấy thông tin phòng theo ID.
    /// </summary>
    /// <param name="id">ID của phòng cần lấy.</param>
    /// <param name="ct">Token hủy bỏ thao tác.</param>
    /// <returns>Thông tin phòng hoặc 404 nếu không tìm thấy.</returns>
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
    /// Kiểm tra tình trạng khả dụng của phòng trong khoảng thời gian.
    /// Dùng để kiểm tra xem phòng có thể đặt được hay không.
    /// </summary>
    /// <param name="id">ID của phòng cần kiểm tra.</param>
    /// <param name="startTime">Thời gian bắt đầu.</param>
    /// <param name="endTime">Thời gian kết thúc.</param>
    /// <param name="excludeBookingId">ID booking cần loại trừ (dùng khi cập nhật booking).</param>
    /// <param name="ct">Token hủy bỏ thao tác.</param>
    /// <returns>Trạng thái khả dụng của phòng.</returns>
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
    /// Tạo mới một phòng trong hệ thống.
    /// </summary>
    /// <param name="dto">Thông tin phòng cần tạo.</param>
    /// <param name="ct">Token hủy bỏ thao tác.</param>
    /// <returns>Thông tin phòng vừa tạo với HTTP 201.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(RoomDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RoomDto>> Create([FromBody] CreateRoomDto dto, CancellationToken ct)
    {
        var room = await _roomService.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = room.Id }, room);
    }

    /// <summary>
    /// Cập nhật thông tin phòng theo ID.
    /// </summary>
    /// <param name="id">ID của phòng cần cập nhật.</param>
    /// <param name="dto">Thông tin cập nhật.</param>
    /// <param name="ct">Token hủy bỏ thao tác.</param>
    /// <returns>Thông tin phòng sau khi cập nhật.</returns>
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
    /// Xóa phòng theo ID.
    /// </summary>
    /// <param name="id">ID của phòng cần xóa.</param>
    /// <param name="ct">Token hủy bỏ thao tác.</param>
    /// <returns>HTTP 204 nếu xóa thành công, 404 nếu không tìm thấy.</returns>
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
