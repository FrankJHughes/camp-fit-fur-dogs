using Frank.Abstractions.Event;

namespace Frank.Tests.DependencyInjection.Fakes;

public sealed class FakeDomainEventHandler
    : IEventHandler<FakeDomainEvent>
{
    public Task HandleAsync(FakeDomainEvent domainEvent, CancellationToken ct)
        => Task.CompletedTask;
}
