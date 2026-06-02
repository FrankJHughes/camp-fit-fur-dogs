using CampFitFurDogs.Application.Abstractions.Audit;

namespace CampFitFurDogs.Application.Authentication.Pipeline.Steps;

public sealed class AuditLoginStep : IAuthCallbackStep
{
    private readonly IAuditLogger _audit;

    public AuthCallbackStepMetadata Metadata =>
        new(
            "AuditLogin",
            "Audit Login",
            AuthCallbackStepCategory.AuditLogin);

    public AuditLoginStep(IAuditLogger audit)
    {
        _audit = audit;
    }

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
