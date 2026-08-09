namespace Frank.Identity.Application.Abstractions.Oidc;

/// <summary>
/// Defines the contract for retrieving UserInfo data from an upstream
/// OpenID Connect (OIDC) provider.
/// <para>
/// The OIDC UserInfo endpoint returns additional profile information about the
/// authenticated user, typically including fields such as <c>email</c>,
/// <c>given_name</c>, <c>family_name</c>, and <c>picture</c>.
/// Implementations of <see cref="IOidcUserInfoClient"/> are responsible for
/// calling this endpoint using a valid access token and returning the provider’s
/// response in a structured model.
/// </para>
/// <para>
/// This abstraction isolates protocol‑level HTTP and provider‑specific behavior
/// from the Identity application pipeline, enabling deterministic and
/// provider‑agnostic OIDC interactions.
/// </para>
/// </summary>
public interface IOidcUserInfoClient
{
    /// <summary>
    /// Retrieves UserInfo data from the upstream OIDC provider using the supplied
    /// access token.
    /// <para>
    /// The access token must be issued by the provider’s token endpoint and must
    /// include the <c>openid</c> and <c>profile</c> scopes (or equivalent) for the
    /// UserInfo endpoint to return meaningful identity information.
    /// </para>
    /// <para>
    /// Implementations must handle provider‑specific error responses, token
    /// validation rules, and HTTP‑level concerns.
    /// The method is asynchronous and supports cancellation via
    /// <paramref name="ct"/>.
    /// </para>
    /// </summary>
    /// <param name="accessToken">
    /// The access token issued by the upstream OIDC provider.
    /// This token authorizes the request to the UserInfo endpoint.
    /// </param>
    /// <param name="ct">
    /// A cancellation token that allows the caller to cancel the UserInfo
    /// retrieval operation.
    /// </param>
    /// <returns>
    /// A structured <see cref="OidcUserInfo"/> containing the profile information
    /// returned by the upstream OIDC provider.
    /// </returns>
    Task<OidcUserInfo> GetUserInfoAsync(string accessToken, CancellationToken ct);
}
