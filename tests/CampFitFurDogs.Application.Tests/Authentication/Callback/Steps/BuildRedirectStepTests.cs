using CampFitFurDogs.Application.Abstractions.Authentication.Callback;
using CampFitFurDogs.Application.Authentication.Callback.Steps;
using CampFitFurDogs.Application.Settings;
using CampFitFurDogs.Application.Tests.Fakes.Authentication.Callback;
using Microsoft.Extensions.Options;

namespace CampFitFurDogs.Application.Tests.Authentication.Callback.Steps;

public sealed class BuildRedirectStepTests
{
    private static BuildRedirectStep NewStep(string defaultUrl = "/home")
    {
        return new BuildRedirectStep(
                Options.Create(new AuthCallbackSettings
                {
                    PostLoginRedirectUrl = defaultUrl
                })
            );
    }

    [Fact]
    public async Task ExecuteAsync_UsesRequestedRedirect_WhenSafe()
    {
        var step = NewStep();

        var ctx = new ApplicationAuthCallbackContext
        {
            External = FakeFrankAuthCallbackResult.Create(),
            Now = DateTimeOffset.UtcNow,
            RequestedRedirectUrl = "/dashboard"
        };

        var result = await step.ExecuteAsync(ctx, CancellationToken.None);

        result.RedirectUrl.Should().Be("/dashboard");
    }

    [Fact]
    public async Task ExecuteAsync_FallsBackToDefault_WhenUnsafe()
    {
        var step = NewStep("/safe");

        var ctx = new ApplicationAuthCallbackContext
        {
            External = FakeFrankAuthCallbackResult.Create(),
            Now = DateTimeOffset.UtcNow,
            RequestedRedirectUrl = "http://evil.com"
        };

        var result = await step.ExecuteAsync(ctx, CancellationToken.None);

        result.RedirectUrl.Should().Be("/safe");
    }

    [Fact]
    public void CanExecute_OnlyWhenRedirectUrlIsNull()
    {
        var step = NewStep();

        step.CanExecute(new ApplicationAuthCallbackContext
        {
            External = FakeFrankAuthCallbackResult.Create(),
            Now = DateTimeOffset.UtcNow,
            RedirectUrl = null
        }).Should().BeTrue();

        step.CanExecute(new ApplicationAuthCallbackContext
        {
            External = FakeFrankAuthCallbackResult.Create(),
            Now = DateTimeOffset.UtcNow,
            RedirectUrl = "/exists"
        }).Should().BeFalse();
    }
}
