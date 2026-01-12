using LandingPageApp.Application.Dtos;
using LandingPageApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LandingPageApp.Api.Controllers;

/// <summary>
/// Controller quản lý các thao tác liên quan đến thanh toán trong hệ thống.
/// Cung cấp các API để tạo, xem, cập nhật trạng thái và xóa thanh toán.
/// Yêu cầu quyền ADMIN hoặc STAFF để truy cập.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "ADMIN,STAFF")]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    /// <summary>
    /// Khởi tạo một instance mới của <see cref="PaymentController"/>.
    /// </summary>
    /// <param name="paymentService">Service xử lý logic nghiệp vụ thanh toán.</param>
    public PaymentController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    /// <summary>
    /// Lấy danh sách tất cả các thanh toán trong hệ thống.
    /// </summary>
    /// <param name="ct">Token hủy bỏ để hủy yêu cầu nếu cần.</param>
    /// <returns>Danh sách các thanh toán dưới dạng <see cref="PaymentDto"/>.</returns>
    /// <response code="200">Trả về danh sách thanh toán thành công.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PaymentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PaymentDto>>> GetAll(CancellationToken ct)
    {
        var data = await _paymentService.GetAllAsync(ct);
        return Ok(data);
    }

    /// <summary>
    /// Lấy thông tin chi tiết của một thanh toán theo ID.
    /// </summary>
    /// <param name="id">ID của thanh toán cần lấy thông tin.</param>
    /// <param name="ct">Token hủy bỏ để hủy yêu cầu nếu cần.</param>
    /// <returns>Thông tin thanh toán dưới dạng <see cref="PaymentDto"/>.</returns>
    /// <response code="200">Trả về thông tin thanh toán thành công.</response>
    /// <response code="404">Không tìm thấy thanh toán với ID được cung cấp.</response>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(PaymentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PaymentDto>> GetById(long id, CancellationToken ct)
    {
        var data = await _paymentService.GetByIdAsync(id, ct);
        if (data is null)
            return NotFound(new { message = "Payment không tồn tại" });
        return Ok(data);
    }

    /// <summary>
    /// Lấy danh sách các thanh toán theo ID đặt chỗ (booking).
    /// </summary>
    /// <param name="bookingId">ID của đặt chỗ cần lấy danh sách thanh toán.</param>
    /// <param name="ct">Token hủy bỏ để hủy yêu cầu nếu cần.</param>
    /// <returns>Danh sách các thanh toán liên quan đến đặt chỗ.</returns>
    /// <response code="200">Trả về danh sách thanh toán theo booking thành công.</response>
    [HttpGet("booking/{bookingId:long}")]
    [ProducesResponseType(typeof(IEnumerable<PaymentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PaymentDto>>> GetByBookingId(long bookingId, CancellationToken ct)
    {
        var data = await _paymentService.GetByBookingIdAsync(bookingId, ct);
        return Ok(data);
    }

    /// <summary>
    /// Lấy danh sách các thanh toán theo ID đơn hàng (order).
    /// </summary>
    /// <param name="orderId">ID của đơn hàng cần lấy danh sách thanh toán.</param>
    /// <param name="ct">Token hủy bỏ để hủy yêu cầu nếu cần.</param>
    /// <returns>Danh sách các thanh toán liên quan đến đơn hàng.</returns>
    /// <response code="200">Trả về danh sách thanh toán theo order thành công.</response>
    [HttpGet("order/{orderId:long}")]
    [ProducesResponseType(typeof(IEnumerable<PaymentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PaymentDto>>> GetByOrderId(long orderId, CancellationToken ct)
    {
        var data = await _paymentService.GetByOrderIdAsync(orderId, ct);
        return Ok(data);
    }

    /// <summary>
    /// Tạo một thanh toán mới trong hệ thống.
    /// </summary>
    /// <param name="dto">Dữ liệu thanh toán cần tạo.</param>
    /// <param name="ct">Token hủy bỏ để hủy yêu cầu nếu cần.</param>
    /// <returns>Thông tin thanh toán vừa được tạo.</returns>
    /// <response code="201">Tạo thanh toán thành công.</response>
    /// <response code="400">Dữ liệu đầu vào không hợp lệ.</response>
    [HttpPost]
    [ProducesResponseType(typeof(PaymentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PaymentDto>> Create([FromBody] CreatePaymentDto dto, CancellationToken ct)
    {
        var result = await _paymentService.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Cập nhật trạng thái của một thanh toán.
    /// </summary>
    /// <param name="id">ID của thanh toán cần cập nhật trạng thái.</param>
    /// <param name="dto">Dữ liệu trạng thái mới cần cập nhật.</param>
    /// <param name="ct">Token hủy bỏ để hủy yêu cầu nếu cần.</param>
    /// <returns>Thông tin thanh toán sau khi cập nhật.</returns>
    /// <response code="200">Cập nhật trạng thái thanh toán thành công.</response>
    /// <response code="404">Không tìm thấy thanh toán với ID được cung cấp.</response>
    [HttpPatch("{id:long}/status")]
    [ProducesResponseType(typeof(PaymentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PaymentDto>> UpdateStatus(long id, [FromBody] UpdatePaymentStatusDto dto, CancellationToken ct)
    {
        var result = await _paymentService.UpdateStatusAsync(id, dto, ct);
        return Ok(result);
    }

    /// <summary>
    /// Xóa một thanh toán khỏi hệ thống.
    /// </summary>
    /// <param name="id">ID của thanh toán cần xóa.</param>
    /// <param name="ct">Token hủy bỏ để hủy yêu cầu nếu cần.</param>
    /// <returns>Không có nội dung nếu xóa thành công.</returns>
    /// <response code="204">Xóa thanh toán thành công.</response>
    /// <response code="404">Không tìm thấy thanh toán với ID được cung cấp.</response>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
    {
        var deleted = await _paymentService.DeleteAsync(id, ct);
        if (!deleted)
            return NotFound(new { message = "Payment không tồn tại" });
        return NoContent();
    }
}
