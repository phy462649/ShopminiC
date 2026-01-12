using LandingPageApp.Domain.Events;

namespace LandingPageApp.Application.Interfaces;

/// <summary>
/// Interface để dispatch domain events
/// </summary>
public interface IEventDispatcher
{
    /// <summary>
    /// Dispatch một event đến tất cả handlers đã đăng ký
    /// </summary>
    Task DispatchAsync(IDomainEvent domainEvent, CancellationToken ct = default);

    /// <summary>
    /// Dispatch nhiều events
    /// </summary>
    Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken ct = default);
}

/// <summary>
/// Interface cho event handler
/// </summary>
public interface IEventHandler<in TEvent> where TEvent : IDomainEvent
{
    Task HandleAsync(TEvent domainEvent, CancellationToken ct = default);
}
