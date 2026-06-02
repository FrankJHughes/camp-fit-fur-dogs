using CampFitFurDogs.Application.Abstractions.Authentication.Oidc;
using CampFitFurDogs.Application.Authentication;
using CampFitFurDogs.Application.Authentication.Pipeline.Steps
;
using CampFitFurDogs.Domain.Authentication.Sessions;
using CampFitFurDogs.Domain.Customers;
using Microsoft.Extensions.Options;

namespace CampFitFurDogs.Application.Tests.Authentication;

public sealed class BuildRedirectStepTests
{
    private static BuildRedirectStep CreateStep(string redirect)
        => new BuildRedirectStep(Options.Create(new OidcOptions
        {
            PostLoginRedirectUrl = redirect
        }));

    [Fact]
    public async Task Updates_redirect_url()
    {
        var ctx = new AuthCallbackContext("code")
        {
            Session = Session.Create(
                ownerId: CustomerId.From(Guid.NewGuid()),
                tokenHash: SessionTokenHash.From(
                    "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"
                ),
                createdAt: DateTimeOffset.UtcNow
            ),
            SessionCookie = SessionCookie.FromPlaintextToken("dummy-cookie")
        };

        var step = CreateStep("https://example.com/after");

        var updated = await step.ExecuteAsync(ctx, CancellationToken.None);

        updated.RedirectUrl.Should().Be("https://example.com/after");
    }
}
