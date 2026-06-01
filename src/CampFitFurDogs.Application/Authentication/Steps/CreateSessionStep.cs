using CampFitFurDogs.Domain.Authentication.Sessions;
using CampFitFurDogs.Domain.Customers;
using SharedKernel.Abstractions;

namespace CampFitFurDogs.Application.Authentication.Steps;

public sealed class CreateSessionStep : IAuthCallbackStep
{
    private readonly ISessionRepository _repo;
    private readonly IUnitOfWork _uow;

    public CreateSessionStep(ISessionRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task ExecuteAsync(AuthCallbackContext ctx, CancellationToken ct)
    {
        ctx.RequireCustomerId();
        ctx.RequireTokenHash();

        var session = Session.Create(
            ctx.TokenHash!,
            CustomerId.From(ctx.CustomerId!.Value),
            ctx.Now
        );

        ctx.Session = session;

        await _repo.CreateAsync(session);

        await _uow.CommitAsync(ct);
    }
}
