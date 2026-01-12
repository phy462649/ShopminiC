using LandingPageApp.Application.Dtos;
using LandingPageApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LandingPageApp.Api.Controllers;

/// <summary>
/// Controller quản lý dịch vụ trong hệ thống.
/// Cung cấp các API để thực hiện CRUD cho dịch vụ massage/spa.
/// Yêu cầu quyền ADMIN để truy cập.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "ADMIN")]
public class ServicesController : ControllerBase
{
    /// <summary>
    /// Service xử lý logic nghiệp vụ dịch vụ.
    /// </summary>
    private readonly IServicesService _servicesService;

    /// <summary>
    /// Khởi tạo ServicesController với dependency injection.
    /// </summary>
    /// <param name="servicesService">Service quản lý dịch vụ.</param>
    public ServicesController(IServicesService servicesService)
    {
        _servicesService = servicesService;
    }

    /// <summary>
    /// Lấy danh sách tất cả các dịch vụ trong hệ thống.
    /// </summary>
    /// <param name="ct">Token hủy bỏ thao tác.</param>
    /// <returns>Danh sách dịch vụ.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ServiceDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ServiceDto>>> GetAll(CancellationToken ct)
    {
        var services = await _servicesService.GetAllAsync(ct);
        return Ok(services);
    }

    /// <summary>
    /// Lấy thông tin dịch vụ theo ID.
    /// </summary>
    /// <param name="id">ID của dịch vụ cần lấy.</param>
    /// <param name="ct">Token hủy bỏ thao tác.</param>
    /// <returns>Thông tin dịch vụ hoặc 404 nếu không tìm thấy.</returns>
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
    /// Tạo mới một dịch vụ trong hệ thống.
    /// </summary>
    /// <param name="dto">Thông tin dịch vụ cần tạo.</param>
    /// <param name="ct">Token hủy bỏ thao tác.</param>
    /// <returns>Thông tin dịch vụ vừa tạo với HTTP 201.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ServiceDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ServiceDto>> Create([FromBody] CreateServiceDto dto, CancellationToken ct)
    {
        var service = await _servicesService.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = service.Id }, service);
    }

    /// <summary>
    /// Cập nhật thông tin dịch vụ theo ID.
    /// </summary>
    /// <param name="id">ID của dịch vụ cần cập nhật.</param>
    /// <param name="dto">Thông tin cập nhật.</param>
    /// <param name="ct">Token hủy bỏ thao tác.</param>
    /// <returns>Thông tin dịch vụ sau khi cập nhật.</returns>
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
    /// Xóa dịch vụ theo ID.
    /// </summary>
    /// <param name="id">ID của dịch vụ cần xóa.</param>
    /// <param name="ct">Token hủy bỏ thao tác.</param>
    /// <returns>HTTP 204 nếu xóa thành công, 404 nếu không tìm thấy.</returns>
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
