using CampFitFurDogs.Application.Abstractions.Authentication.Callback;
using CampFitFurDogs.Domain.Authentication.Sessions;
using CampFitFurDogs.Domain.Customers;
using Frank.Abstractions.ImmutableContextBuilder;
using Frank.Abstractions.UnitOfWork;

namespace CampFitFurDogs.Application.Authentication.Callback.Steps;

public sealed class CreateSessionStep
    : IImmutableContextBuildStep<ApplicationAuthCallbackContext>
{
    private readonly ISessionRepository _repo;
    private readonly IUnitOfWork _uow;

    public CreateSessionStep(ISessionRepository repo, IUnitOfWork uow)
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
        => ctx.CustomerId is not null && ctx.TokenHash is not null && ctx.SessionId is null;

    public async Task<ApplicationAuthCallbackContext> ExecuteAsync(
        ApplicationAuthCallbackContext ctx,
        CancellationToken ct)
    {
        if (ctx.CustomerId is null)
            throw new InvalidOperationException("CustomerId must be resolved before creating a session.");

        if (ctx.TokenHash is null)
            throw new InvalidOperationException("TokenHash must be generated before creating a session.");

        var session = Session.Create(
            tokenHash: SessionTokenHash.From(ctx.TokenHash),
            ownerId: CustomerId.From(ctx.CustomerId.Value),
            createdAt: ctx.Now
        );

        await _repo.CreateAsync(session);
        await _uow.CommitAsync(ct);

        return ctx with { SessionId = session.Id.Value };
    }
}
