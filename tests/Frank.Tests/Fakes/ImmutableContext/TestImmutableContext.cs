using Frank.Abstractions.ImmutableContext;

namespace Frank.Tests.Fakes.ImmutableContext;

public sealed record TestImmutableContext : ImmutableContextBase
{
    public string Code { get; init; } = null!;
    public DateTimeOffset Now { get; init; }
}

