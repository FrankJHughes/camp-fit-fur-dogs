using Frank.Core.Application.Abstractions.ImmutableContexts;

namespace Frank.Identity.Application.Abstractions.Callback.Oidc;

/// <summary>
/// Represents the immutable context produced during an OIDC callback flow.
/// <para>
/// This context captures all relevant protocol inputs, token‑exchange results,
/// identity claims, and UserInfo metadata returned by the upstream OIDC provider.
/// It is passed through the Identity application pipeline as a stable,
/// read‑only snapshot of the callback state.
/// </para>
/// </summary>
/// <remarks>
/// This record inherits from <see cref="ImmutableContextBase"/>, ensuring that
/// all values are immutable once constructed.
/// <para>
/// The context does not depend on any system clock or time source.
/// The caller is responsible for supplying a timestamp obtained from the
/// application’s clock abstraction, ensuring deterministic and testable
/// time‑dependent behavior throughout the pipeline.
/// </para>
/// <para>
/// The context is intentionally provider‑agnostic except for the explicit
/// <see cref="Provider"/> property, which identifies the upstream identity
/// provider used for the callback.
/// </para>
/// </remarks>
public sealed record CallbackOidcContext : ImmutableContextBase
{
    //
    // OIDC protocol inputs
    //

    /// <summary>
    /// The authorization code returned by the upstream OIDC provider during the
    /// callback phase.
    /// This code is exchanged for tokens at the provider’s token endpoint.
    /// </summary>
    public required string Code { get; init; }

    /// <summary>
    /// The timestamp representing the moment the callback was processed.
    /// <para>
    /// This value must be supplied by the caller using the application’s clock
    /// abstraction (e.g., <c>clock.UtcNow</c>).
    /// Capturing the timestamp externally ensures deterministic, testable, and
    /// replayable time‑based behavior throughout the Identity pipeline.
    /// </para>
    /// </summary>
    public required DateTimeOffset Timestamp { get; init; }

    //
    // Token exchange results
    //

    /// <summary>
    /// The access token returned by the provider’s token endpoint, if available.
    /// <para>
    /// This token may be used to call the provider’s UserInfo endpoint or other
    /// protected upstream APIs.
    /// </para>
    /// </summary>
    public string? AccessToken { get; init; }

    /// <summary>
    /// The ID token returned by the provider’s token endpoint, if available.
    /// <para>
    /// This token typically contains identity claims about the authenticated user
    /// and may be validated by downstream components.
    /// </para>
    /// </summary>
    public string? IdToken { get; init; }

    //
    // User identity from OIDC
    //

    /// <summary>
    /// The subject identifier (<c>sub</c>) representing the user in the upstream
    /// OIDC provider.
    /// This value uniquely identifies the user within the provider’s identity
    /// domain.
    /// </summary>
    public string? SubjectId { get; init; }

    /// <summary>
    /// A dictionary of claims extracted from the ID token or other OIDC sources.
    /// <para>
    /// Claim keys and values are provider‑specific and may include fields such as
    /// <c>email</c>, <c>given_name</c>, <c>family_name</c>, or custom attributes.
    /// </para>
    /// </summary>
    public IReadOnlyDictionary<string, string>? Claims { get; init; }

    //
    // UserInfo endpoint results
    //

    /// <summary>
    /// The user’s email address as returned by the provider’s UserInfo endpoint,
    /// if available.
    /// </summary>
    public string? Email { get; init; }

    /// <summary>
    /// The user’s given name (first name) from the UserInfo endpoint.
    /// </summary>
    public string? GivenName { get; init; }

    /// <summary>
    /// The user’s family name (last name) from the UserInfo endpoint.
    /// </summary>
    public string? FamilyName { get; init; }

    /// <summary>
    /// A URL pointing to the user’s profile picture, if provided by the upstream
    /// identity provider.
    /// </summary>
    public string? Picture { get; init; }

    //
    // Provider metadata
    //

    /// <summary>
    /// Identifies the upstream OIDC provider used for this callback.
    /// <para>
    /// This implementation currently assumes <c>auth0</c> as the provider.
    /// Downstream components may use this value for provider‑specific logic,
    /// mapping, or diagnostics.
    /// </para>
    /// </summary>
    public string Provider => "auth0";
}
