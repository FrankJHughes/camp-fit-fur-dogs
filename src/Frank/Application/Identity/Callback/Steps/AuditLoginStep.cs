using Frank.Application.Abstractions.Audit;
using Frank.Application.Abstractions.Identity.Callback;
using Frank.Abstractions.ImmutableContext;

namespace Frank.Application.Identity.Callback.Steps;

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
        => ctx.UserId is not null; // always runs once user is resolved

    public async Task<ApplicationAuthCallbackContext> ExecuteAsync(
        ApplicationAuthCallbackContext ctx,
        CancellationToken ct)
    {
        if (ctx.UserId is null)
            throw new InvalidOperationException("UserId must be resolved before auditing login.");

        var external = ctx.External;

        await _audit.LoginSucceeded(
            userId: ctx.UserId.Value,
            externalId: external.SubjectId
        );

        return ctx;
    }
}
