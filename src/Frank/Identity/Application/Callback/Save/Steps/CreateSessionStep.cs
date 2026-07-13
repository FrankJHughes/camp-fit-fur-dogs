using Frank.Core.Application.Abstractions.ImmutableContext;
using Frank.Core.Application.Abstractions.UnitOfWork;
using Frank.Identity.Application.Abstractions.Callback.Save;
using Frank.Identity.Domain.Sessions;
using Frank.Identity.Domain.Users;

namespace Frank.Identity.Application.Callback.Save.Steps;

public sealed class CreateSessionStep
    : IImmutableContextBuildStep<SaveCallbackContext>
{
    private readonly ISessionRepository _repo;
    private readonly IFrankIdentityUnitOfWork _uow;

    public CreateSessionStep(ISessionRepository repo, IFrankIdentityUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public IImmutableContextBuildStepMetadata Metadata =>
        new ImmutableContextBuildStepMetadata(
            id: "CreateSession",
            displayName: "Create Session"
        );

    public bool CanExecute(SaveCallbackContext ctx)
        => ctx.UserId is not null && ctx.TokenHash is not null && ctx.SessionId is null;

    public async Task<SaveCallbackContext> ExecuteAsync(
        SaveCallbackContext ctx,
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

        await _repo.CreateAsync(session, ct);
        await _uow.CommitAsync(ct);

        return ctx with { SessionId = session.Id.Value };
    }
}
