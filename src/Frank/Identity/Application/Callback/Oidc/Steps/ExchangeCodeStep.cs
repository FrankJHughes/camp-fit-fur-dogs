using Frank.Core.Application.Abstractions.ImmutableContexts;
using Frank.Identity.Application.Abstractions.Callback.Oidc;
using Frank.Identity.Application.Abstractions.Oidc;


/// <summary>
/// Represents the OIDC pipeline step responsible for exchanging the authorization
/// code for tokens (access token and ID token).
/// <para>
/// This step executes only when the <see cref="CallbackOidcContext.Code"/> value
/// is present. It calls the configured <see cref="IOidcTokenClient"/> to perform
/// the token exchange and enriches the immutable callback context with the
/// returned tokens.
/// </para>
/// <para>
/// The step is deterministic, side‑effect free (aside from the token endpoint
/// call), and fits into the immutable context pipeline by returning a new
/// enriched <see cref="CallbackOidcContext"/> instance.
/// </para>
/// </summary>
public sealed class ExchangeCodeStep : IImmutableContextBuildStep<CallbackOidcContext>
{
    private readonly IOidcTokenClient _client;

    /// <summary>
    /// Metadata describing this build step, including its unique identifier and
    /// human‑readable description. Used by pipeline diagnostics and observability.
    /// </summary>
    public IImmutableContextBuildStepMetadata Metadata { get; } =
        new ImmutableContextBuildStepMetadata("oidc.exchange-code", "Exchange Authorization Code");

    /// <summary>
    /// Creates a new <see cref="ExchangeCodeStep"/> using the provided OIDC token
    /// client.
    /// </summary>
    /// <param name="client">
    /// The OIDC token client responsible for exchanging the authorization code
    /// for tokens.
    /// </param>
    public ExchangeCodeStep(IOidcTokenClient client)
    {
        _client = client;
    }

    /// <summary>
    /// Determines whether this step can execute based on the current callback
    /// context.
    /// <para>
    /// This step executes only when an authorization code is present.
    /// </para>
    /// </summary>
    /// <param name="ctx">The current immutable callback context.</param>
    /// <returns>
    /// <c>true</c> if <see cref="CallbackOidcContext.Code"/> is not null;
    /// otherwise <c>false</c>.
    /// </returns>
    public bool CanExecute(CallbackOidcContext ctx)
        => ctx.Code is not null;

    /// <summary>
    /// Exchanges the authorization code for tokens and enriches the callback
    /// context with the returned access token and ID token.
    /// <para>
    /// The returned context is a new immutable instance containing the updated
    /// token values.
    /// </para>
    /// </summary>
    /// <param name="ctx">The current immutable callback context.</param>
    /// <param name="ct">A cancellation token for the asynchronous operation.</param>
    /// <returns>
    /// A new <see cref="CallbackOidcContext"/> containing the exchanged tokens.
    /// </returns>
    public async Task<CallbackOidcContext> ExecuteAsync(
        CallbackOidcContext ctx,
        CancellationToken ct)
    {
        var tokens = await _client.ExchangeCodeAsync(ctx.Code!, ct);

        return ctx with
        {
            AccessToken = tokens.AccessToken,
            IdToken = tokens.IdToken
        };
    }
}
