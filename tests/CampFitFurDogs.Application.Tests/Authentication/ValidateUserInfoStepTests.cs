using CampFitFurDogs.Application.Abstractions.Authentication;
using CampFitFurDogs.Application.Authentication.Steps

;

namespace CampFitFurDogs.Application.Tests.Authentication;

public sealed class ValidateUserInfoStepTests
{
    [Fact]
    public async Task Throws_WhenExternalIdMissing()
    {
        var ctx = new AuthCallbackContext("code")
        {
            User = new AuthUser("", "Frank", "Smith", "frank@example.com")
        };

        var step = new ValidateUserStep();

        await Assert.ThrowsAsync<AuthCallbackException>(() =>
            step.ExecuteAsync(ctx, CancellationToken.None));
    }

    [Fact]
    public async Task Succeeds_WhenExternalIdPresent()
    {
        var ctx = new AuthCallbackContext("code")
        {
            User = new AuthUser("ext-1", "Frank", "Smith", "frank@example.com")
        };

        var step = new ValidateUserStep();

        await step.ExecuteAsync(ctx, CancellationToken.None);
    }
}
