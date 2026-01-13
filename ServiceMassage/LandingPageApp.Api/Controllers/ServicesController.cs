using LandingPageApp.Application.Dtos;
using LandingPageApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LandingPageApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
//[Authorize(Roles = "ADMIN")]
public class ServicesController : ControllerBase
{
    private readonly IServicesService _servicesService;

    public ServicesController(IServicesService servicesService)
    {
        _servicesService = servicesService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ServiceDto>>> GetAll(CancellationToken ct)
        => Ok(await _servicesService.GetAllAsync(ct));

    [HttpGet("{id:long}")]
    public async Task<ActionResult<ServiceDto>> GetById(long id, CancellationToken ct)
    {
        var service = await _servicesService.GetByIdAsync(id, ct);
        return service is null ? NotFound(new { message = $"Không tìm thấy dịch vụ với Id: {id}" }) : Ok(service);
    }

    [HttpPost]
    public async Task<ActionResult<ServiceDto>> Create([FromBody] CreateServiceDto dto, CancellationToken ct)
    {
        var service = await _servicesService.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = service.Id }, service);
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<ServiceDto>> Update(long id, [FromBody] UpdateServiceDto dto, CancellationToken ct)
        => Ok(await _servicesService.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
        => await _servicesService.DeleteAsync(id, ct) ? NoContent() : NotFound(new { message = $"Không tìm thấy dịch vụ với Id: {id}" });
}
