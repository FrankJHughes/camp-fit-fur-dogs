using CampFitFurDogs.Application.Abstractions.Authentication;
using CampFitFurDogs.Application.Abstractions.Authentication.Oidc;
using CampFitFurDogs.Application.Authentication;
using CampFitFurDogs.Application.Authentication.Steps;
using CampFitFurDogs.Domain.Authentication.Sessions;
using CampFitFurDogs.Domain.Customers;
using Microsoft.Extensions.Options;

namespace CampFitFurDogs.Application.Tests.Authentication;

public sealed class BuildRedirectStepTests
{
    private static AuthCallbackResult FakeResult()
        => new(
            CustomerId.From(Guid.NewGuid()),
            SessionCookie.FromPlaintextToken("abcd1234abcd1234abcd1234abcd1234abcd1234abcd1234abcd1234abcd1234"),
            ""
        );

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
                SessionTokenHash.From("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"),
                CustomerId.From(Guid.NewGuid()),
                DateTimeOffset.UtcNow
            ),
            Result = FakeResult()
        };

        var step = CreateStep("https://example.com/after");

        await step.ExecuteAsync(ctx, CancellationToken.None);

        ctx.Result!.RedirectUrl.Should().Be("https://example.com/after");
    }
}
