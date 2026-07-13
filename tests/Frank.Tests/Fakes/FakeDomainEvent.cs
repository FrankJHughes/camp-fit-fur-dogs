using Frank.Core.Domain;

namespace Frank.Tests.Fakes;

public sealed record FakeDomainEvent(string Message) : IDomainEvent;
