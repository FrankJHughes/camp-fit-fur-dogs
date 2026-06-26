using Frank.Abstractions.Events;

namespace Frank.Tests.DependencyInjection.Fakes;

public sealed class AnotherFakeDomainEventHandler : IEventHandler<FakeDomainEvent>
{
    public Task HandleAsync(FakeDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
