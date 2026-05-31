using CampFitFurDogs.Application.Abstractions.Authentication;
using CampFitFurDogs.Application.Authentication;
using CampFitFurDogs.Application.Authentication.Steps;
using CampFitFurDogs.Domain.Authentication.Sessions;
using CampFitFurDogs.Domain.Customers;

public sealed class CreateSessionStep : IAuthCallbackStep
{
    private readonly ISessionRepository _repo;

    public CreateSessionStep(ISessionRepository repo)
    {
        _repo = repo;
    }

    public async Task ExecuteAsync(AuthCallbackContext ctx, CancellationToken ct)
    {
        ctx.RequireCustomerId();
        ctx.RequireTokenHash();

        // Domain invariants enforce correctness here
        var session = Session.Create(
            ctx.TokenHash!,
            CustomerId.From(ctx.CustomerId!.Value),
            ctx.Now
        );

        ctx.Session = session;

        await _repo.CreateAsync(session);
    }
}
