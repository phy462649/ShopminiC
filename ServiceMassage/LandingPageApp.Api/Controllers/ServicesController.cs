using LandingPageApp.Application.Dtos;
using LandingPageApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LandingPageApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "ADMIN")]
public class ServicesController : ControllerBase
{
    private readonly IServicesService _servicesService;

    public ServicesController(IServicesService servicesService)
    {
        _servicesService = servicesService;
    }

    /// <summary>
    /// Get all services
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ServiceDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ServiceDto>>> GetAll(CancellationToken ct)
    {
        var services = await _servicesService.GetAllAsync(ct);
        return Ok(services);
    }

    /// <summary>
    /// Get service by ID
    /// </summary>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(ServiceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServiceDto>> GetById(long id, CancellationToken ct)
    {
        var service = await _servicesService.GetByIdAsync(id, ct);
        if (service is null)
            return NotFound(new { message = $"Không tìm thấy dịch vụ với Id: {id}" });
        return Ok(service);
    }

    /// <summary>
    /// Create a new service
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ServiceDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ServiceDto>> Create([FromBody] CreateServiceDto dto, CancellationToken ct)
    {
        var service = await _servicesService.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = service.Id }, service);
    }

    /// <summary>
    /// Update an existing service
    /// </summary>
    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(ServiceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ServiceDto>> Update(long id, [FromBody] UpdateServiceDto dto, CancellationToken ct)
    {
        var service = await _servicesService.UpdateAsync(id, dto, ct);
        return Ok(service);
    }

    /// <summary>
    /// Delete a service
    /// </summary>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
    {
        var result = await _servicesService.DeleteAsync(id, ct);
        if (!result)
            return NotFound(new { message = $"Không tìm thấy dịch vụ với Id: {id}" });
        return NoContent();
    }
}
