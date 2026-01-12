using LandingPageApp.Application.Dtos;

namespace LandingPageApp.Application.Interfaces;

/// <summary>
/// Interface cho VNPay Service
/// Định nghĩa các phương thức tích hợp thanh toán VNPay
/// </summary>
public interface IVnPayService
{
    /// <summary>
    /// Tạo URL thanh toán VNPay
    /// URL này sẽ redirect user đến cổng thanh toán VNPay
    /// </summary>
    /// <param name="request">Thông tin đơn hàng: OrderId, Amount, Description</param>
    /// <param name="ipAddress">IP address của khách hàng (bắt buộc theo VNPay)</param>
    /// <returns>URL hoàn chỉnh để redirect user đến VNPay</returns>
    string CreatePaymentUrl(VnPayRequestDto request, string ipAddress);

    /// <summary>
    /// Xử lý và xác thực callback từ VNPay
    /// Được gọi khi VNPay redirect user về hoặc gọi IPN
    /// </summary>
    /// <param name="queryParams">Query parameters từ VNPay (đã convert sang Dictionary)</param>
    /// <returns>Kết quả thanh toán đã được xác thực chữ ký</returns>
    VnPayResponseDto ProcessCallback(IDictionary<string, string> queryParams);
}
