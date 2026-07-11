using Frank.Application.Abstractions.Identity.Callback;
using Frank.Abstractions.ImmutableContext;
using System;
using System.Threading.Tasks;
using System.Threading;

namespace Frank.Tests.Fakes.Application.Authentication.Callback.Steps;

public sealed class SetFinalValuesStep : IImmutableContextBuildStep<ApplicationAuthCallbackContext>
{
    private readonly Guid _userId;
    private readonly Guid _sessionId;
    private readonly string _tokenHash;
    private readonly string _cookieValue;
    private readonly string _redirectUrl;

    public SetFinalValuesStep(
        Guid userId,
        Guid sessionId,
        string tokenHash,
        string cookieValue,
        string redirectUrl)
    {
        _userId = userId;
        _sessionId = sessionId;
        _tokenHash = tokenHash;
        _cookieValue = cookieValue;
        _redirectUrl = redirectUrl;
    }

    public IImmutableContextBuildStepMetadata Metadata =>
        new ImmutableContextBuildStepMetadata("SetFinalValues", "Set Final Values");

    public bool CanExecute(ApplicationAuthCallbackContext ctx) => true;

    public Task<ApplicationAuthCallbackContext> ExecuteAsync(ApplicationAuthCallbackContext ctx, CancellationToken ct)
        => Task.FromResult(
            ctx with
            {
                UserId = _userId,
                SessionId = _sessionId,
                TokenHash = _tokenHash,
                CookieValue = _cookieValue,
                RedirectUrl = _redirectUrl
            }
        );
}
