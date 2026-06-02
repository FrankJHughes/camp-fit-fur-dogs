using CampFitFurDogs.Domain.Authentication.Sessions;
using CampFitFurDogs.Domain.Customers;
using SharedKernel.Abstractions;

namespace CampFitFurDogs.Application.Authentication.Pipeline.Steps
;

public sealed class CreateSessionStep : IAuthCallbackStep
{
    private readonly ISessionRepository _repo;
    private readonly IUnitOfWork _uow;
    public AuthCallbackStepMetadata Metadata =>
        new(
            "CreateSession",
            "Create Session",
            AuthCallbackStepCategory.CreateSession);


    public CreateSessionStep(ISessionRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<AuthCallbackContext> ExecuteAsync(AuthCallbackContext ctx, CancellationToken ct)
    {
        ctx.RequireCustomerId();
        ctx.RequireTokenHash();

        var session = Session.Create(
            ctx.TokenHash!,
            CustomerId.From(ctx.CustomerId!.Value),
            ctx.Now
        );

        await _repo.CreateAsync(session);
        await _uow.CommitAsync(ct);

        return ctx with { Session = session };
    }
}
