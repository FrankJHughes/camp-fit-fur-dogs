using Frank.Core.Application.Abstractions.DomainEvents;

namespace Frank.TestUtilities.ValidServices;

public sealed class FakeDomainEventHandler
    : IDomainEventHandler<FakeDomainEvent>
{
    public Task HandleAsync(FakeDomainEvent domainEvent, CancellationToken ct)
        => Task.CompletedTask;
}
