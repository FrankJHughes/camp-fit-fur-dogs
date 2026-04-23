using SharedKernel.Events;

namespace SharedKernel.Tests.DependencyInjection.Fakes;

public sealed class AnotherFakeDomainEventHandler : IDomainEventHandler<FakeDomainEvent>
{
    public Task Handle(FakeDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
