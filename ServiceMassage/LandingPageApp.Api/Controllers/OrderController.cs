using LandingPageApp.Application.Dtos;
using LandingPageApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LandingPageApp.Api.Controllers;

/// <summary>
/// Controller quản lý đơn hàng sản phẩm.
/// Cung cấp các API để thực hiện các thao tác CRUD trên đơn hàng,
/// bao gồm tạo mới, xem chi tiết, cập nhật trạng thái và xóa đơn hàng.
/// </summary>
/// <remarks>
/// Controller này yêu cầu xác thực và chỉ cho phép người dùng có vai trò ADMIN hoặc STAFF truy cập.
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "ADMIN,STAFF")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    /// <summary>
    /// Khởi tạo một instance mới của <see cref="OrderController"/>.
    /// </summary>
    /// <param name="orderService">Service xử lý logic nghiệp vụ đơn hàng.</param>
    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    /// <summary>
    /// Lấy danh sách tất cả đơn hàng trong hệ thống.
    /// </summary>
    /// <param name="ct">Token hủy bỏ để hủy thao tác bất đồng bộ nếu cần.</param>
    /// <returns>Danh sách tất cả đơn hàng.</returns>
    /// <response code="200">Trả về danh sách đơn hàng thành công.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<OrderDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetAll(CancellationToken ct)
    {
        var data = await _orderService.GetAllAsync(ct);
        return Ok(data);
    }

    /// <summary>
    /// Lấy thông tin chi tiết của một đơn hàng theo ID.
    /// </summary>
    /// <param name="id">ID của đơn hàng cần lấy thông tin.</param>
    /// <param name="ct">Token hủy bỏ để hủy thao tác bất đồng bộ nếu cần.</param>
    /// <returns>Thông tin chi tiết của đơn hàng.</returns>
    /// <response code="200">Trả về thông tin đơn hàng thành công.</response>
    /// <response code="404">Không tìm thấy đơn hàng với ID được cung cấp.</response>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderDto>> GetById(long id, CancellationToken ct)
    {
        var data = await _orderService.GetByIdAsync(id, ct);
        if (data is null)
            return NotFound(new { message = "Order không tồn tại" });
        return Ok(data);
    }

    /// <summary>
    /// Lấy danh sách đơn hàng của một khách hàng cụ thể.
    /// </summary>
    /// <param name="customerId">ID của khách hàng cần lấy danh sách đơn hàng.</param>
    /// <param name="ct">Token hủy bỏ để hủy thao tác bất đồng bộ nếu cần.</param>
    /// <returns>Danh sách đơn hàng của khách hàng.</returns>
    /// <response code="200">Trả về danh sách đơn hàng của khách hàng thành công.</response>
    [HttpGet("customer/{customerId:long}")]
    [ProducesResponseType(typeof(IEnumerable<OrderDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetByCustomerId(long customerId, CancellationToken ct)
    {
        var data = await _orderService.GetByCustomerIdAsync(customerId, ct);
        return Ok(data);
    }

    /// <summary>
    /// Tạo mới một đơn hàng.
    /// </summary>
    /// <param name="dto">Dữ liệu đơn hàng cần tạo bao gồm thông tin khách hàng và các sản phẩm.</param>
    /// <param name="ct">Token hủy bỏ để hủy thao tác bất đồng bộ nếu cần.</param>
    /// <returns>Thông tin đơn hàng vừa được tạo.</returns>
    /// <response code="201">Tạo đơn hàng thành công, trả về thông tin đơn hàng mới.</response>
    /// <response code="400">Dữ liệu đầu vào không hợp lệ.</response>
    [HttpPost]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OrderDto>> Create([FromBody] CreateOrderDto dto, CancellationToken ct)
    {
        var result = await _orderService.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Cập nhật trạng thái của một đơn hàng.
    /// </summary>
    /// <param name="id">ID của đơn hàng cần cập nhật trạng thái.</param>
    /// <param name="dto">Dữ liệu trạng thái mới của đơn hàng.</param>
    /// <param name="ct">Token hủy bỏ để hủy thao tác bất đồng bộ nếu cần.</param>
    /// <returns>Thông tin đơn hàng sau khi cập nhật trạng thái.</returns>
    /// <response code="200">Cập nhật trạng thái đơn hàng thành công.</response>
    /// <response code="404">Không tìm thấy đơn hàng với ID được cung cấp.</response>
    [HttpPatch("{id:long}/status")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderDto>> UpdateStatus(long id, [FromBody] UpdateOrderStatusDto dto, CancellationToken ct)
    {
        var result = await _orderService.UpdateStatusAsync(id, dto, ct);
        return Ok(result);
    }

    /// <summary>
    /// Xóa một đơn hàng khỏi hệ thống.
    /// </summary>
    /// <param name="id">ID của đơn hàng cần xóa.</param>
    /// <param name="ct">Token hủy bỏ để hủy thao tác bất đồng bộ nếu cần.</param>
    /// <returns>Không có nội dung trả về nếu xóa thành công.</returns>
    /// <response code="204">Xóa đơn hàng thành công.</response>
    /// <response code="404">Không tìm thấy đơn hàng với ID được cung cấp.</response>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
    {
        var deleted = await _orderService.DeleteAsync(id, ct);
        if (!deleted)
            return NotFound(new { message = "Order không tồn tại" });
        return NoContent();
    }
}
