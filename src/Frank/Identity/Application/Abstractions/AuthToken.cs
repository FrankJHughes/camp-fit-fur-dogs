namespace Frank.Identity.Application.Abstractions;

/// <summary>
/// Represents an access token issued by an external identity provider during the
/// OIDC authentication callback pipeline.
/// <para>
/// This model is intentionally minimal: it wraps only the raw access token
/// string.
/// The application layer uses this abstraction to pass the token safely between
/// components without exposing additional OIDC or HTTP details.
/// </para>
/// <para>
/// Infrastructure components (token validators, userinfo clients, etc.) may
/// consume this token to perform authorized calls to the identity provider.
/// </para>
/// </summary>
/// <param name="AccessToken">
/// The raw access token returned by the identity provider's token endpoint.
/// </param>
public sealed record AuthToken(string AccessToken);
