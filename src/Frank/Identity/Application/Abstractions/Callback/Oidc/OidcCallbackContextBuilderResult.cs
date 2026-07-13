using Frank.Core.Application.Abstractions.ImmutableContext;

namespace Frank.Identity.Application.Abstractions.Callback.Oidc;

public sealed record OidcCallbackContextBuilderResult : ImmutableContextBuilderResultBase
{
    public required string SubjectId { get; init; }
    public required IReadOnlyDictionary<string, string> Claims { get; init; }

    public string? Email { get; init; }
    public string? GivenName { get; init; }
    public string? FamilyName { get; init; }
    public string? Picture { get; init; }

    public string Provider { get; init; } = "unknown";
}
