using LandingPageApp.Application.Dtos;
using LandingPageApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LandingPageApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "ADMIN,STAFF")]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PaymentDto>>> GetAll(CancellationToken ct)
        => Ok(await _paymentService.GetAllAsync(ct));

    [HttpGet("{id:long}")]
    public async Task<ActionResult<PaymentDto>> GetById(long id, CancellationToken ct)
    {
        var data = await _paymentService.GetByIdAsync(id, ct);
        return data is null ? NotFound(new { message = "Payment không tồn tại" }) : Ok(data);
    }

    [HttpGet("booking/{bookingId:long}")]
    public async Task<ActionResult<IEnumerable<PaymentDto>>> GetByBookingId(long bookingId, CancellationToken ct)
        => Ok(await _paymentService.GetByBookingIdAsync(bookingId, ct));

    [HttpGet("order/{orderId:long}")]
    public async Task<ActionResult<IEnumerable<PaymentDto>>> GetByOrderId(long orderId, CancellationToken ct)
        => Ok(await _paymentService.GetByOrderIdAsync(orderId, ct));

    [HttpPost]
    public async Task<ActionResult<PaymentDto>> Create([FromBody] CreatePaymentDto dto, CancellationToken ct)
    {
        var result = await _paymentService.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPatch("{id:long}/status")]
    public async Task<ActionResult<PaymentDto>> UpdateStatus(long id, [FromBody] UpdatePaymentStatusDto dto, CancellationToken ct)
        => Ok(await _paymentService.UpdateStatusAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
        => await _paymentService.DeleteAsync(id, ct) ? NoContent() : NotFound(new { message = "Payment không tồn tại" });
}
