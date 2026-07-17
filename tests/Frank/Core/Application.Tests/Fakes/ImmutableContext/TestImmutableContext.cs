using Frank.Core.Application.Abstractions.ImmutableContexts;

namespace Frank.Core.Application.Tests.Fakes.ImmutableContext;

public sealed record TestImmutableContext : ImmutableContextBase
{
    public string Code { get; init; } = null!;
    public DateTimeOffset Now { get; init; }
}

