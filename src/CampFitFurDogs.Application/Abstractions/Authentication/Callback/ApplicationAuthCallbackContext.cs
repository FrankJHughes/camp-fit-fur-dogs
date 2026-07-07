using Frank.Abstractions.Authentication.Callback;
using Frank.Abstractions.ImmutableContext;

namespace CampFitFurDogs.Application.Abstractions.Authentication.Callback;

public sealed record ApplicationAuthCallbackContext : ImmutableContextBase
{
    //
    // Immutable inputs
    //
    public required FrankAuthCallbackResult External { get; init; }
    public required DateTimeOffset Now { get; init; }
    public Guid? CustomerId { get; init; }
    public Guid? SessionId { get; init; }
    public string? TokenHash { get; init; }
    public string? CookieValue { get; init; }
}
