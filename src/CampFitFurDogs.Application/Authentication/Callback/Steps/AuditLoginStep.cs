using CampFitFurDogs.Application.Abstractions.Audit;
using CampFitFurDogs.Application.Abstractions.Authentication.Callback;
using Frank.Abstractions.ImmutableContextBuilder;

namespace CampFitFurDogs.Application.Authentication.Callback.Steps;

public sealed class AuditLoginStep
    : IImmutableContextBuildStep<ApplicationAuthCallbackContext>
{
    private readonly IAuditLogger _audit;

    public AuditLoginStep(IAuditLogger audit)
    {
        _audit = audit;
    }

    public IImmutableContextBuildStepMetadata Metadata =>
        new ImmutableContextBuildStepMetadata(
            id: "AuditLogin",
            displayName: "Audit Login"
        );

    public bool CanExecute(ApplicationAuthCallbackContext ctx)
        => ctx.CustomerId is not null; // always runs once customer is resolved

    public async Task<ApplicationAuthCallbackContext> ExecuteAsync(
        ApplicationAuthCallbackContext ctx,
        CancellationToken ct)
    {
        if (ctx.CustomerId is null)
            throw new InvalidOperationException("CustomerId must be resolved before auditing login.");

        var external = ctx.External;

        await _audit.LoginSucceeded(
            customerId: ctx.CustomerId.Value,
            externalId: external.SubjectId
        );

        return ctx;
    }
}
