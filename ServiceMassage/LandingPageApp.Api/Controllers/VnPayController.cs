using LandingPageApp.Application.Dtos;
using LandingPageApp.Application.Interfaces;
using LandingPageApp.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LandingPageApp.Api.Controllers;

/// <summary>
/// Controller xử lý thanh toán qua VNPay
/// Cung cấp các endpoint để tạo URL thanh toán và xử lý callback từ VNPay
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class VnPayController : ControllerBase
{
    private readonly IVnPayService _vnPayService;      // Service xử lý logic VNPay
    private readonly IPaymentService _paymentService;  // Service quản lý Payment
    private readonly IBookingService _bookingService;  // Service quản lý Booking
    private readonly IOrderService _orderService;      // Service quản lý Order
    private readonly ILogger<VnPayController> _logger; // Logger để ghi log

    public VnPayController(IVnPayService vnPayService, IPaymentService paymentService,
        IBookingService bookingService, IOrderService orderService, ILogger<VnPayController> logger)
    {
        _vnPayService = vnPayService;
        _paymentService = paymentService;
        _bookingService = bookingService;
        _orderService = orderService;
        _logger = logger;
    }

    /// <summary>
    /// Tạo URL thanh toán VNPay cho Booking
    /// </summary>
    /// <param name="dto">Chứa BookingId cần thanh toán</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>URL thanh toán VNPay và PaymentId</returns>
    [HttpPost("booking")]
    [Authorize]
    public async Task<IActionResult> CreateBookingPayment([FromBody] CreateVnPayBookingPaymentDto dto, CancellationToken ct)
    {
        // Kiểm tra booking có tồn tại không
        var booking = await _bookingService.GetByIdAsync(dto.BookingId, ct);
        if (booking is null) 
            return NotFound(new { message = "Booking không tồn tại" });
        
        // Kiểm tra booking có tổng tiền chưa
        if (booking.TotalAmount is null || booking.TotalAmount <= 0)
            return BadRequest(new { message = "Booking chưa có tổng tiền" });

        // Tạo record Payment với trạng thái Pending
        var payment = await _paymentService.CreateAsync(new CreatePaymentDto
        {
            PaymentType = Payment_type.Booking,    // Loại thanh toán: Booking
            BookingId = dto.BookingId,             // Liên kết với Booking
            PersonalId = booking.CustomerId,       // Người thanh toán
            Method = PaymentMethod.VNPay           // Phương thức: VNPay
        }, ct);

        // Tạo request để gửi đến VNPay
        var request = new VnPayRequestDto
        {
            // Format: BOOKING_{BookingId}_{PaymentId} để parse khi callback
            OrderId = $"BOOKING_{dto.BookingId}_{payment.Id}",
            Amount = booking.TotalAmount.Value,
            OrderDescription = $"Thanh toán booking #{dto.BookingId}",
            OrderType = "other"
        };

        // Tạo URL thanh toán VNPay
        var paymentUrl = _vnPayService.CreatePaymentUrl(request, GetIpAddress());
        
        _logger.LogInformation("Tạo URL VNPay cho Booking {BookingId}, Payment {PaymentId}", 
            dto.BookingId, payment.Id);

        return Ok(new { paymentUrl, paymentId = payment.Id });
    }

    /// <summary>
    /// Tạo URL thanh toán VNPay cho Order (đơn hàng sản phẩm)
    /// </summary>
    /// <param name="dto">Chứa OrderId cần thanh toán</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>URL thanh toán VNPay và PaymentId</returns>
    [HttpPost("order")]
    [Authorize]
    public async Task<IActionResult> CreateOrderPayment([FromBody] CreateVnPayOrderPaymentDto dto, CancellationToken ct)
    {
        // Kiểm tra order có tồn tại không
        var order = await _orderService.GetByIdAsync(dto.OrderId, ct);
        if (order is null) 
            return NotFound(new { message = "Order không tồn tại" });
        
        // Kiểm tra order có tổng tiền chưa
        if (order.TotalAmount is null || order.TotalAmount <= 0) 
            return BadRequest(new { message = "Order chưa có tổng tiền" });

        // Tạo record Payment với trạng thái Pending
        var payment = await _paymentService.CreateAsync(new CreatePaymentDto
        {
            PaymentType = Payment_type.Order,      // Loại thanh toán: Order
            OrderId = dto.OrderId,                 // Liên kết với Order
            PersonalId = order.CustomerId,         // Người thanh toán
            Method = PaymentMethod.VNPay           // Phương thức: VNPay
        }, ct);

        // Tạo request để gửi đến VNPay
        var request = new VnPayRequestDto
        {
            // Format: ORDER_{OrderId}_{PaymentId} để parse khi callback
            OrderId = $"ORDER_{dto.OrderId}_{payment.Id}",
            Amount = order.TotalAmount.Value,
            OrderDescription = $"Thanh toán đơn hàng #{dto.OrderId}",
            OrderType = "other"
        };

        // Tạo URL thanh toán VNPay
        var paymentUrl = _vnPayService.CreatePaymentUrl(request, GetIpAddress());
        
        _logger.LogInformation("Tạo URL VNPay cho Order {OrderId}, Payment {PaymentId}", 
            dto.OrderId, payment.Id);

        return Ok(new { paymentUrl, paymentId = payment.Id });
    }

    /// <summary>
    /// VNPay IPN (Instant Payment Notification) - Callback server-to-server
    /// VNPay gọi endpoint này để thông báo kết quả thanh toán
    /// Endpoint này KHÔNG redirect user, chỉ trả về response cho VNPay
    /// </summary>
    [HttpGet("ipn")]
    [AllowAnonymous] // Cho phép VNPay gọi mà không cần auth
    public async Task<IActionResult> VnPayIPN(CancellationToken ct)
    {
        _logger.LogInformation("Nhận VNPay IPN callback");

        // Chuyển query params thành Dictionary để xử lý
        var queryParams = Request.Query.ToDictionary(x => x.Key, x => x.Value.ToString());
        
        // Xác thực chữ ký và parse response từ VNPay
        var response = _vnPayService.ProcessCallback(queryParams);

        // Nếu thanh toán thất bại hoặc chữ ký không hợp lệ
        if (!response.IsSuccess)
        {
            _logger.LogWarning("VNPay IPN thất bại: {Message}", response.Message);
            return Ok(new { RspCode = "99", Message = response.Message });
        }

        // Parse OrderId để lấy PaymentId
        // Format: {TYPE}_{EntityId}_{PaymentId}
        var parts = response.OrderId.Split('_');
        if (parts.Length < 3 || !long.TryParse(parts[2], out var paymentId))
        {
            _logger.LogWarning("OrderId không hợp lệ: {OrderId}", response.OrderId);
            return Ok(new { RspCode = "99", Message = "Invalid OrderId" });
        }

        // Cập nhật trạng thái Payment thành Completed
        await _paymentService.UpdateStatusAsync(paymentId, 
            new UpdatePaymentStatusDto { Status = PaymentStatus.Completed }, ct);

        // Nếu là thanh toán Booking, cập nhật trạng thái Booking thành Confirmed
        if (parts[0] == "BOOKING" && long.TryParse(parts[1], out var bookingId))
        {
            await _bookingService.UpdateStatusAsync(bookingId, 
                new UpdateBookingStatusDto { Status = StatusBooking.Confirmed }, ct);
            
            _logger.LogInformation("Booking {BookingId} đã được xác nhận sau thanh toán", bookingId);
        }

        _logger.LogInformation("VNPay IPN thành công: PaymentId {PaymentId}", paymentId);
        
        // Trả về response theo format VNPay yêu cầu
        return Ok(new { RspCode = "00", Message = "Success" });
    }

    /// <summary>
    /// VNPay Return URL - Redirect user sau khi thanh toán
    /// User được redirect về đây sau khi hoàn tất thanh toán trên VNPay
    /// </summary>
    [HttpGet("return")]
    [AllowAnonymous] // User chưa login lại sau khi từ VNPay về
    public async Task<IActionResult> VnPayReturn(CancellationToken ct)
    {
        _logger.LogInformation("Nhận VNPay Return callback");

        // Chuyển query params thành Dictionary để xử lý
        var queryParams = Request.Query.ToDictionary(x => x.Key, x => x.Value.ToString());
        
        // Xác thực chữ ký và parse response từ VNPay
        var response = _vnPayService.ProcessCallback(queryParams);

        // Parse OrderId để lấy PaymentId
        var parts = response.OrderId.Split('_');
        long.TryParse(parts.Length >= 3 ? parts[2] : "0", out var paymentId);

        // Cập nhật trạng thái Payment dựa trên kết quả
        var status = response.IsSuccess ? PaymentStatus.Completed : PaymentStatus.Failed;
        if (paymentId > 0)
        {
            await _paymentService.UpdateStatusAsync(paymentId, 
                new UpdatePaymentStatusDto { Status = status }, ct);
        }

        // Log kết quả
        if (response.IsSuccess)
            _logger.LogInformation("VNPay Return thành công: OrderId {OrderId}", response.OrderId);
        else
            _logger.LogWarning("VNPay Return thất bại: {Message}", response.Message);

        // Redirect user về trang kết quả thanh toán trên Frontend
        return Redirect($"/payment/result?success={response.IsSuccess}&orderId={response.OrderId}");
    }

    /// <summary>
    /// Lấy IP address của client để gửi cho VNPay
    /// Ưu tiên lấy từ header X-Forwarded-For (nếu qua proxy/load balancer)
    /// </summary>
    private string GetIpAddress() =>
        Request.Headers["X-Forwarded-For"].FirstOrDefault() 
        ?? HttpContext.Connection.RemoteIpAddress?.ToString() 
        ?? "127.0.0.1";
}
