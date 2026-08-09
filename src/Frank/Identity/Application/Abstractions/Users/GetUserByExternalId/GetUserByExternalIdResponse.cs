namespace Frank.Identity.Application.Abstractions.Users.GetUserByExternalId;

/// <summary>
/// Represents the application‑layer response returned when resolving a user
/// by their external identity provider identifier.
/// <para>
/// This response is intentionally minimal: it exposes only the internal
/// <see cref="Guid"/> identifier of the resolved user.
/// The external identity provider (e.g., OIDC subject) is used solely for lookup
/// and is not exposed in the response.
/// </para>
/// <para>
/// This model is used by authentication and onboarding flows that need to map an
/// external identity to an internal user record.
/// </para>
/// </summary>
/// <param name="Id">
/// The unique internal identifier of the user associated with the provided
/// external identity provider ID.
/// </param>
public sealed record GetUserByExternalIdResponse(Guid Id);
