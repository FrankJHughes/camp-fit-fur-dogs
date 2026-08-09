using Frank.Identity.Application.Abstractions.Callback.Oidc;

namespace Frank.Identity.Application.Abstractions.Users;

/// <summary>
/// Resolves the internal user associated with an OIDC authentication callback.
/// <para>
/// This abstraction represents the final step of the OIDC login pipeline:
/// mapping an external identity provider subject (and related claims) to an
/// internal user record.
/// </para>
/// <para>
/// The resolver ensures that the authentication callback result is transformed
/// into a stable internal <see cref="Guid"/> user identifier, which is then used
/// for session creation and authorization flows.
/// </para>
/// </summary>
/// <remarks>
/// Implementations typically perform the following steps:
/// <list type="bullet">
/// <item><description>
/// Extract the external identity provider subject (e.g., OIDC <c>sub</c> claim)
/// from <see cref="CallbackOidcContextBuilderResult"/></description></item>
/// <item><description>
/// Attempt to resolve an existing user via <c>IGetUserByExternalIdReader</c></description></item>
/// <item><description>
/// If no user exists, create a new one via <c>CreateUserCommand</c> + <c>ICreateUserWriter</c>.</description></item>
/// <item><description>
/// Return the internal user ID for downstream session creation.
/// </description></item>
/// </list>
/// The resolver encapsulates this logic so that the OIDC callback handler remains
/// clean, deterministic, and infrastructure‑agnostic.
/// </remarks>
public interface IUserResolver
{
    /// <summary>
    /// Resolves the internal user associated with the provided OIDC callback
    /// result.
    /// <para>
    /// If the external identity provider subject is already mapped to an
    /// existing user, that user’s ID is returned.
    /// Otherwise, a new user is provisioned and its ID returned.
    /// </para>
    /// </summary>
    /// <param name="authCallbackResult">
    /// The processed OIDC callback context containing validated claims from the
    /// external identity provider.
    /// </param>
    /// <param name="cancellationToken">
    /// A token allowing the caller to cancel the resolution operation.
    /// </param>
    /// <returns>
    /// The internal <see cref="Guid"/> identifier of the resolved or newly
    /// created user.
    /// </returns>
    Task<Guid> ResolveAsync(
        CallbackOidcContextBuilderResult authCallbackResult,
        CancellationToken cancellationToken);
}
