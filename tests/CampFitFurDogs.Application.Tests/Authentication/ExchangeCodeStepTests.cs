using CampFitFurDogs.Application.Abstractions.Authentication;
using CampFitFurDogs.Application.Authentication;
using CampFitFurDogs.Application.Authentication.Steps;
using CampFitFurDogs.TestUtilities.Fakes;

namespace CampFitFurDogs.Application.Tests.Authentication;

public sealed class ExchangeCodeStepTests
{
    [Fact]
    public async Task ExecuteAsync_SetsTokenOnContext()
    {
        var fake = new FakeAuthClient
        {
            TokenToReturn = new AuthToken("abc123")
        };

        var ctx = new AuthCallbackContext("the-code");
        var step = new ExchangeCodeStep(fake);

        await step.ExecuteAsync(ctx, CancellationToken.None);

        Assert.Equal("abc123", ctx.Token!.AccessToken);
    }
}
