using Frank.Identity.Application.Abstractions.AuditLogging;
using Frank.Core.Application.Abstractions.ImmutableContexts;
using Frank.Identity.Application.Abstractions.Callback.Save;

namespace Frank.Identity.Application.Callback.Save.Steps;

/// <summary>
/// Represents the pipeline step responsible for auditing a successful login
/// after the user has been resolved.
/// <para>
/// This step executes only when <see cref="CallbackSaveContext.UserId"/> is
/// present, ensuring that login auditing occurs strictly after user resolution
/// and before session persistence.
/// </para>
/// <para>
/// The step emits an audit event through <see cref="IAuditLogger"/> indicating
/// that the login succeeded for the resolved internal user and associated
/// external identity provider subject.
/// </para>
/// <para>
/// The step does not modify the context; it simply performs an external side
/// effect (audit logging) and returns the original immutable context.
/// </para>
/// </summary>
public sealed class AuditLoginStep
    : IImmutableContextBuildStep<CallbackSaveContext>
{
    private readonly IAuditLogger _audit;

    /// <summary>
    /// Creates a new <see cref="AuditLoginStep"/> using the provided audit logger.
    /// </summary>
    /// <param name="audit">
    /// The audit logger responsible for recording successful login events.
    /// </param>
    public AuditLoginStep(IAuditLogger audit)
    {
        _audit = audit;
    }

    /// <summary>
    /// Metadata describing this pipeline step, including its unique identifier
    /// and human‑readable display name. Used by pipeline diagnostics and
    /// observability tooling.
    /// </summary>
    public IImmutableContextBuildStepMetadata Metadata =>
        new ImmutableContextBuildStepMetadata(
            id: "AuditLogin",
            displayName: "Audit Login"
        );

    /// <summary>
    /// Determines whether this step can execute based on the current callback
    /// save context.
    /// <para>
    /// This step executes only when <see cref="CallbackSaveContext.UserId"/> is
    /// present, ensuring that login auditing occurs after user resolution.
    /// </para>
    /// </summary>
    /// <param name="ctx">The current immutable callback save context.</param>
    /// <returns>
    /// <c>true</c> if <see cref="CallbackSaveContext.UserId"/> is not null;
    /// otherwise <c>false</c>.
    /// </returns>
    public bool CanExecute(CallbackSaveContext ctx)
        => ctx.UserId is not null;

    /// <summary>
    /// Emits a login‑succeeded audit event for the resolved user and returns the
    /// original immutable context unchanged.
    /// <para>
    /// This step performs an external side effect (audit logging) but does not
    /// modify the context, preserving immutability guarantees.
    /// </para>
    /// </summary>
    /// <param name="ctx">The current immutable callback save context.</param>
    /// <param name="ct">A cancellation token for the asynchronous operation.</param>
    /// <returns>
    /// The same <see cref="CallbackSaveContext"/> instance passed in.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if <see cref="CallbackSaveContext.UserId"/> is unexpectedly null
    /// when the step executes.
    /// </exception>
    public async Task<CallbackSaveContext> ExecuteAsync(
        CallbackSaveContext ctx,
        CancellationToken ct)
    {
        if (ctx.UserId is null)
            throw new InvalidOperationException("UserId must be resolved before auditing login.");

        var external = ctx.External;

        await _audit.LoginSucceeded(
            userId: ctx.UserId.Value,
            externalId: external.SubjectId
        );

        return ctx;
    }
}
