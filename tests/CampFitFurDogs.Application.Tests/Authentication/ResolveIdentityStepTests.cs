using CampFitFurDogs.Application.Abstractions.Authentication;
using CampFitFurDogs.Application.Authentication;
using CampFitFurDogs.Application.Authentication.Pipeline.Steps
;
using CampFitFurDogs.TestUtilities.Fakes;

namespace CampFitFurDogs.Application.Tests.Authentication;

public sealed class ResolveIdentityStepTests
{
    [Fact]
    public async Task SetsCustomerId_OnReturnedContext()
    {
        var fake = new FakeIdentityResolver
        {
            Result = Guid.NewGuid()
        };

        var ctx = new AuthCallbackContext(
            Code: "code",
            User: new AuthUser("ext-1", "Frank", "Smith", "frank@example.com")
        );

        var step = new ResolveIdentityStep(fake);

        var updated = await step.ExecuteAsync(ctx, CancellationToken.None);

        updated.CustomerId.Should().Be(fake.Result);

        // original context remains unchanged (immutability guarantee)
        ctx.CustomerId.Should().BeNull();
    }
}
