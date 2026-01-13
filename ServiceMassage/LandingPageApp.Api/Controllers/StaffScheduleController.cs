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
    public async Task<ActionResult<IEnumerable<StaffScheduleDto>>> GetAll(CancellationToken ct)
        => Ok(await _staffScheduleService.GetAllAsync(ct));

    [HttpGet("{id:long}")]
    public async Task<ActionResult<StaffScheduleDto>> GetById(long id, CancellationToken ct)
    {
        var data = await _staffScheduleService.GetByIdAsync(id, ct);
        return data is null ? NotFound(new { message = "StaffSchedule không tồn tại" }) : Ok(data);
    }

    [HttpGet("staff/{staffId:long}")]
    public async Task<ActionResult<IEnumerable<StaffScheduleDto>>> GetByStaffId(long staffId, CancellationToken ct)
        => Ok(await _staffScheduleService.GetByStaffIdAsync(staffId, ct));

    [HttpGet("staff/{staffId:long}/weekly")]
    public async Task<ActionResult<StaffWeeklyScheduleDto>> GetWeeklySchedule(long staffId, CancellationToken ct)
        => Ok(await _staffScheduleService.GetWeeklyScheduleAsync(staffId, ct));

    [HttpPost]
    public async Task<ActionResult<StaffScheduleDto>> Create([FromBody] CreateStaffScheduleDto dto, CancellationToken ct)
    {
        var result = await _staffScheduleService.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPost("bulk")]
    public async Task<ActionResult<IEnumerable<StaffScheduleDto>>> CreateBulk([FromBody] CreateBulkStaffScheduleDto dto, CancellationToken ct)
        => CreatedAtAction(nameof(GetByStaffId), new { staffId = dto.StaffId }, await _staffScheduleService.CreateBulkAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<StaffScheduleDto>> Update(long id, [FromBody] UpdateStaffScheduleDto dto, CancellationToken ct)
        => Ok(await _staffScheduleService.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
        => await _staffScheduleService.DeleteAsync(id, ct) ? NoContent() : NotFound(new { message = "StaffSchedule không tồn tại" });

    [HttpDelete("staff/{staffId:long}")]
    public async Task<IActionResult> DeleteByStaffId(long staffId, CancellationToken ct)
        => await _staffScheduleService.DeleteByStaffIdAsync(staffId, ct) ? NoContent() : NotFound(new { message = "Không tìm thấy lịch làm việc của nhân viên" });
}
