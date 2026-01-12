namespace LandingPageApp.Application.Dtos;

/// <summary>
/// DTO để tạo request thanh toán VNPay
/// </summary>
public class VnPayRequestDto
{
    /// <summary>
    /// Mã đơn hàng (phải unique cho mỗi giao dịch)
    /// Format khuyến nghị: {TYPE}_{EntityId}_{PaymentId}
    /// Ví dụ: BOOKING_123_456, ORDER_789_101
    /// </summary>
    public string OrderId { get; set; } = string.Empty;

    /// <summary>
    /// Số tiền thanh toán (đơn vị: VND)
    /// Lưu ý: VNPay sẽ nhân 100 khi gửi đi
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Mô tả đơn hàng - hiển thị trên trang thanh toán VNPay
    /// Ví dụ: "Thanh toán booking #123"
    /// </summary>
    public string OrderDescription { get; set; } = string.Empty;

    /// <summary>
    /// Loại đơn hàng theo phân loại của VNPay
    /// Các giá trị: billpayment, fashion, food, other...
    /// Mặc định: other
    /// </summary>
    public string OrderType { get; set; } = "other";
}

/// <summary>
/// DTO chứa kết quả trả về từ VNPay callback
/// </summary>
public class VnPayResponseDto
{
    /// <summary>
    /// Kết quả giao dịch: true = thành công, false = thất bại
    /// Được xác định bởi: chữ ký hợp lệ VÀ ResponseCode = "00"
    /// </summary>
    public bool IsSuccess { get; set; }
    
    /// <summary>
    /// Mã đơn hàng (vnp_TxnRef) - giống với OrderId khi tạo request
    /// </summary>
    public string OrderId { get; set; } = string.Empty;
    
    /// <summary>
    /// Mã giao dịch tại VNPay (vnp_TransactionNo)
    /// Dùng để tra cứu, đối soát với VNPay
    /// </summary>
    public string TransactionId { get; set; } = string.Empty;
    
    /// <summary>
    /// Mã phản hồi từ VNPay (vnp_ResponseCode)
    /// 00 = Thành công, các mã khác = Lỗi
    /// </summary>
    public string ResponseCode { get; set; } = string.Empty;
    
    /// <summary>
    /// Trạng thái giao dịch (vnp_TransactionStatus)
    /// 00 = Thành công
    /// </summary>
    public string TransactionStatus { get; set; } = string.Empty;
    
    /// <summary>
    /// Số tiền thanh toán (đã chia 100 từ vnp_Amount)
    /// </summary>
    public decimal Amount { get; set; }
    
    /// <summary>
    /// Mã ngân hàng khách hàng sử dụng (vnp_BankCode)
    /// Ví dụ: NCB, VIETCOMBANK, TECHCOMBANK...
    /// </summary>
    public string BankCode { get; set; } = string.Empty;
    
    /// <summary>
    /// Thời gian thanh toán (vnp_PayDate)
    /// Format: yyyyMMddHHmmss
    /// </summary>
    public string PayDate { get; set; } = string.Empty;
    
    /// <summary>
    /// Thông tin đơn hàng (vnp_OrderInfo)
    /// </summary>
    public string OrderInfo { get; set; } = string.Empty;
    
    /// <summary>
    /// Message mô tả kết quả (tiếng Việt)
    /// </summary>
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// DTO request tạo thanh toán VNPay cho Booking
/// </summary>
public class CreateVnPayBookingPaymentDto
{
    /// <summary>
    /// ID của Booking cần thanh toán
    /// </summary>
    public long BookingId { get; set; }
}

/// <summary>
/// DTO request tạo thanh toán VNPay cho Order
/// </summary>
public class CreateVnPayOrderPaymentDto
{
    /// <summary>
    /// ID của Order cần thanh toán
    /// </summary>
    public long OrderId { get; set; }
}
