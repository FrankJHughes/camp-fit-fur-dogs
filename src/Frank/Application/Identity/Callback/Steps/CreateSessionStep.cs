using Frank.Application.Abstractions.Identity.Callback;
using Frank.Domain.Sessions;
using Frank.Domain.Users;
using Frank.Abstractions.ImmutableContext;
using Frank.Abstractions.UnitOfWork;

namespace Frank.Application.Identity.Callback.Steps;

public sealed class CreateSessionStep
    : IImmutableContextBuildStep<ApplicationAuthCallbackContext>
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

    public bool CanExecute(ApplicationAuthCallbackContext ctx)
        => ctx.UserId is not null && ctx.TokenHash is not null && ctx.SessionId is null;

    public async Task<ApplicationAuthCallbackContext> ExecuteAsync(
        ApplicationAuthCallbackContext ctx,
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

        await _repo.CreateAsync(session);
        await _uow.CommitAsync(ct);

        return ctx with { SessionId = session.Id.Value };
    }
}
