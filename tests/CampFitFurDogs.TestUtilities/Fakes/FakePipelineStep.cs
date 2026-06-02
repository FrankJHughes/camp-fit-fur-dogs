using CampFitFurDogs.Application.Authentication;
using CampFitFurDogs.Application.Authentication.Pipeline;
using CampFitFurDogs.Domain.Authentication.Sessions;
using CampFitFurDogs.Domain.Customers;

public sealed class FakePipelineStep : IAuthCallbackStep
{
    public AuthCallbackStepMetadata Metadata =>
        new("FakeStep", "Fake Step", AuthCallbackStepCategory.BuildRedirect);

    public required CustomerId CustomerId { get; init; }
    public required string RedirectUrl { get; init; }

    public Task<AuthCallbackContext> ExecuteAsync(AuthCallbackContext ctx, CancellationToken ct)
    {
        return Task.FromResult(ctx with
        {
            CustomerId = this.CustomerId.Value,
            RedirectUrl = this.RedirectUrl,
            SessionCookie = SessionCookie.FromPlaintextToken("fake-cookie")

        });
    }
}
