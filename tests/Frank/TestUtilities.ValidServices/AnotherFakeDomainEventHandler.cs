using Frank.Core.Application.Abstractions.DomainEvents;

namespace Frank.TestUtilities.ValidServices;

public sealed class AnotherFakeDomainEventHandler : IDomainEventHandler<FakeDomainEvent>
{
    public Task HandleAsync(FakeDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
