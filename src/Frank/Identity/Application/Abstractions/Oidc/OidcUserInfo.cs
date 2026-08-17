namespace Frank.Identity.Application.Abstractions.Oidc;

/// <summary>
/// Represents the profile information returned by an upstream OpenID Connect
/// (OIDC) provider’s UserInfo endpoint.
/// <para>
/// The UserInfo response supplements the identity information found in the ID
/// token and typically includes fields such as <c>email</c>, <c>given_name</c>,
/// <c>family_name</c>, and <c>picture</c>.
/// Providers may also return additional custom claims, which are captured in the
/// <see cref="Claims"/> dictionary.
/// </para>
/// <para>
/// This model is used by the OIDC callback pipeline to enrich the normalized
/// identity context before it is passed into the application‑level save phase.
/// </para>
/// </summary>
/// <param name="Subject">
/// The subject identifier (<c>sub</c>) representing the authenticated user in the
/// upstream OIDC provider.
/// This value uniquely identifies the user within the provider’s identity domain.
/// </param>
/// <param name="Email">
/// The user’s email address, if provided by the UserInfo endpoint.
/// </param>
/// <param name="GivenName">
/// The user’s given name (first name), if provided.
/// </param>
/// <param name="FamilyName">
/// The user’s family name (last name), if provided.
/// </param>
/// <param name="Picture">
/// A URL pointing to the user’s profile picture, if supplied by the provider.
/// </param>
/// <param name="Claims">
/// A dictionary of additional claims returned by the UserInfo endpoint.
/// These may include standard OIDC profile fields or provider‑specific custom
/// attributes.
/// Claim keys and values are normalized into a simple string‑to‑string mapping.
/// </param>
public sealed record OidcUserInfo(
    string Subject,
    string? Email,
    string? GivenName,
    string? FamilyName,
    string? Picture,
    IReadOnlyDictionary<string, string> Claims
);
