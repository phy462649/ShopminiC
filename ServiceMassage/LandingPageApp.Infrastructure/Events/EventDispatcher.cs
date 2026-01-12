using LandingPageApp.Application.Interfaces;
using LandingPageApp.Domain.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LandingPageApp.Infrastructure.Events;

/// <summary>
/// Implementation của IEventDispatcher sử dụng DI để resolve handlers
/// </summary>
public class EventDispatcher : IEventDispatcher
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<EventDispatcher> _logger;

    public EventDispatcher(IServiceProvider serviceProvider, ILogger<EventDispatcher> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task DispatchAsync(IDomainEvent domainEvent, CancellationToken ct = default)
    {
        var eventType = domainEvent.GetType();
        var handlerType = typeof(IEventHandler<>).MakeGenericType(eventType);

        var handlers = _serviceProvider.GetServices(handlerType);

        foreach (var handler in handlers)
        {
            try
            {
                var method = handlerType.GetMethod("HandleAsync");
                if (method != null)
                {
                    var task = (Task?)method.Invoke(handler, new object[] { domainEvent, ct });
                    if (task != null)
                    {
                        await task;
                    }
                }

                _logger.LogInformation(
                    "Dispatched event {EventType} (Id: {EventId}) to handler {HandlerType}",
                    eventType.Name, domainEvent.EventId, handler?.GetType().Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error dispatching event {EventType} (Id: {EventId}) to handler {HandlerType}",
                    eventType.Name, domainEvent.EventId, handler?.GetType().Name);
                throw;
            }
        }
    }

    public async Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken ct = default)
    {
        foreach (var domainEvent in domainEvents)
        {
            await DispatchAsync(domainEvent, ct);
        }
    }
}
