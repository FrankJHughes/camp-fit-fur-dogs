using CampFitFurDogs.Application.Abstractions.Authentication;
using CampFitFurDogs.Application.Authentication;
using CampFitFurDogs.Application.Authentication.Steps;
using CampFitFurDogs.TestUtilities.Fakes;

namespace CampFitFurDogs.Application.Tests.Authentication;

public sealed class ExchangeCodeStepTests
{
    [Fact]
    public async Task ExecuteAsync_SetsTokenOnReturnedContext()
    {
        var fake = new FakeAuthClient
        {
            TokenToReturn = new AuthToken("abc123")
        };

        var ctx = new AuthCallbackContext("the-code");
        var step = new ExchangeCodeStep(fake);

        var updated = await step.ExecuteAsync(ctx, CancellationToken.None);

        updated.Token.Should().NotBeNull();
        updated.Token!.AccessToken.Should().Be("abc123");

        // original context remains unchanged (immutability guarantee)
        ctx.Token.Should().BeNull();
    }
}
