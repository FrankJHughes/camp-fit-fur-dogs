using Frank.Core.Application.Abstractions.ImmutableContexts;

namespace Frank.Identity.Application.Abstractions.Callback.Oidc;

public record OidcCallbackContextBuilderRequest : ImmutableContextBuilderRequestBase
{
    public required string Code { get; init; }
}
