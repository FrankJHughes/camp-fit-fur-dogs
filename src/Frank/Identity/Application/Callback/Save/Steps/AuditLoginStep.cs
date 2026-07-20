using Frank.Core.Application.Abstractions.Audit;
using Frank.Core.Application.Abstractions.ImmutableContexts;
using Frank.Identity.Application.Abstractions.Callback.Save;

namespace Frank.Identity.Application.Callback.Save.Steps;

public sealed class AuditLoginStep
    : IImmutableContextBuildStep<CallbackSaveContext>
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

    public bool CanExecute(CallbackSaveContext ctx)
        => ctx.UserId is not null; // always runs once user is resolved

    public async Task<CallbackSaveContext> ExecuteAsync(
        CallbackSaveContext ctx,
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
