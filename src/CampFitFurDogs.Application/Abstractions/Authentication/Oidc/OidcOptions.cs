namespace CampFitFurDogs.Application.Abstractions.Authentication.Oidc;

public sealed class OidcOptions
{
    public string Authority { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string CallbackUrl { get; set; } = string.Empty;
    public string PostLoginRedirectUrl { get; set; } = string.Empty;
}
