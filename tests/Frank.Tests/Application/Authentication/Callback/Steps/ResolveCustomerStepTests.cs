using Frank.Application.Abstractions.Identity.Callback;
using Frank.Abstractions.Identity;
using Frank.Application.Identity.Callback.Steps;
using Frank.TestUtilities.Fakes.Authentication.Callback;
using Frank.Abstractions.Identity.Callback;

namespace CampFitFurDogs.Application.Tests.Authentication.Callback.Steps;

public sealed class ResolveUserStepTests
{
    private sealed class FakeIdentityResolver : IIdentityResolver
    {
        public Guid ReturnedId { get; set; } = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        public FrankAuthCallbackResult? ReceivedExternal { get; private set; }

        public Task<Guid> ResolveAsync(FrankAuthCallbackResult external, CancellationToken ct)
        {
            ReceivedExternal = external;
            return Task.FromResult(ReturnedId);
        }
    }

    [Fact]
    public async Task ExecuteAsync_SetsUserId()
    {
        var resolver = new FakeIdentityResolver();
        var step = new ResolveUserStep(resolver);

        var ctx = new ApplicationAuthCallbackContext
        {
            External = FakeFrankAuthCallbackResult.Create("sub-123"),
            Now = DateTimeOffset.UtcNow
        };

        var result = await step.ExecuteAsync(ctx, CancellationToken.None);

        result.UserId.Should().Be(resolver.ReturnedId);
        resolver.ReceivedExternal!.SubjectId.Should().Be("sub-123");
    }

    [Fact]
    public void CanExecute_OnlyWhenUserIdIsNull()
    {
        var step = new ResolveUserStep(new FakeIdentityResolver());

        step.CanExecute(new ApplicationAuthCallbackContext
        {
            External = FakeFrankAuthCallbackResult.Create(),
            Now = DateTimeOffset.UtcNow,
            UserId = null
        }).Should().BeTrue();

        step.CanExecute(new ApplicationAuthCallbackContext
        {
            External = FakeFrankAuthCallbackResult.Create(),
            Now = DateTimeOffset.UtcNow,
            UserId = Guid.NewGuid()
        }).Should().BeFalse();
    }
}
