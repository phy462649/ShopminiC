using LandingPageApp.Application.Dtos;
using LandingPageApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LandingPageApp.Api.Controllers;

/// <summary>
/// Controller quản lý lịch làm việc của nhân viên.
/// Cung cấp các API để thực hiện CRUD và quản lý lịch làm việc theo tuần.
/// Yêu cầu quyền ADMIN để truy cập.
/// </summary>
[ApiController]
[Route("api/staff-schedule")]
[Authorize(Roles = "ADMIN")]
public class StaffScheduleController : ControllerBase
{
    /// <summary>
    /// Service xử lý logic nghiệp vụ lịch làm việc.
    /// </summary>
    private readonly IStaffScheduleService _staffScheduleService;

    /// <summary>
    /// Khởi tạo StaffScheduleController với dependency injection.
    /// </summary>
    /// <param name="staffScheduleService">Service quản lý lịch làm việc.</param>
    public StaffScheduleController(IStaffScheduleService staffScheduleService)
    {
        _staffScheduleService = staffScheduleService;
    }

    /// <summary>
    /// Lấy danh sách tất cả lịch làm việc.
    /// </summary>
    /// <param name="ct">Token hủy bỏ thao tác.</param>
    /// <returns>Danh sách lịch làm việc.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<StaffScheduleDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<StaffScheduleDto>>> GetAll(CancellationToken ct)
    {
        var data = await _staffScheduleService.GetAllAsync(ct);
        return Ok(data);
    }

    /// <summary>
    /// Lấy thông tin lịch làm việc theo ID.
    /// </summary>
    /// <param name="id">ID của lịch làm việc.</param>
    /// <param name="ct">Token hủy bỏ thao tác.</param>
    /// <returns>Thông tin lịch làm việc hoặc 404 nếu không tìm thấy.</returns>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(StaffScheduleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StaffScheduleDto>> GetById(long id, CancellationToken ct)
    {
        var data = await _staffScheduleService.GetByIdAsync(id, ct);
        if (data is null)
            return NotFound(new { message = "StaffSchedule không tồn tại" });
        return Ok(data);
    }

    /// <summary>
    /// Lấy danh sách lịch làm việc theo ID nhân viên.
    /// </summary>
    /// <param name="staffId">ID của nhân viên.</param>
    /// <param name="ct">Token hủy bỏ thao tác.</param>
    /// <returns>Danh sách lịch làm việc của nhân viên.</returns>
    [HttpGet("staff/{staffId:long}")]
    [ProducesResponseType(typeof(IEnumerable<StaffScheduleDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<StaffScheduleDto>>> GetByStaffId(long staffId, CancellationToken ct)
    {
        var data = await _staffScheduleService.GetByStaffIdAsync(staffId, ct);
        return Ok(data);
    }

    /// <summary>
    /// Lấy lịch làm việc theo tuần của nhân viên.
    /// Trả về lịch làm việc được nhóm theo các ngày trong tuần.
    /// </summary>
    /// <param name="staffId">ID của nhân viên.</param>
    /// <param name="ct">Token hủy bỏ thao tác.</param>
    /// <returns>Lịch làm việc theo tuần.</returns>
    [HttpGet("staff/{staffId:long}/weekly")]
    [ProducesResponseType(typeof(StaffWeeklyScheduleDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<StaffWeeklyScheduleDto>> GetWeeklySchedule(long staffId, CancellationToken ct)
    {
        var data = await _staffScheduleService.GetWeeklyScheduleAsync(staffId, ct);
        return Ok(data);
    }

    /// <summary>
    /// Tạo mới một lịch làm việc.
    /// </summary>
    /// <param name="dto">Thông tin lịch làm việc cần tạo.</param>
    /// <param name="ct">Token hủy bỏ thao tác.</param>
    /// <returns>Thông tin lịch làm việc vừa tạo với HTTP 201.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(StaffScheduleDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<StaffScheduleDto>> Create([FromBody] CreateStaffScheduleDto dto, CancellationToken ct)
    {
        var result = await _staffScheduleService.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Tạo nhiều lịch làm việc cùng lúc cho một nhân viên.
    /// Dùng để thiết lập lịch làm việc cho cả tuần.
    /// </summary>
    /// <param name="dto">Thông tin các lịch làm việc cần tạo.</param>
    /// <param name="ct">Token hủy bỏ thao tác.</param>
    /// <returns>Danh sách lịch làm việc vừa tạo.</returns>
    [HttpPost("bulk")]
    [ProducesResponseType(typeof(IEnumerable<StaffScheduleDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<StaffScheduleDto>>> CreateBulk([FromBody] CreateBulkStaffScheduleDto dto, CancellationToken ct)
    {
        var result = await _staffScheduleService.CreateBulkAsync(dto, ct);
        return CreatedAtAction(nameof(GetByStaffId), new { staffId = dto.StaffId }, result);
    }

    /// <summary>
    /// Cập nhật thông tin lịch làm việc theo ID.
    /// </summary>
    /// <param name="id">ID của lịch làm việc cần cập nhật.</param>
    /// <param name="dto">Thông tin cập nhật.</param>
    /// <param name="ct">Token hủy bỏ thao tác.</param>
    /// <returns>Thông tin lịch làm việc sau khi cập nhật.</returns>
    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(StaffScheduleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StaffScheduleDto>> Update(long id, [FromBody] UpdateStaffScheduleDto dto, CancellationToken ct)
    {
        var result = await _staffScheduleService.UpdateAsync(id, dto, ct);
        return Ok(result);
    }

    /// <summary>
    /// Xóa lịch làm việc theo ID.
    /// </summary>
    /// <param name="id">ID của lịch làm việc cần xóa.</param>
    /// <param name="ct">Token hủy bỏ thao tác.</param>
    /// <returns>HTTP 204 nếu xóa thành công, 404 nếu không tìm thấy.</returns>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
    {
        var deleted = await _staffScheduleService.DeleteAsync(id, ct);
        if (!deleted)
            return NotFound(new { message = "StaffSchedule không tồn tại" });
        return NoContent();
    }

    /// <summary>
    /// Xóa tất cả lịch làm việc của một nhân viên.
    /// </summary>
    /// <param name="staffId">ID của nhân viên.</param>
    /// <param name="ct">Token hủy bỏ thao tác.</param>
    /// <returns>HTTP 204 nếu xóa thành công, 404 nếu không tìm thấy.</returns>
    [HttpDelete("staff/{staffId:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteByStaffId(long staffId, CancellationToken ct)
    {
        var deleted = await _staffScheduleService.DeleteByStaffIdAsync(staffId, ct);
        if (!deleted)
            return NotFound(new { message = "Không tìm thấy lịch làm việc của nhân viên" });
        return NoContent();
    }
}
