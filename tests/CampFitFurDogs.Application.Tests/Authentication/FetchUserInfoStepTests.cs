using CampFitFurDogs.Application.Abstractions.Authentication;
using CampFitFurDogs.Application.Authentication.Steps

;
using CampFitFurDogs.TestUtilities.Fakes;

namespace CampFitFurDogs.Application.Tests.Authentication;

public sealed class FetchUserInfoStepTests
{
    [Fact]
    public async Task ExecuteAsync_SetsUserOnReturnedContext()
    {
        var fake = new FakeAuthClient
        {
            UserToReturn = new AuthUser("ext-1", "Frank", "Smith", "frank@example.com")
        };

        var ctx = new AuthCallbackContext(
            Code: "code",
            Token: new AuthToken("abc123")
        );

        var step = new FetchUserStep(fake);

        var updated = await step.ExecuteAsync(ctx, CancellationToken.None);

        updated.User.Should().NotBeNull();
        updated.User!.ExternalId.Should().Be("ext-1");

        // original context remains unchanged (immutability guarantee)
        ctx.User.Should().BeNull();
    }

    [Fact]
    public async Task ExecuteAsync_Throws_WhenUserIsNull()
    {
        var fake = new FakeAuthClient
        {
            UserToReturn = null
        };

        var ctx = new AuthCallbackContext(
            Code: "code",
            Token: new AuthToken("abc123")
        );

        var step = new FetchUserStep(fake);

        await Assert.ThrowsAsync<AuthCallbackException>(() =>
            step.ExecuteAsync(ctx, CancellationToken.None));
    }
}
