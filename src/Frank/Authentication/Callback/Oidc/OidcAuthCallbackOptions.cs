namespace Frank.Authentication.Callback.Oidc;

public sealed class OidcAuthCallbackOptions
{
    public required string Authority { get; init; }
    public required string ClientId { get; init; }
    public required string ClientSecret { get; init; }
    public required string CallbackUrl { get; init; }
}
