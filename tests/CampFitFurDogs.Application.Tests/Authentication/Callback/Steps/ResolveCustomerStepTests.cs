using CampFitFurDogs.Application.Abstractions.Authentication.Callback;
using CampFitFurDogs.Application.Abstractions.Identity;
using CampFitFurDogs.Application.Authentication.Callback.Steps;
using CampFitFurDogs.Application.Tests.Fakes.Authentication.Callback;
using Frank.Abstractions.Authentication.Callback;

namespace CampFitFurDogs.Application.Tests.Authentication.Callback.Steps;

public sealed class ResolveCustomerStepTests
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
    public async Task ExecuteAsync_SetsCustomerId()
    {
        var resolver = new FakeIdentityResolver();
        var step = new ResolveCustomerStep(resolver);

        var ctx = new ApplicationAuthCallbackContext
        {
            External = FakeFrankAuthCallbackResult.Create("sub-123"),
            Now = DateTimeOffset.UtcNow
        };

        var result = await step.ExecuteAsync(ctx, CancellationToken.None);

        result.CustomerId.Should().Be(resolver.ReturnedId);
        resolver.ReceivedExternal!.SubjectId.Should().Be("sub-123");
    }

    [Fact]
    public void CanExecute_OnlyWhenCustomerIdIsNull()
    {
        var step = new ResolveCustomerStep(new FakeIdentityResolver());

        step.CanExecute(new ApplicationAuthCallbackContext
        {
            External = FakeFrankAuthCallbackResult.Create(),
            Now = DateTimeOffset.UtcNow,
            CustomerId = null
        }).Should().BeTrue();

        step.CanExecute(new ApplicationAuthCallbackContext
        {
            External = FakeFrankAuthCallbackResult.Create(),
            Now = DateTimeOffset.UtcNow,
            CustomerId = Guid.NewGuid()
        }).Should().BeFalse();
    }
}
