using Frank.Abstractions.ImmutableContext;

namespace Frank.Authentication.Callback.Oidc;

public sealed record OidcAuthCallbackContext : ImmutableContextBase
{
    //
    // OIDC protocol inputs
    //
    public required string Code { get; init; }
    public required DateTimeOffset Now { get; init; }

    //
    // Token exchange results
    //
    public string? AccessToken { get; init; }
    public string? IdToken { get; init; }

    //
    // User identity from OIDC
    //
    public string? SubjectId { get; init; }
    public IReadOnlyDictionary<string, string>? Claims { get; init; }

    //
    // UserInfo endpoint results
    //
    public string? Email { get; init; }
    public string? GivenName { get; init; }
    public string? FamilyName { get; init; }
    public string? Picture { get; init; }

    //
    // Provider metadata
    //
    public string Provider => "auth0";
}
