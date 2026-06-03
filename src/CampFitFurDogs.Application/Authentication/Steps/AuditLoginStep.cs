using CampFitFurDogs.Application.Abstractions.Audit;
using CampFitFurDogs.Application.Abstractions.Authentication;

namespace CampFitFurDogs.Application.Authentication.Steps;

public sealed class AuditLoginStep : IAuthCallbackStep
{
    private readonly IAuditLogger _audit;

    public AuthCallbackStepMetadata Metadata =>
        new("AuditLogin", "Audit Login");

    public AuditLoginStep(IAuditLogger audit)
    {
        _audit = audit;
    }

    public bool CanExecute(AuthCallbackContext ctx)
        => ctx.User is not null && ctx.CustomerId is not null;

    public async Task<AuthCallbackContext> ExecuteAsync(AuthCallbackContext ctx, CancellationToken ct)
    {
        ctx.RequireCustomerId();
        ctx.RequireUser();

        await _audit.LoginSucceeded(
            ctx.CustomerId!.Value,
            ctx.User!.ExternalId
        );

        return ctx;
    }
}
