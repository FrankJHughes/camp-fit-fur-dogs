using CampFitFurDogs.Application.Abstractions.Authentication;
using CampFitFurDogs.Domain.Authentication.Sessions;
using CampFitFurDogs.Domain.Customers;
using Frank.Abstractions;

namespace CampFitFurDogs.Application.Authentication.Steps;

public sealed class CreateSessionStep : IAuthCallbackStep
{
    private readonly ISessionRepository _repo;
    private readonly IUnitOfWork _uow;

    public AuthCallbackStepMetadata Metadata =>
        new("CreateSession", "Create Session");

    public CreateSessionStep(ISessionRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public bool CanExecute(AuthCallbackContext ctx)
        => ctx.CustomerId is not null && ctx.TokenHash is not null;

    public async Task<AuthCallbackContext> ExecuteAsync(AuthCallbackContext ctx, CancellationToken ct)
    {
        // Preconditions (pure validators)
        ctx.RequireCustomerId();
        ctx.RequireTokenHash();

        // Pure domain creation
        var session = Session.Create(
            ctx.TokenHash!,
            CustomerId.From(ctx.CustomerId!.Value),
            ctx.Now
        );

        // Side effects (isolated)
        await _repo.CreateAsync(session);
        await _uow.CommitAsync(ct);

        // Pure state transition
        return ctx with { Session = session };
    }
}
