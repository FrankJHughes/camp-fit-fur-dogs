#nullable enable

namespace Frank.Identity.Api.Abstractions.Endpoints;

/// <summary>
/// Represents the response returned by the <c>Logout</c> endpoint.
/// <para>
/// This DTO provides the next URL that the client must navigate to in order to
/// complete the logout flow with the external identity provider (OIDC).
/// It intentionally contains no authentication tokens, no session identifiers,
/// and no provider‑specific metadata, in alignment with the Identity purity rules
/// described in authentication stories such as US‑110, US‑111, and US‑133.
/// </para>
/// </summary>
/// <remarks>
/// This response type must remain minimal and safe for client consumption:
/// <list type="bullet">
/// <item><description>No identity provider tokens may be included.</description></item>
/// <item><description>No session state or domain logic may be embedded.</description></item>
/// <item><description>Only the redirect URL required to continue the logout flow is returned.</description></item>
/// </list>
/// </remarks>
public sealed record LogoutEndpointResponse(string NextUrl);
