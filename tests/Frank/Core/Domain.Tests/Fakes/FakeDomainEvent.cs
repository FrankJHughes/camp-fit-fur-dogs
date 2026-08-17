namespace Frank.Core.Domain.Tests.Fakes;

public sealed record FakeDomainEvent(string Message) : IDomainEvent;
