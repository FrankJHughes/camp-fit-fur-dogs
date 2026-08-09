using Frank.Core.Application.Abstractions.ImmutableContexts;
using Frank.Identity.Application.Abstractions.Callback.Oidc;
using Frank.Identity.Application.Abstractions.Oidc;

namespace Frank.Identity.Application.Callback.Oidc.Steps;

/// <summary>
/// Represents the OIDC pipeline step responsible for validating the ID token
/// returned by the identity provider.
/// <para>
/// This step executes only when <see cref="CallbackOidcContext.IdToken"/> is
/// present. It uses the configured <see cref="IOidcTokenValidator"/> to perform
/// cryptographic and structural validation of the ID token.
/// </para>
/// <para>
/// Upon successful validation, the step enriches the immutable callback context
/// with the subject identifier and validated claims extracted from the token.
/// </para>
/// <para>
/// The step is deterministic and side‑effect free aside from the validation
/// operation, returning a new <see cref="CallbackOidcContext"/> instance.
/// </para>
/// </summary>
public sealed class ValidateTokensStep : IImmutableContextBuildStep<CallbackOidcContext>
{
    private readonly IOidcTokenValidator _validator;

    /// <summary>
    /// Metadata describing this build step, including its unique identifier and
    /// human‑readable description. Used by pipeline diagnostics and observability.
    /// </summary>
    public IImmutableContextBuildStepMetadata Metadata { get; } =
        new ImmutableContextBuildStepMetadata("oidc.validate-tokens", "Validate ID Token");

    /// <summary>
    /// Creates a new <see cref="ValidateTokensStep"/> using the provided OIDC
    /// token validator.
    /// </summary>
    /// <param name="validator">
    /// The validator responsible for verifying the ID token's signature,
    /// issuer, audience, expiration, and claims.
    /// </param>
    public ValidateTokensStep(IOidcTokenValidator validator)
    {
        _validator = validator;
    }

    /// <summary>
    /// Determines whether this step can execute based on the current callback
    /// context.
    /// <para>
    /// This step executes only when an ID token is present, since validation
    /// requires a non‑null token.
    /// </para>
    /// </summary>
    /// <param name="ctx">The current immutable callback context.</param>
    /// <returns>
    /// <c>true</c> if <see cref="CallbackOidcContext.IdToken"/> is not null;
    /// otherwise <c>false</c>.
    /// </returns>
    public bool CanExecute(CallbackOidcContext ctx)
        => ctx.IdToken is not null;

    /// <summary>
    /// Validates the ID token and enriches the callback context with the
    /// validated subject identifier and claims.
    /// <para>
    /// The returned context is a new immutable instance containing:
    /// </para>
    /// <list type="bullet">
    /// <item><description>The validated subject identifier</description></item>
    /// <item><description>The validated claim set</description></item>
    /// </list>
    /// </summary>
    /// <param name="ctx">The current immutable callback context.</param>
    /// <param name="ct">A cancellation token for the asynchronous operation.</param>
    /// <returns>
    /// A new <see cref="CallbackOidcContext"/> enriched with validated token data.
    /// </returns>
    public async Task<CallbackOidcContext> ExecuteAsync(
        CallbackOidcContext ctx,
        CancellationToken ct)
    {
        var result = await _validator.ValidateAsync(ctx.IdToken!, ct);

        return ctx with
        {
            SubjectId = result.SubjectId,
            Claims = result.Claims
        };
    }
}
