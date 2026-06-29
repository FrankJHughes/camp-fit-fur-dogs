using Frank.Abstractions.Event;

namespace Frank.Tests.Fakes;

public sealed record FakeDomainEvent(string Message) : IEvent;
