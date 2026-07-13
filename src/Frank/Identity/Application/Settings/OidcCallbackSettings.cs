namespace Frank.Identity.Application.Settings;

public sealed class OidcCallbackSettings
{
    public required string Authority { get; init; }
    public required string ClientId { get; init; }
    public required string ClientSecret { get; init; }
    public required string CallbackUrl { get; init; }
}
