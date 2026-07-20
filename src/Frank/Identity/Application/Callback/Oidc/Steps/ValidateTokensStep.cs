using Frank.Core.Application.Abstractions.ImmutableContexts;
using Frank.Identity.Application.Abstractions.Callback.Oidc;
using Frank.Identity.Application.Abstractions.Oidc;

namespace Frank.Identity.Application.Callback.Oidc.Steps;

public sealed class ValidateTokensStep : IImmutableContextBuildStep<CallbackOidcContext>
{
    private readonly IOidcTokenValidator _validator;

    public IImmutableContextBuildStepMetadata Metadata { get; } =
        new ImmutableContextBuildStepMetadata("oidc.validate-tokens", "Validate ID Token");

    public ValidateTokensStep(IOidcTokenValidator validator)
    {
        _validator = validator;
    }

    public bool CanExecute(CallbackOidcContext ctx)
        => ctx.IdToken is not null;

    public async Task<CallbackOidcContext> ExecuteAsync(
        CallbackOidcContext ctx,
        CancellationToken ct)
    {
        var result = await _validator.ValidateAsync(ctx.IdToken!, ct);

        return ctx with
        {
            SubjectId = result.SubjectId,
            Claims = result.Claims
        };
    }
}
