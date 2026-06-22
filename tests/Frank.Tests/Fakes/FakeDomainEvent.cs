using Frank.Abstractions.Events;

namespace Frank.Tests.Fakes;

public sealed record FakeDomainEvent(string Message) : IDomainEvent;
