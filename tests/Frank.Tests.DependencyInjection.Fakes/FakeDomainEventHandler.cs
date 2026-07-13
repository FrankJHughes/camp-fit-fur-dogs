using Frank.Core.Application.Abstractions.DomainEvents;

namespace Frank.Tests.DependencyInjection.Fakes;

public sealed class FakeDomainEventHandler
    : IDomainEventHandler<FakeDomainEvent>
{
    public Task HandleAsync(FakeDomainEvent domainEvent, CancellationToken ct)
        => Task.CompletedTask;
}
