using Frank.Core.Application.Abstractions.ImmutableContexts;
using Frank.Identity.Application.Abstractions.Callback.Save;
using Frank.Identity.Application.Abstractions.Users;

namespace Frank.Identity.Application.Callback.Save.Steps;

/// <summary>
/// Represents the pipeline step responsible for resolving the internal user
/// associated with the external identity provider information captured during
/// the OIDC callback.
/// <para>
/// This step executes only when <see cref="CallbackSaveContext.UserId"/> is
/// null, ensuring that user resolution occurs exactly once at the beginning of
/// the save pipeline.
/// </para>
/// <para>
/// The step delegates resolution to <see cref="IUserResolver"/>, which maps the
/// external identity provider subject and claims to an internal user identifier.
/// </para>
/// <para>
/// The step returns a new immutable context containing the resolved user ID.
/// </para>
/// </summary>
public sealed class ResolveUserStep
    : IImmutableContextBuildStep<CallbackSaveContext>
{
    private readonly IUserResolver _identityResolver;

    /// <summary>
    /// Creates a new <see cref="ResolveUserStep"/> using the provided user
    /// resolver.
    /// </summary>
    /// <param name="identityResolver">
    /// The resolver responsible for mapping external identity provider data to
    /// an internal user identifier.
    /// </param>
    public ResolveUserStep(IUserResolver identityResolver)
    {
        _identityResolver = identityResolver;
    }

    /// <summary>
    /// Metadata describing this pipeline step, including its unique identifier
    /// and human‑readable display name. Used by pipeline diagnostics and
    /// observability tooling.
    /// </summary>
    public IImmutableContextBuildStepMetadata Metadata =>
        new ImmutableContextBuildStepMetadata(
            id: "ResolveUser",
            displayName: "Resolve User"
        );

    /// <summary>
    /// Determines whether this step can execute based on the current callback
    /// save context.
    /// <para>
    /// This step executes only when <see cref="CallbackSaveContext.UserId"/> is
    /// null, ensuring that user resolution happens exactly once at the start of
    /// the pipeline.
    /// </para>
    /// </summary>
    /// <param name="ctx">The current immutable callback save context.</param>
    /// <returns>
    /// <c>true</c> if <see cref="CallbackSaveContext.UserId"/> is null;
    /// otherwise <c>false</c>.
    /// </returns>
    public bool CanExecute(CallbackSaveContext ctx)
        => ctx.UserId is null;

    /// <summary>
    /// Resolves the internal user identifier from the external identity provider
    /// information and returns a new immutable context containing the resolved
    /// user ID.
    /// <para>
    /// This step performs no external side effects beyond invoking the user
    /// resolver and maintains full immutability guarantees.
    /// </para>
    /// </summary>
    /// <param name="ctx">The current immutable callback save context.</param>
    /// <param name="ct">A cancellation token for the asynchronous operation.</param>
    /// <returns>
    /// A new <see cref="CallbackSaveContext"/> containing the resolved user ID.
    /// </returns>
    public async Task<CallbackSaveContext> ExecuteAsync(
        CallbackSaveContext ctx,
        CancellationToken ct)
    {
        var external = ctx.External;
        var userId = await _identityResolver.ResolveAsync(external, ct);

        return ctx with { UserId = userId };
    }
}
