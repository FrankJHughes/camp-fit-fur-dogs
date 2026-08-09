#nullable enable

namespace Frank.Identity.Api.Abstractions.Endpoints;

/// <summary>
/// Represents the response returned by the <c>GetIdentity</c> endpoint.
/// <para>
/// This DTO exposes only the caller's resolved identity information and
/// intentionally contains no domain logic, no authentication tokens, and
/// no provider‑specific details.
/// It aligns with the Identity purity rules described in the authentication
/// stories, ensuring that identity information is surfaced safely and predictably
/// to API consumers.
/// </para>
/// <para>
/// Typical usage includes returning the authenticated user's display name
/// after a successful OIDC login flow, consistent with the acceptance criteria
/// in the authentication stories.
/// </para>
/// </summary>
/// <remarks>
/// This response type must remain stable and minimal:
/// <list type="bullet">
/// <item><description>No session tokens or provider tokens may be included (US‑110).</description></item>
/// <item><description>No domain logic may be embedded in identity endpoints (US‑111).</description></item>
/// <item><description>Only resolved identity attributes safe for client consumption should appear.</description></item>
/// </list>
/// </remarks>
public sealed class GetIdentityEndpointResponse
{
    /// <summary>
    /// The resolved display name of the authenticated user.
    /// <para>
    /// This value is derived from the identity provider and represents the
    /// human‑readable name associated with the authenticated session.
    /// </para>
    /// </summary>
    public string Name { get; init; } = default!;
}
