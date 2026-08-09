namespace Frank.Identity.Application.Abstractions;

/// <summary>
/// Represents the set of error conditions that may occur during the OIDC
/// authentication callback pipeline.
/// <para>
/// These errors describe failures in the multi‑step process of exchanging the
/// authorization code, retrieving tokens, fetching user information, and
/// constructing the final callback result.
/// </para>
/// <para>
/// The enum is intentionally explicit and granular, allowing upstream handlers
/// (e.g., <c>IUserResolver</c>, session creation, middleware) to react
/// deterministically to each failure mode.
/// </para>
/// </summary>
public enum AuthCallbackError
{
    /// <summary>
    /// The OIDC provider did not supply an authorization code.
    /// </summary>
    MissingAuthorizationCode,

    /// <summary>
    /// The OIDC client configuration is incomplete or invalid.
    /// </summary>
    IncompleteConfiguration,

    /// <summary>
    /// The token endpoint did not return an access token.
    /// </summary>
    MissingAccessToken,

    /// <summary>
    /// The userinfo endpoint request failed or returned invalid data.
    /// </summary>
    UserInfoFailure,

    /// <summary>
    /// The external identity provider did not supply a subject identifier.
    /// </summary>
    MissingExternalId,

    /// <summary>
    /// The callback result could not be constructed due to missing or invalid
    /// intermediate data.
    /// </summary>
    MissingResult
}
