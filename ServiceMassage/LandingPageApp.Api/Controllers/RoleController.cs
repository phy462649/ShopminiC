using LandingPageApp.Application.Dtos;
using LandingPageApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LandingPageApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "ADMIN")]
public class RoleController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RoleController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RoleDto>>> GetAll(CancellationToken ct)
        => Ok(await _roleService.GetAllAsync(ct));

    [HttpGet("{id:long}")]
    public async Task<ActionResult<RoleDto>> GetById(long id, CancellationToken ct)
    {
        var role = await _roleService.GetByIdAsync(id, ct);
        return role is null ? NotFound(new { message = $"Role với Id {id} không tồn tại" }) : Ok(role);
    }

    [HttpPost]
    public async Task<ActionResult<RoleDto>> Create([FromBody] CreateRoleDto dto, CancellationToken ct)
    {
        var result = await _roleService.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<RoleDto>> Update(long id, [FromBody] UpdateRoleDto dto, CancellationToken ct)
        => Ok(await _roleService.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
        => await _roleService.DeleteAsync(id, ct) ? NoContent() : NotFound(new { message = $"Role với Id {id} không tồn tại" });
}
