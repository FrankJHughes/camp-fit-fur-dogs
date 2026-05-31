using CampFitFurDogs.Application.Abstractions.Audit;
using CampFitFurDogs.Application.Abstractions.Authentication;

namespace CampFitFurDogs.Application.Authentication.Steps;

public sealed class AuditLoginStep : IAuthCallbackStep
{
    private readonly IAuditLogger _audit;

    public AuditLoginStep(IAuditLogger audit)
    {
        _audit = audit;
    }

    public async Task ExecuteAsync(AuthCallbackContext ctx, CancellationToken ct)
    {
        ctx.RequireCustomerId();
        ctx.RequireUser();

        await _audit.LoginSucceeded(
            ctx.CustomerId!.Value,
            ctx.User!.ExternalId
        );
    }
}
