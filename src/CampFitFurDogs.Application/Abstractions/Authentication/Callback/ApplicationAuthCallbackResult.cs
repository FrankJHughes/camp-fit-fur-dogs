using Frank.Abstractions.ImmutableContextBuilder;

namespace CampFitFurDogs.Application.Abstractions.Authentication.Callback;

public sealed record ApplicationAuthCallbackContextBuilderResult : ImmutableContextBuilderResultBase
{
    public required Guid CustomerId { get; init; }
    public required Guid SessionId { get; init; }

    public required string TokenHash { get; init; }
    public required string CookieValue { get; init; }
    public required string RedirectUrl { get; init; }
}
