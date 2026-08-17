using Frank.Core.Application.Abstractions.ImmutableContexts;
using Frank.Identity.Application.Abstractions.Callback.Save;
using Frank.Identity.Application.Abstractions.Sessions;
using Frank.Identity.Domain.Sessions;

namespace Frank.Identity.Application.Callback.Save.Steps;

/// <summary>
/// Represents the pipeline step responsible for generating a new session token
/// and constructing the corresponding cookie value used for authentication.
/// <para>
/// This step executes only when <see cref="CallbackSaveContext.CookieValue"/> is
/// null, ensuring that cookie creation occurs exactly once during the save
/// pipeline, prior to session persistence.
/// </para>
/// <para>
/// The step generates a plaintext session token and its hashed representation,
/// constructs a secure cookie value using <see cref="SessionCookie"/>, and
/// returns a new immutable context containing these values.
/// </para>
/// </summary>
public sealed class BuildCookieStep
    : IImmutableContextBuildStep<CallbackSaveContext>
{
    private readonly ISessionTokenGenerator _tokens;

    /// <summary>
    /// Creates a new <see cref="BuildCookieStep"/> using the provided session
    /// token generator.
    /// </summary>
    /// <param name="tokens">
    /// The generator responsible for producing plaintext and hashed session
    /// tokens.
    /// </param>
    public BuildCookieStep(ISessionTokenGenerator tokens)
    {
        _tokens = tokens;
    }

    /// <summary>
    /// Metadata describing this pipeline step, including its unique identifier
    /// and human‑readable display name. Used by pipeline diagnostics and
    /// observability tooling.
    /// </summary>
    public IImmutableContextBuildStepMetadata Metadata =>
        new ImmutableContextBuildStepMetadata(
            id: "BuildCookie",
            displayName: "Build Cookie"
        );

    /// <summary>
    /// Determines whether this step can execute based on the current callback
    /// save context.
    /// <para>
    /// This step executes only when <see cref="CallbackSaveContext.CookieValue"/>
    /// is null, ensuring that cookie creation happens exactly once.
    /// </para>
    /// </summary>
    /// <param name="ctx">The current immutable callback save context.</param>
    /// <returns>
    /// <c>true</c> if <see cref="CallbackSaveContext.CookieValue"/> is null;
    /// otherwise <c>false</c>.
    /// </returns>
    public bool CanExecute(CallbackSaveContext ctx)
        => ctx.CookieValue is null;

    /// <summary>
    /// Generates a new session token, constructs the corresponding cookie value,
    /// and returns a new immutable context containing the token hash and cookie
    /// value.
    /// <para>
    /// The returned context includes:
    /// </para>
    /// <list type="bullet">
    /// <item><description>The hashed session token</description></item>
    /// <item><description>The cookie value derived from the plaintext token</description></item>
    /// </list>
    /// <para>
    /// This step does not perform any external side effects beyond token
    /// generation and maintains full immutability guarantees.
    /// </para>
    /// </summary>
    /// <param name="ctx">The current immutable callback save context.</param>
    /// <param name="ct">A cancellation token for the asynchronous operation.</param>
    /// <returns>
    /// A new <see cref="CallbackSaveContext"/> containing the generated token
    /// hash and cookie value.
    /// </returns>
    public Task<CallbackSaveContext> ExecuteAsync(
        CallbackSaveContext ctx,
        CancellationToken ct)
    {
        // 1. Generate token + hash
        var generated = _tokens.Generate();

        // 2. Build the cookie value
        var cookie = SessionCookie.FromPlaintextToken(generated.PlaintextToken);

        // 3. Return updated context
        return Task.FromResult(
            ctx with
            {
                TokenHash = generated.HashedToken.Value,
                CookieValue = cookie.Value
            }
        );
    }
}
