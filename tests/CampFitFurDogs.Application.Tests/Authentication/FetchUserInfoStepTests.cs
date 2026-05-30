using CampFitFurDogs.Application.Abstractions.Authentication;
using CampFitFurDogs.Application.Authentication;
using CampFitFurDogs.Application.Authentication.Steps;
using CampFitFurDogs.TestUtilities.Fakes;

namespace CampFitFurDogs.Application.Tests.Authentication;

public sealed class FetchUserInfoStepTests
{
    [Fact]
    public async Task ExecuteAsync_SetsUserOnContext()
    {
        var fake = new FakeAuthClient
        {
            UserToReturn = new AuthUser("ext-1", "Frank", "Smith", "frank@example.com")
        };

        var ctx = new AuthCallbackContext("code")
        {
            Token = new AuthToken("abc123")
        };

        var step = new FetchUserInfoStep(fake);

        await step.ExecuteAsync(ctx, CancellationToken.None);

        Assert.Equal("ext-1", ctx.User!.ExternalId);
    }

    [Fact]
    public async Task ExecuteAsync_Throws_WhenUserIsNull()
    {
        var fake = new FakeAuthClient
        {
            UserToReturn = null
        };

        var ctx = new AuthCallbackContext("code")
        {
            Token = new AuthToken("abc123")
        };

        var step = new FetchUserInfoStep(fake);

        await Assert.ThrowsAsync<AuthCallbackException>(() =>
            step.ExecuteAsync(ctx, CancellationToken.None));
    }
}
