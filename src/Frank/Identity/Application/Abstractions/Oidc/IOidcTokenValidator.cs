namespace Frank.Identity.Application.Abstractions.Oidc;

/// <summary>
/// Defines the contract for validating an OpenID Connect (OIDC) ID token issued
/// by an upstream identity provider.
/// <para>
/// Implementations of <see cref="IOidcTokenValidator"/> are responsible for
/// performing all required validation steps, which may include:
/// </para>
/// <list type="bullet">
/// <item><description>Signature verification</description></item>
/// <item><description>Issuer and audience validation</description></item>
/// <item><description>Nonce validation</description></item>
/// <item><description>Expiration and “not‑before” checks</description></item>
/// <item><description>Claim extraction and normalization</description></item>
/// </list>
/// <para>
/// This abstraction isolates protocol‑level token validation from the Identity
/// application pipeline, enabling deterministic, testable, and provider‑agnostic
/// OIDC behavior.
/// </para>
/// </summary>
public interface IOidcTokenValidator
{
    /// <summary>
    /// Validates an OIDC ID token and returns the extracted identity information
    /// if validation succeeds.
    /// <para>
    /// Implementations must reject invalid tokens and may throw provider‑specific
    /// or protocol‑specific exceptions when validation fails.
    /// The method is asynchronous and supports cancellation via
    /// <paramref name="ct"/>.
    /// </para>
    /// </summary>
    /// <param name="idToken">
    /// The raw ID token issued by the upstream OIDC provider.
    /// This value typically contains identity claims about the authenticated user.
    /// </param>
    /// <param name="ct">
    /// A cancellation token that allows the caller to cancel the validation
    /// operation.
    /// </param>
    /// <returns>
    /// A structured <see cref="OidcTokenValidationResult"/> containing the
    /// subject identifier and normalized claims extracted from the validated
    /// token.
    /// </returns>
    Task<OidcTokenValidationResult> ValidateAsync(string idToken, CancellationToken ct);
}

/// <summary>
/// Represents the result of validating an OIDC ID token.
/// <para>
/// A successful validation yields:
/// </para>
/// <list type="bullet">
/// <item><description>
/// The subject identifier (<c>sub</c>) uniquely identifying the user within the
/// upstream provider.
/// </description></item>
/// <item><description>
/// A normalized dictionary of claims extracted from the ID token.
/// </description></item>
/// </list>
/// <para>
/// This model is used by the OIDC callback pipeline to construct higher‑level
/// immutable contexts and to map upstream identity into local application
/// identity.
/// </para>
/// </summary>
/// <param name="SubjectId">
/// The subject identifier (<c>sub</c>) representing the authenticated user in the
/// upstream OIDC provider.
/// </param>
/// <param name="Claims">
/// A normalized dictionary of claims extracted from the validated ID token.
/// Claim keys and values are provider‑specific and may include standard OIDC
/// fields such as <c>email</c>, <c>given_name</c>, <c>family_name</c>, or custom
/// attributes.
/// </param>
public sealed record OidcTokenValidationResult(
    string SubjectId,
    IReadOnlyDictionary<string, string> Claims);
