using Frank.Core.Application.Abstractions.ImmutableContexts;
using Frank.Identity.Application.Abstractions.Callback.Save;
using Frank.Identity.Application.Abstractions.Sessions.CreateSession;
using Frank.Identity.Application.Abstractions.UnitOfWork;
using Frank.Identity.Domain.Sessions;
using Frank.Identity.Domain.Users;

namespace Frank.Identity.Application.Callback.Save.Steps;

/// <summary>
/// Represents the pipeline step responsible for creating a new authenticated
/// session for the resolved user.
/// <para>
/// This step executes only when:
/// </para>
/// <list type="bullet">
/// <item><description><see cref="CallbackSaveContext.UserId"/> is present</description></item>
/// <item><description><see cref="CallbackSaveContext.TokenHash"/> is present</description></item>
/// <item><description><see cref="CallbackSaveContext.SessionId"/> is null</description></item>
/// </list>
/// <para>
/// The step constructs a new <see cref="Session"/> domain object, persists it
/// using <see cref="ICreateSessionWriter"/>, commits the unit of work, and
/// returns a new immutable context containing the assigned session identifier.
/// </para>
/// </summary>
public sealed class CreateSessionStep
    : IImmutableContextBuildStep<CallbackSaveContext>
{
    private readonly ICreateSessionWriter _writer;
    private readonly IFrankIdentityUnitOfWork _uow;

    /// <summary>
    /// Creates a new <see cref="CreateSessionStep"/> using the provided session
    /// writer and unit of work.
    /// </summary>
    /// <param name="writer">
    /// The writer responsible for persisting newly created sessions.
    /// </param>
    /// <param name="uow">
    /// The unit of work used to commit session creation to the underlying store.
    /// </param>
    public CreateSessionStep(ICreateSessionWriter writer, IFrankIdentityUnitOfWork uow)
    {
        _writer = writer;
        _uow = uow;
    }

    /// <summary>
    /// Metadata describing this pipeline step, including its unique identifier
    /// and human‑readable display name. Used by pipeline diagnostics and
    /// observability tooling.
    /// </summary>
    public IImmutableContextBuildStepMetadata Metadata =>
        new ImmutableContextBuildStepMetadata(
            id: "CreateSession",
            displayName: "Create Session"
        );

    /// <summary>
    /// Determines whether this step can execute based on the current callback
    /// save context.
    /// <para>
    /// This step executes only when:
    /// </para>
    /// <list type="bullet">
    /// <item><description><see cref="CallbackSaveContext.UserId"/> is not null</description></item>
    /// <item><description><see cref="CallbackSaveContext.TokenHash"/> is not null</description></item>
    /// <item><description><see cref="CallbackSaveContext.SessionId"/> is null</description></item>
    /// </list>
    /// </summary>
    /// <param name="ctx">The current immutable callback save context.</param>
    /// <returns>
    /// <c>true</c> if the step is ready to execute; otherwise <c>false</c>.
    /// </returns>
    public bool CanExecute(CallbackSaveContext ctx)
        => ctx.UserId is not null && ctx.TokenHash is not null && ctx.SessionId is null;

    /// <summary>
    /// Creates a new authenticated session for the resolved user, persists it,
    /// commits the unit of work, and returns a new immutable context containing
    /// the assigned session identifier.
    /// <para>
    /// The returned context includes:
    /// </para>
    /// <list type="bullet">
    /// <item><description>The newly created session's identifier</description></item>
    /// </list>
    /// <para>
    /// This step performs external side effects (session persistence and commit)
    /// but maintains full immutability guarantees by returning a new context
    /// instance.
    /// </para>
    /// </summary>
    /// <param name="ctx">The current immutable callback save context.</param>
    /// <param name="ct">A cancellation token for the asynchronous operation.</param>
    /// <returns>
    /// A new <see cref="CallbackSaveContext"/> containing the created session ID.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if required fields (<see cref="CallbackSaveContext.UserId"/> or
    /// <see cref="CallbackSaveContext.TokenHash"/>) are unexpectedly null.
    /// </exception>
    public async Task<CallbackSaveContext> ExecuteAsync(
        CallbackSaveContext ctx,
        CancellationToken ct)
    {
        if (ctx.UserId is null)
            throw new InvalidOperationException("UserId must be resolved before creating a session.");

        if (ctx.TokenHash is null)
            throw new InvalidOperationException("TokenHash must be generated before creating a session.");

        var session = Session.Create(
            tokenHash: SessionTokenHash.From(ctx.TokenHash),
            ownerId: UserId.From(ctx.UserId.Value),
            createdAt: ctx.Now
        );

        await _writer.WriteAsync(session, ct);
        await _uow.CommitAsync(ct);

        return ctx with { SessionId = session.Id.Value };
    }
}
