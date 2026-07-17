using Frank.Core.Application.Abstractions.ImmutableContexts;

namespace Frank.Identity.Application.Abstractions.Callback.Save;

public sealed record SaveCallbackContextBuilderResult : ImmutableContextBuilderResultBase
{
    public required Guid UserId { get; init; }
    public required Guid SessionId { get; init; }

    public required string TokenHash { get; init; }
    public required string CookieValue { get; init; }
}
