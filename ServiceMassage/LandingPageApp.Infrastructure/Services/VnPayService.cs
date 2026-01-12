using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using LandingPageApp.Application.Dtos;
using LandingPageApp.Application.Interfaces;
using Microsoft.Extensions.Options;

namespace LandingPageApp.Infrastructure.Services;

/// <summary>
/// Cấu hình VNPay - đọc từ appsettings.json
/// </summary>
public class VnPaySettings
{
    /// <summary>
    /// Mã website của merchant (lấy từ VNPay)
    /// </summary>
    public string TmnCode { get; set; } = string.Empty;
    
    /// <summary>
    /// Chuỗi bí mật để tạo chữ ký (lấy từ VNPay)
    /// </summary>
    public string HashSecret { get; set; } = string.Empty;
    
    /// <summary>
    /// URL cổng thanh toán VNPay
    /// Sandbox: https://sandbox.vnpayment.vn/paymentv2/vpcpay.html
    /// Production: https://pay.vnpay.vn/vpcpay.html
    /// </summary>
    public string BaseUrl { get; set; } = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
    
    /// <summary>
    /// URL VNPay sẽ redirect user về sau khi thanh toán
    /// </summary>
    public string ReturnUrl { get; set; } = string.Empty;
    
    /// <summary>
    /// Phiên bản API VNPay
    /// </summary>
    public string Version { get; set; } = "2.1.0";
    
    /// <summary>
    /// Mã API: pay = thanh toán
    /// </summary>
    public string Command { get; set; } = "pay";
    
    /// <summary>
    /// Đơn vị tiền tệ: VND
    /// </summary>
    public string CurrCode { get; set; } = "VND";
    
    /// <summary>
    /// Ngôn ngữ: vn = Tiếng Việt, en = English
    /// </summary>
    public string Locale { get; set; } = "vn";
}

/// <summary>
/// Service xử lý tích hợp VNPay
/// - Tạo URL thanh toán
/// - Xác thực callback từ VNPay
/// </summary>
public class VnPayService : IVnPayService
{
    private readonly VnPaySettings _settings;

    public VnPayService(IOptions<VnPaySettings> settings)
    {
        _settings = settings.Value;
    }

    /// <summary>
    /// Tạo URL thanh toán VNPay
    /// </summary>
    /// <param name="request">Thông tin đơn hàng cần thanh toán</param>
    /// <param name="ipAddress">IP của khách hàng</param>
    /// <returns>URL redirect đến cổng thanh toán VNPay</returns>
    public string CreatePaymentUrl(VnPayRequestDto request, string ipAddress)
    {
        // Sử dụng SortedList để tự động sắp xếp theo key (yêu cầu của VNPay)
        var vnpay = new SortedList<string, string>();
        
        // Thêm các tham số theo tài liệu VNPay
        vnpay.Add("vnp_Version", _settings.Version);           // Phiên bản API
        vnpay.Add("vnp_Command", _settings.Command);           // Mã API (pay)
        vnpay.Add("vnp_TmnCode", _settings.TmnCode);           // Mã website merchant
        vnpay.Add("vnp_Amount", ((long)(request.Amount * 100)).ToString()); // Số tiền * 100 (VNPay yêu cầu)
        vnpay.Add("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss")); // Thời gian tạo
        vnpay.Add("vnp_CurrCode", _settings.CurrCode);         // Đơn vị tiền tệ
        vnpay.Add("vnp_IpAddr", ipAddress);                    // IP khách hàng
        vnpay.Add("vnp_Locale", _settings.Locale);             // Ngôn ngữ
        vnpay.Add("vnp_OrderInfo", request.OrderDescription);  // Mô tả đơn hàng
        vnpay.Add("vnp_OrderType", request.OrderType);         // Loại đơn hàng
        vnpay.Add("vnp_ReturnUrl", _settings.ReturnUrl);       // URL callback
        vnpay.Add("vnp_TxnRef", request.OrderId);              // Mã đơn hàng (unique)
        vnpay.Add("vnp_ExpireDate", DateTime.Now.AddMinutes(15).ToString("yyyyMMddHHmmss")); // Hết hạn sau 15 phút

        // Tạo chuỗi query string từ các tham số
        var queryString = BuildQueryString(vnpay);
        
        // Tạo chữ ký HMAC-SHA512
        var vnpSecureHash = HmacSHA512(_settings.HashSecret, queryString);
        
