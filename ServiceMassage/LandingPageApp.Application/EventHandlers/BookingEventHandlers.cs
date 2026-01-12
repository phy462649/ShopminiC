using LandingPageApp.Application.Interfaces;
using LandingPageApp.Domain.Events;
using Microsoft.Extensions.Logging;

namespace LandingPageApp.Application.EventHandlers;

/// <summary>
/// Handler cho BookingCreatedEvent - gửi email xác nhận đặt lịch
/// </summary>
public class BookingCreatedEventHandler : IEventHandler<BookingCreatedEvent>
{
    private readonly IEmailService _emailService;
    private readonly ILogger<BookingCreatedEventHandler> _logger;

    public BookingCreatedEventHandler(IEmailService emailService, ILogger<BookingCreatedEventHandler> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    public async Task HandleAsync(BookingCreatedEvent domainEvent, CancellationToken ct = default)
    {
        _logger.LogInformation(
            "Handling BookingCreatedEvent: BookingId={BookingId}, CustomerId={CustomerId}",
            domainEvent.BookingId, domainEvent.CustomerId);

        if (!string.IsNullOrEmpty(domainEvent.CustomerEmail))
        {
            var subject = "Xác nhận đặt lịch - Spa Service";
            var body = $@"
                <h2>Cảm ơn bạn đã đặt lịch!</h2>
                <p>Mã đặt lịch: <strong>#{domainEvent.BookingId}</strong></p>
                <p>Thời gian: <strong>{domainEvent.BookingDate:dd/MM/yyyy HH:mm}</strong></p>
                <p>Chúng tôi sẽ liên hệ xác nhận trong thời gian sớm nhất.</p>
                <br/>
                <p>Trân trọng,<br/>Spa Service Team</p>
            ";

            await _emailService.SendEmailAsync(domainEvent.CustomerEmail, subject, body);
            _logger.LogInformation("Sent booking confirmation email to {Email}", domainEvent.CustomerEmail);
        }
    }
}

/// <summary>
/// Handler cho BookingConfirmedEvent - gửi email xác nhận
/// </summary>
public class BookingConfirmedEventHandler : IEventHandler<BookingConfirmedEvent>
{
    private readonly IEmailService _emailService;
    private readonly ILogger<BookingConfirmedEventHandler> _logger;

    public BookingConfirmedEventHandler(IEmailService emailService, ILogger<BookingConfirmedEventHandler> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    public async Task HandleAsync(BookingConfirmedEvent domainEvent, CancellationToken ct = default)
    {
        _logger.LogInformation(
            "Handling BookingConfirmedEvent: BookingId={BookingId}",
            domainEvent.BookingId);

        if (!string.IsNullOrEmpty(domainEvent.CustomerEmail))
        {
            var subject = "Đặt lịch đã được xác nhận - Spa Service";
            var body = $@"
                <h2>Đặt lịch của bạn đã được xác nhận!</h2>
                <p>Mã đặt lịch: <strong>#{domainEvent.BookingId}</strong></p>
                <p>Thời gian: <strong>{domainEvent.BookingDate:dd/MM/yyyy HH:mm}</strong></p>
                <p>Vui lòng đến đúng giờ. Nếu cần hủy, vui lòng liên hệ trước 24h.</p>
                <br/>
                <p>Trân trọng,<br/>Spa Service Team</p>
            ";

            await _emailService.SendEmailAsync(domainEvent.CustomerEmail, subject, body);
        }
    }
}

/// <summary>
/// Handler cho BookingCancelledEvent - gửi email thông báo hủy
/// </summary>
public class BookingCancelledEventHandler : IEventHandler<BookingCancelledEvent>
{
    private readonly IEmailService _emailService;
    private readonly ILogger<BookingCancelledEventHandler> _logger;

    public BookingCancelledEventHandler(IEmailService emailService, ILogger<BookingCancelledEventHandler> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    public async Task HandleAsync(BookingCancelledEvent domainEvent, CancellationToken ct = default)
    {
        _logger.LogInformation(
            "Handling BookingCancelledEvent: BookingId={BookingId}, Reason={Reason}",
            domainEvent.BookingId, domainEvent.CancellationReason);

        if (!string.IsNullOrEmpty(domainEvent.CustomerEmail))
        {
            var subject = "Đặt lịch đã bị hủy - Spa Service";
            var body = $@"
                <h2>Đặt lịch của bạn đã bị hủy</h2>
                <p>Mã đặt lịch: <strong>#{domainEvent.BookingId}</strong></p>
                {(string.IsNullOrEmpty(domainEvent.CancellationReason) ? "" : $"<p>Lý do: {domainEvent.CancellationReason}</p>")}
                <p>Nếu bạn muốn đặt lịch mới, vui lòng truy cập website của chúng tôi.</p>
                <br/>
                <p>Trân trọng,<br/>Spa Service Team</p>
            ";

            await _emailService.SendEmailAsync(domainEvent.CustomerEmail, subject, body);
        }
    }
}

/// <summary>
/// Handler cho BookingCompletedEvent - gửi email cảm ơn và yêu cầu đánh giá
/// </summary>
public class BookingCompletedEventHandler : IEventHandler<BookingCompletedEvent>
{
    private readonly IEmailService _emailService;
    private readonly ILogger<BookingCompletedEventHandler> _logger;

    public BookingCompletedEventHandler(IEmailService emailService, ILogger<BookingCompletedEventHandler> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    public async Task HandleAsync(BookingCompletedEvent domainEvent, CancellationToken ct = default)
    {
        _logger.LogInformation(
            "Handling BookingCompletedEvent: BookingId={BookingId}, TotalAmount={TotalAmount}",
            domainEvent.BookingId, domainEvent.TotalAmount);

        // Có thể thêm logic gửi email cảm ơn, yêu cầu đánh giá, tích điểm, v.v.
        await Task.CompletedTask;
    }
}
