using Frank.Abstractions.ImmutableContextBuilder;

namespace Frank.Abstractions.Authentication.Callback;

public sealed record FrankAuthCallbackResult : ImmutableContextBuilderResultBase
{
    public required string SubjectId { get; init; }
    public required IReadOnlyDictionary<string, string> Claims { get; init; }

    public string? Email { get; init; }
    public string? GivenName { get; init; }
    public string? FamilyName { get; init; }
    public string? Picture { get; init; }

    public string Provider { get; init; } = "unknown";
}
