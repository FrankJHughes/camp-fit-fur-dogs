using SharedKernel.Events;

namespace SharedKernel.Tests.Fakes;

public sealed class FakeDomainEventHandler1 : IDomainEventHandler<FakeDomainEvent>
{
    public Task HandleAsync(FakeDomainEvent domainEvent, CancellationToken ct) =>
        Task.CompletedTask;
}

public sealed class FakeDomainEventHandler2 : IDomainEventHandler<FakeDomainEvent>
{
    public Task HandleAsync(FakeDomainEvent domainEvent, CancellationToken ct) =>
        Task.CompletedTask;
}

public abstract class AbstractFakeDomainEventHandler : IDomainEventHandler<FakeDomainEvent>
{
    public abstract Task HandleAsync(FakeDomainEvent domainEvent, CancellationToken ct);
}
