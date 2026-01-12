using LandingPageApp.Application.Dtos;
using LandingPageApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LandingPageApp.Api.Controllers;

[ApiController]
[Route("api/staff-schedule")]
[Authorize(Roles = "ADMIN")]
public class StaffScheduleController : ControllerBase
{
    private readonly IStaffScheduleService _staffScheduleService;

    public StaffScheduleController(IStaffScheduleService staffScheduleService)
    {
        _staffScheduleService = staffScheduleService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<StaffScheduleDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<StaffScheduleDto>>> GetAll(CancellationToken ct)
    {
        var data = await _staffScheduleService.GetAllAsync(ct);
        return Ok(data);
    }

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

    [HttpGet("staff/{staffId:long}")]
    [ProducesResponseType(typeof(IEnumerable<StaffScheduleDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<StaffScheduleDto>>> GetByStaffId(long staffId, CancellationToken ct)
    {
        var data = await _staffScheduleService.GetByStaffIdAsync(staffId, ct);
        return Ok(data);
    }

    [HttpGet("staff/{staffId:long}/weekly")]
    [ProducesResponseType(typeof(StaffWeeklyScheduleDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<StaffWeeklyScheduleDto>> GetWeeklySchedule(long staffId, CancellationToken ct)
    {
        var data = await _staffScheduleService.GetWeeklyScheduleAsync(staffId, ct);
        return Ok(data);
    }

    [HttpPost]
    [ProducesResponseType(typeof(StaffScheduleDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<StaffScheduleDto>> Create([FromBody] CreateStaffScheduleDto dto, CancellationToken ct)
    {
        var result = await _staffScheduleService.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPost("bulk")]
    [ProducesResponseType(typeof(IEnumerable<StaffScheduleDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<StaffScheduleDto>>> CreateBulk([FromBody] CreateBulkStaffScheduleDto dto, CancellationToken ct)
    {
        var result = await _staffScheduleService.CreateBulkAsync(dto, ct);
        return CreatedAtAction(nameof(GetByStaffId), new { staffId = dto.StaffId }, result);
    }

    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(StaffScheduleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StaffScheduleDto>> Update(long id, [FromBody] UpdateStaffScheduleDto dto, CancellationToken ct)
    {
        var result = await _staffScheduleService.UpdateAsync(id, dto, ct);
        return Ok(result);
    }

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
