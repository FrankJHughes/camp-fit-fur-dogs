using Frank.Core.Application.Abstractions.ImmutableContext;

namespace Frank.Identity.Application.Abstractions.Callback.Oidc;

public record OidcCallbackContextBuilderRequest : ImmutableContextBuilderRequestBase
{
    public required string Code { get; init; }
}
