using CampFitFurDogs.Application.Abstractions.Authentication.Callback;
using Frank.Abstractions.ImmutableContext;

namespace Frank.Tests.Fakes.Application.Authentication.Callback.Steps;

public sealed class SetFinalValuesStep : IImmutableContextBuildStep<ApplicationAuthCallbackContext>
{
    private readonly Guid _customerId;
    private readonly Guid _sessionId;
    private readonly string _tokenHash;
    private readonly string _cookieValue;
    private readonly string _redirectUrl;

    public SetFinalValuesStep(
        Guid customerId,
        Guid sessionId,
        string tokenHash,
        string cookieValue,
        string redirectUrl)
    {
        _customerId = customerId;
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
                CustomerId = _customerId,
                SessionId = _sessionId,
                TokenHash = _tokenHash,
                CookieValue = _cookieValue,
                RedirectUrl = _redirectUrl
            }
        );
}
