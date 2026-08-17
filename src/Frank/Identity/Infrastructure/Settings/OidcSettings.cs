namespace Frank.Identity.Infrastructure.Settings;

/// <summary>
/// Represents the configuration settings required to integrate with an
/// OpenID Connect (OIDC) identity provider such as Auth0.
/// <para>
/// These settings define the authority, client credentials, and callback URL
/// used during the authorization‑code flow, token exchange, token validation,
/// and userinfo retrieval.
/// </para>
/// <para>
/// All properties are marked as <c>required</c> to ensure that the application
/// fails fast during startup if OIDC configuration is missing or incomplete.
/// </para>
/// </summary>
public sealed class OidcSettings
{
    /// <summary>
    /// The base authority URL of the OIDC provider.
    /// <para>
    /// Typically in the form <c>https://tenant.auth0.com</c> or another
    /// provider‑specific issuer URL.
    /// </para>
    /// </summary>
    public required string Authority { get; init; }

    /// <summary>
    /// The OIDC client identifier registered with the identity provider.
    /// <para>
    /// Used during authorization‑code exchange, token validation, and userinfo
    /// retrieval.
    /// </para>
    /// </summary>
    public required string ClientId { get; init; }

    /// <summary>
    /// The OIDC client secret associated with the configured client identifier.
    /// <para>
    /// Used when exchanging authorization codes for tokens.
    /// Must be stored securely and never logged.
    /// </para>
    /// </summary>
    public required string ClientSecret { get; init; }

    /// <summary>
    /// The callback URL registered with the OIDC provider.
    /// <para>
    /// This must match the redirect URI configured in Auth0 or the identity
    /// provider. It is used during the authorization‑code flow to receive the
    /// authorization code before token exchange.
    /// </para>
    /// </summary>
    public required string CallbackUrl { get; init; }
}
