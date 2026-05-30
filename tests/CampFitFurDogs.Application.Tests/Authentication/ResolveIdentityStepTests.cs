using CampFitFurDogs.Application.Abstractions.Authentication;
using CampFitFurDogs.Application.Authentication;
using CampFitFurDogs.Application.Authentication.Steps;
using CampFitFurDogs.TestUtilities.Fakes;

namespace CampFitFurDogs.Application.Tests.Authentication;

public sealed class ResolveIdentityStepTests
{
    [Fact]
    public async Task SetsCustomerId()
    {
        var fake = new FakeIdentityResolver
        {
            Result = Guid.NewGuid()
        };

        var ctx = new AuthCallbackContext("code")
        {
            User = new AuthUser("ext-1", "Frank", "Smith", "frank@example.com")
        };

        var step = new ResolveIdentityStep(fake);

        await step.ExecuteAsync(ctx, CancellationToken.None);

        Assert.Equal(fake.Result, ctx.CustomerId);
    }
}
