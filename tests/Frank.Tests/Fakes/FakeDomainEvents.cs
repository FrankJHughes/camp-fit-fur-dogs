using Frank.Abstractions.Events;

namespace Frank.Tests.Fakes;

public sealed class FakeDomainEventHandler1 : IEventHandler<FakeDomainEvent>
{
    public Task HandleAsync(FakeDomainEvent domainEvent, CancellationToken ct) =>
        Task.CompletedTask;
}

public sealed class FakeDomainEventHandler2 : IEventHandler<FakeDomainEvent>
{
    public Task HandleAsync(FakeDomainEvent domainEvent, CancellationToken ct) =>
        Task.CompletedTask;
}

public abstract class AbstractFakeDomainEventHandler : IEventHandler<FakeDomainEvent>
{
    public abstract Task HandleAsync(FakeDomainEvent domainEvent, CancellationToken ct);
}
