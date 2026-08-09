#nullable enable

namespace Frank.Identity.Api.Abstractions.Endpoints;

/// <summary>
/// Represents the response returned by the <c>GetLoginUrl</c> endpoint.
/// <para>
/// This DTO provides the next URL that the client must navigate to in order to
/// begin the external identity provider login flow (OIDC).
/// It intentionally contains no authentication tokens, no provider‑specific
/// metadata, and no domain logic, in accordance with the Identity purity rules
/// described in authentication stories such as US‑110 and US‑111.
/// </para>
/// </summary>
/// <remarks>
/// This response type must remain minimal and safe for client consumption:
/// <list type="bullet">
/// <item><description>No identity provider tokens may be included (US‑110).</description></item>
/// <item><description>No domain logic or session state may be embedded (US‑111).</description></item>
/// <item><description>Only the redirect URL required to continue the login flow is returned.</description></item>
/// </list>
/// </remarks>
public sealed record GetLoginUrlEndpointResponse
(
    string NextUrl
);
