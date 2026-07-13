using Frank.Identity.Abstractions;
using Frank.Identity.Application.Abstractions.Callback.Oidc;
using Frank.Identity.Application.Abstractions.Callback.Save;
using Frank.Identity.Application.Callback.Save.Steps;
using Frank.TestUtilities.Fakes.Authentication.Callback;

namespace CampFitFurDogs.Application.Tests.Authentication.Callback.Steps;

public sealed class ResolveUserStepTests
{
    private sealed class FakeIdentityResolver : IIdentityResolver
    {
        public Guid ReturnedId { get; set; } = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        public OidcCallbackContextBuilderResult? ReceivedExternal { get; private set; }

        public Task<Guid> ResolveAsync(OidcCallbackContextBuilderResult external, CancellationToken ct)
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

        var ctx = new SaveCallbackContext
        {
            External = FakeOidcCallbackResult.Create("sub-123"),
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

        step.CanExecute(new SaveCallbackContext
        {
            External = FakeOidcCallbackResult.Create(),
            Now = DateTimeOffset.UtcNow,
            UserId = null
        }).Should().BeTrue();

        step.CanExecute(new SaveCallbackContext
        {
            External = FakeOidcCallbackResult.Create(),
            Now = DateTimeOffset.UtcNow,
            UserId = Guid.NewGuid()
        }).Should().BeFalse();
    }
}
