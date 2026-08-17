namespace Frank.Identity.Application.Abstractions.Oidc;

/// <summary>
/// Defines the contract for performing an authorization‑code exchange against an
/// upstream OpenID Connect (OIDC) provider.
/// <para>
/// Implementations of <see cref="IOidcTokenClient"/> are responsible for calling
/// the provider’s token endpoint, submitting the authorization code, and
/// returning the resulting tokens in a structured response model.
/// </para>
/// <para>
/// This abstraction isolates protocol‑level concerns from the Identity
/// application pipeline, enabling deterministic, testable, and provider‑agnostic
/// OIDC interactions.
/// </para>
/// </summary>
public interface IOidcTokenClient
{
    /// <summary>
    /// Exchanges an OIDC authorization code for tokens at the provider’s token
    /// endpoint.
    /// <para>
    /// The returned <see cref="OidcTokenResponse"/> contains the access token
    /// (always present if the exchange succeeds) and the ID token (present only
    /// when the provider issues one).
    /// </para>
    /// <para>
    /// Implementations must handle provider‑specific error responses, token
    /// validation rules, and HTTP‑level concerns.
    /// The method is asynchronous and supports cancellation via
    /// <paramref name="ct"/>.
    /// </para>
    /// </summary>
    /// <param name="code">
    /// The authorization code received during the OIDC callback flow.
    /// This value is exchanged for tokens at the provider’s token endpoint.
    /// </param>
    /// <param name="ct">
    /// A cancellation token that allows the caller to cancel the token‑exchange
    /// operation.
    /// </param>
    /// <returns>
    /// A structured <see cref="OidcTokenResponse"/> containing the tokens issued
    /// by the upstream OIDC provider.
    /// </returns>
    Task<OidcTokenResponse> ExchangeCodeAsync(string code, CancellationToken ct);
}

/// <summary>
/// Represents the token response returned by an upstream OIDC provider after a
/// successful authorization‑code exchange.
/// <para>
/// The access token is always present when the exchange succeeds.
/// The ID token may be absent depending on provider configuration, scopes, or
/// token‑endpoint behavior.
/// </para>
/// </summary>
/// <param name="AccessToken">
/// The access token issued by the provider.
/// This token may be used to call the UserInfo endpoint or other protected
/// upstream APIs.
/// </param>
/// <param name="IdToken">
/// The ID token issued by the provider, if available.
/// This token typically contains identity claims about the authenticated user.
/// </param>
public sealed record OidcTokenResponse(
    string AccessToken,
    string? IdToken);
