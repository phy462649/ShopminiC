using LandingPageApp.Application.Dtos;
using LandingPageApp.Application.Interfaces;
using LandingPageApp.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LandingPageApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VnPayController : ControllerBase
{
    private readonly IVnPayService _vnPayService;
    private readonly IPaymentService _paymentService;
    private readonly IBookingService _bookingService;
    private readonly IOrderService _orderService;
    private readonly ILogger<VnPayController> _logger;

    public VnPayController(IVnPayService vnPayService, IPaymentService paymentService,
        IBookingService bookingService, IOrderService orderService, ILogger<VnPayController> logger)
    {
        _vnPayService = vnPayService;
        _paymentService = paymentService;
        _bookingService = bookingService;
        _orderService = orderService;
        _logger = logger;
    }

    [HttpPost("booking")]
    [Authorize]
    public async Task<IActionResult> CreateBookingPayment([FromBody] CreateVnPayBookingPaymentDto dto, CancellationToken ct)
    {
        var booking = await _bookingService.GetByIdAsync(dto.BookingId, ct);
        if (booking is null) return NotFound(new { message = "Booking không tồn tại" });
        if (booking.TotalAmount is null || booking.TotalAmount <= 0)
            return BadRequest(new { message = "Booking chưa có tổng tiền" });

        var payment = await _paymentService.CreateAsync(new CreatePaymentDto
        {
            PaymentType = Payment_type.Booking, BookingId = dto.BookingId,
            PersonalId = booking.CustomerId, Method = PaymentMethod.VNPay
        }, ct);

        var request = new VnPayRequestDto
        {
            OrderId = $"BOOKING_{dto.BookingId}_{payment.Id}",
            Amount = booking.TotalAmount.Value,
            OrderDescription = $"Thanh toán booking #{dto.BookingId}",
            OrderType = "other"
        };

        var paymentUrl = _vnPayService.CreatePaymentUrl(request, GetIpAddress());
        _logger.LogInformation("Tạo URL VNPay cho Booking {BookingId}, Payment {PaymentId}", dto.BookingId, payment.Id);

        return Ok(new { paymentUrl, paymentId = payment.Id });
    }

    [HttpPost("order")]
    [Authorize]
    public async Task<IActionResult> CreateOrderPayment([FromBody] CreateVnPayOrderPaymentDto dto, CancellationToken ct)
    {
        var order = await _orderService.GetByIdAsync(dto.OrderId, ct);
        if (order is null) return NotFound(new { message = "Order không tồn tại" });
        if (order.TotalAmount is null || order.TotalAmount <= 0)
            return BadRequest(new { message = "Order chưa có tổng tiền" });

        var payment = await _paymentService.CreateAsync(new CreatePaymentDto
        {
            PaymentType = Payment_type.Order, OrderId = dto.OrderId,
            PersonalId = order.CustomerId, Method = PaymentMethod.VNPay
        }, ct);

        var request = new VnPayRequestDto
        {
            OrderId = $"ORDER_{dto.OrderId}_{payment.Id}",
            Amount = order.TotalAmount.Value,
            OrderDescription = $"Thanh toán đơn hàng #{dto.OrderId}",
            OrderType = "other"
        };

        var paymentUrl = _vnPayService.CreatePaymentUrl(request, GetIpAddress());
        _logger.LogInformation("Tạo URL VNPay cho Order {OrderId}, Payment {PaymentId}", dto.OrderId, payment.Id);

        return Ok(new { paymentUrl, paymentId = payment.Id });
    }

    [HttpGet("ipn")]
    [AllowAnonymous]
    public async Task<IActionResult> VnPayIPN(CancellationToken ct)
    {
        _logger.LogInformation("Nhận VNPay IPN callback");
        var queryParams = Request.Query.ToDictionary(x => x.Key, x => x.Value.ToString());
        var response = _vnPayService.ProcessCallback(queryParams);

        if (!response.IsSuccess)
        {
            _logger.LogWarning("VNPay IPN thất bại: {Message}", response.Message);
            return Ok(new { RspCode = "99", Message = response.Message });
        }

        var parts = response.OrderId.Split('_');
        if (parts.Length < 3 || !long.TryParse(parts[2], out var paymentId))
        {
            _logger.LogWarning("OrderId không hợp lệ: {OrderId}", response.OrderId);
            return Ok(new { RspCode = "99", Message = "Invalid OrderId" });
        }

        await _paymentService.UpdateStatusAsync(paymentId, new UpdatePaymentStatusDto { Status = PaymentStatus.Completed }, ct);

        if (parts[0] == "BOOKING" && long.TryParse(parts[1], out var bookingId))
        {
            await _bookingService.UpdateStatusAsync(bookingId, new UpdateBookingStatusDto { Status = StatusBooking.Confirmed }, ct);
            _logger.LogInformation("Booking {BookingId} đã được xác nhận sau thanh toán", bookingId);
        }

        _logger.LogInformation("VNPay IPN thành công: PaymentId {PaymentId}", paymentId);
        return Ok(new { RspCode = "00", Message = "Success" });
    }

    [HttpGet("return")]
    [AllowAnonymous]
    public async Task<IActionResult> VnPayReturn(CancellationToken ct)
    {
        _logger.LogInformation("Nhận VNPay Return callback");
        var queryParams = Request.Query.ToDictionary(x => x.Key, x => x.Value.ToString());
        var response = _vnPayService.ProcessCallback(queryParams);

        var parts = response.OrderId.Split('_');
        long.TryParse(parts.Length >= 3 ? parts[2] : "0", out var paymentId);

        var status = response.IsSuccess ? PaymentStatus.Completed : PaymentStatus.Failed;
        if (paymentId > 0)
            await _paymentService.UpdateStatusAsync(paymentId, new UpdatePaymentStatusDto { Status = status }, ct);

        if (response.IsSuccess)
            _logger.LogInformation("VNPay Return thành công: OrderId {OrderId}", response.OrderId);
        else
            _logger.LogWarning("VNPay Return thất bại: {Message}", response.Message);

        return Redirect($"/payment/result?success={response.IsSuccess}&orderId={response.OrderId}");
    }

    private string GetIpAddress() =>
        Request.Headers["X-Forwarded-For"].FirstOrDefault()
        ?? HttpContext.Connection.RemoteIpAddress?.ToString()
        ?? "127.0.0.1";
}
