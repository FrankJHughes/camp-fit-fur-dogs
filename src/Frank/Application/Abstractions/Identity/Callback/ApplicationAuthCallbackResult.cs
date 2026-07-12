using Frank.Abstractions.ImmutableContext;

namespace Frank.Application.Abstractions.Identity.Callback;

public sealed record ApplicationAuthCallbackContextBuilderResult : ImmutableContextBuilderResultBase
{
    public required Guid UserId { get; init; }
    public required Guid SessionId { get; init; }

    public required string TokenHash { get; init; }
    public required string CookieValue { get; init; }
}
