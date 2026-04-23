using SharedKernel.Events;

namespace SharedKernel.Tests.Fakes;

public sealed record FakeDomainEvent(string Message) : IDomainEvent;
