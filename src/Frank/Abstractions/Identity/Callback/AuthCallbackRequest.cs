using Frank.Abstractions.ImmutableContext;

namespace Frank.Abstractions.Identity.Callback;

public record FrankAuthCallbackRequest : ImmutableContextBuilderRequestBase
{
    public required string Code { get; init; }
}
