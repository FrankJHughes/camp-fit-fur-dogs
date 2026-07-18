using Frank.Core.Application.Abstractions.ImmutableContexts;
using Frank.Identity.Application.Abstractions.Callback.Save;

namespace Frank.Core.Application.Tests.Fakes.Application.Authentication.Callback.Steps;

public sealed class SetFinalValuesStep : IImmutableContextBuildStep<CallbackSaveContext>
{
    private readonly Guid _userId;
    private readonly Guid _sessionId;
    private readonly string _tokenHash;
    private readonly string _cookieValue;

    public SetFinalValuesStep(
        Guid userId,
        Guid sessionId,
        string tokenHash,
        string cookieValue)
    {
        _userId = userId;
        _sessionId = sessionId;
        _tokenHash = tokenHash;
        _cookieValue = cookieValue;
    }

    public IImmutableContextBuildStepMetadata Metadata =>
        new ImmutableContextBuildStepMetadata("SetFinalValues", "Set Final Values");

    public bool CanExecute(CallbackSaveContext ctx) => true;

    public Task<CallbackSaveContext> ExecuteAsync(CallbackSaveContext ctx, CancellationToken ct)
        => Task.FromResult(
            ctx with
            {
                UserId = _userId,
                SessionId = _sessionId,
                TokenHash = _tokenHash,
                CookieValue = _cookieValue,
            }
        );
}