        // Trả về URL hoàn chỉnh với chữ ký
        return $"{_settings.BaseUrl}?{queryString}&vnp_SecureHash={vnpSecureHash}";
    }

    /// <summary>
    /// Xử lý và xác thực callback từ VNPay
    /// </summary>
    /// <param name="queryParams">Query parameters từ VNPay callback</param>
    /// <returns>Kết quả thanh toán đã được xác thực</returns>
    public VnPayResponseDto ProcessCallback(IDictionary<string, string> queryParams)
    {
        // Tạo SortedList để sắp xếp params (bỏ qua SecureHash)
        var vnpay = new SortedList<string, string>();
        
        foreach (var (key, value) in queryParams)
        {
            // Chỉ lấy các param bắt đầu bằng vnp_ và không phải SecureHash
            if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_") 
                && key != "vnp_SecureHash" && key != "vnp_SecureHashType")
            {
                vnpay.Add(key, value);
            }
        }

        // Parse các giá trị từ response
        var response = new VnPayResponseDto
        {
            OrderId = queryParams.TryGetValue("vnp_TxnRef", out var txnRef) ? txnRef : "",
            TransactionId = queryParams.TryGetValue("vnp_TransactionNo", out var transNo) ? transNo : "",
            ResponseCode = queryParams.TryGetValue("vnp_ResponseCode", out var respCode) ? respCode : "",
            TransactionStatus = queryParams.TryGetValue("vnp_TransactionStatus", out var transStatus) ? transStatus : "",
            Amount = queryParams.TryGetValue("vnp_Amount", out var amtStr) && decimal.TryParse(amtStr, out var amt) ? amt / 100 : 0,
            BankCode = queryParams.TryGetValue("vnp_BankCode", out var bankCode) ? bankCode : "",
            PayDate = queryParams.TryGetValue("vnp_PayDate", out var payDate) ? payDate : "",
            OrderInfo = queryParams.TryGetValue("vnp_OrderInfo", out var orderInfo) ? orderInfo : ""
        };

        // Lấy chữ ký từ VNPay gửi về
        queryParams.TryGetValue("vnp_SecureHash", out var vnpSecureHash);
        vnpSecureHash ??= "";
        
        // Tạo lại chữ ký từ data nhận được
        var signData = BuildQueryString(vnpay);
        var checkSignature = HmacSHA512(_settings.HashSecret, signData);

        // Xác thực: chữ ký khớp VÀ mã response = 00 (thành công)
        response.IsSuccess = vnpSecureHash.Equals(checkSignature, StringComparison.InvariantCultureIgnoreCase) 
                            && response.ResponseCode == "00";
        
        // Lấy message tương ứng với mã response
        response.Message = GetResponseMessage(response.ResponseCode);

        return response;
    }

    /// <summary>
    /// Tạo query string từ SortedList
    /// Format: key1=value1&key2=value2&...
    /// </summary>
    private string BuildQueryString(SortedList<string, string> data)
    {
        var query = new StringBuilder();
        foreach (var (key, value) in data)
        {
            if (!string.IsNullOrEmpty(value))
            {
                // URL encode cả key và value
                query.Append(WebUtility.UrlEncode(key) + "=" + WebUtility.UrlEncode(value) + "&");
            }
        }
        // Bỏ dấu & cuối cùng
        return query.ToString().TrimEnd('&');
    }

    /// <summary>
    /// Tạo chữ ký HMAC-SHA512
    /// </summary>
    /// <param name="key">Secret key</param>
    /// <param name="data">Data cần ký</param>
    /// <returns>Chữ ký dạng hex string</returns>
    private string HmacSHA512(string key, string data)
    {
        var keyBytes = Encoding.UTF8.GetBytes(key);
        var dataBytes = Encoding.UTF8.GetBytes(data);
        
        using var hmac = new HMACSHA512(keyBytes);
        var hashBytes = hmac.ComputeHash(dataBytes);
        
        // Chuyển sang hex string, lowercase, không có dấu -
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
    }

    /// <summary>
    /// Lấy message tiếng Việt tương ứng với mã response từ VNPay
    /// </summary>
    private string GetResponseMessage(string responseCode)
    {
        return responseCode switch
        {
            "00" => "Giao dịch thành công",
            "07" => "Trừ tiền thành công. Giao dịch bị nghi ngờ (liên quan tới lừa đảo, giao dịch bất thường)",
            "09" => "Thẻ/Tài khoản của khách hàng chưa đăng ký dịch vụ InternetBanking tại ngân hàng",
            "10" => "Khách hàng xác thực thông tin thẻ/tài khoản không đúng quá 3 lần",
            "11" => "Đã hết hạn chờ thanh toán. Xin quý khách vui lòng thực hiện lại giao dịch",
            "12" => "Thẻ/Tài khoản của khách hàng bị khóa",
            "13" => "Quý khách nhập sai mật khẩu xác thực giao dịch (OTP). Xin quý khách vui lòng thực hiện lại giao dịch",
            "24" => "Khách hàng hủy giao dịch",
            "51" => "Tài khoản của quý khách không đủ số dư để thực hiện giao dịch",
            "65" => "Tài khoản của Quý khách đã vượt quá hạn mức giao dịch trong ngày",
            "75" => "Ngân hàng thanh toán đang bảo trì",
            "79" => "Quý khách nhập sai mật khẩu thanh toán quá số lần quy định. Xin quý khách vui lòng thực hiện lại giao dịch",
            "99" => "Lỗi không xác định",
            _ => "Lỗi không xác định"
        };
    }
}
