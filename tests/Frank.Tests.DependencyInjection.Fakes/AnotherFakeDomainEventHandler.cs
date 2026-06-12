using Frank.Abstractions.Events;
using Frank.Events;

namespace Frank.Tests.DependencyInjection.Fakes;

public sealed class AnotherFakeDomainEventHandler : IDomainEventHandler<FakeDomainEvent>
{
    public Task HandleAsync(FakeDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
