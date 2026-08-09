using Frank.Core.Application.Abstractions.ImmutableContexts;

namespace Frank.Identity.Application.Abstractions.Callback.Oidc;

/// <summary>
/// Represents the immutable result produced by the OIDC callback context builder.
/// <para>
/// This result contains all identity information extracted from the upstream
/// OIDC provider after token exchange and optional UserInfo retrieval.
/// It is used to populate a <see cref="CallbackOidcContext"/> with stable,
/// provider‑normalized identity data.
/// </para>
/// </summary>
/// <remarks>
/// This record inherits from <see cref="ImmutableContextBuilderResultBase"/>,
/// ensuring that all values are immutable once constructed.
/// Builder‑result types represent the *post‑processing* stage of the callback
/// pipeline, where upstream identity information has been validated, normalized,
/// and prepared for inclusion in the final immutable context.
/// </remarks>
public sealed record CallbackOidcContextBuilderResult : ImmutableContextBuilderResultBase
{
    /// <summary>
    /// The subject identifier (<c>sub</c>) representing the authenticated user
    /// within the upstream OIDC provider.
    /// This value uniquely identifies the user in the provider’s identity domain
    /// and is the primary key for correlating upstream identity with local
    /// application identity.
    /// </summary>
    public required string SubjectId { get; init; }

    /// <summary>
    /// A dictionary of identity claims extracted from the ID token or other
    /// provider‑specific sources.
    /// <para>
    /// Claims may include standard OIDC fields (e.g., <c>email</c>,
    /// <c>given_name</c>, <c>family_name</c>) as well as provider‑specific or
    /// custom attributes.
    /// The builder is responsible for normalizing these claims into a consistent
    /// key/value representation.
    /// </para>
    /// </summary>
    public required IReadOnlyDictionary<string, string> Claims { get; init; }

    /// <summary>
    /// The user’s email address as returned by the provider’s UserInfo endpoint,
    /// if available.
    /// This value may duplicate the <c>email</c> claim but is kept separate to
    /// reflect the distinct OIDC data sources.
    /// </summary>
    public string? Email { get; init; }

    /// <summary>
    /// The user’s given name (first name) as returned by the provider’s UserInfo
    /// endpoint, if available.
    /// </summary>
    public string? GivenName { get; init; }

    /// <summary>
    /// The user’s family name (last name) as returned by the provider’s UserInfo
    /// endpoint, if available.
    /// </summary>
    public string? FamilyName { get; init; }

    /// <summary>
    /// A URL pointing to the user’s profile picture, if provided by the upstream
    /// identity provider.
    /// This value is typically sourced from the UserInfo endpoint.
    /// </summary>
    public string? Picture { get; init; }

    /// <summary>
    /// Identifies the upstream OIDC provider that produced this identity data.
    /// <para>
    /// This value defaults to <c>"unknown"</c> but may be set by the builder
    /// based on provider‑specific metadata or configuration.
    /// Downstream components may use this value for diagnostics, mapping, or
    /// provider‑specific logic.
    /// </para>
    /// </summary>
    public string Provider { get; init; } = "unknown";
}
