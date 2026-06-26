using Frank.Abstractions.Authentication.Callback;
using Frank.Abstractions.ImmutableContextBuilder;

namespace CampFitFurDogs.Application.Abstractions.Authentication.Callback;

public sealed record ApplicationAuthCallbackRequest : ImmutableContextBuilderRequestBase
{
    /// <summary>
    /// The external identity resolved by the protocol layer (Frank).
    /// </summary>
    public required FrankAuthCallbackResult External { get; init; }

    /// <summary>
    /// Optional return URL requested by the client.
    /// </summary>
    public string? RequestedRedirectUrl { get; init; }

    /// <summary>
    /// Timestamp captured at the start of the application pipeline.
    /// </summary>
    public required DateTimeOffset Now { get; init; }
}
