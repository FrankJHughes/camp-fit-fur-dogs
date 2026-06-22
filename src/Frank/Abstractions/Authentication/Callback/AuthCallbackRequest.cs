using Frank.Abstractions.ImmutableContextBuilder;

namespace Frank.Abstractions.Authentication.Callback;

public record FrankAuthCallbackRequest : ImmutableContextBuilderRequestBase
{
    public required string Code { get; init; }
}
