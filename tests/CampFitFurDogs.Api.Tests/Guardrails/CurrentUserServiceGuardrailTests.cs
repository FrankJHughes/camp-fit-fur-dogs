using System.Security.Claims;
using CampFitFurDogs.Infrastructure.Identity;
using FluentAssertions;
using Frank.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CampFitFurDogs.Api.Tests.Guardrails;

public class CurrentUserServiceGuardrailTests
{
    private static ServiceProvider BuildProvider()
    {
        var services = new ServiceCollection();
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, AuthenticatedUserService>();
        return services.BuildServiceProvider();
    }

    // ------------------------------------------------------------
    // GUARDRAIL 1 — Should resolve AuthenticatedUserService
    // ------------------------------------------------------------
    [Fact]
    public void ShouldResolveAuthenticatedUserService()
    {
        using var provider = BuildProvider();
        using var scope = provider.CreateScope();

        scope.ServiceProvider
            .GetRequiredService<ICurrentUser>()
            .Should()
            .BeOfType<AuthenticatedUserService>();
    }

    // ------------------------------------------------------------
    // GUARDRAIL 2 — Should behave as scoped service
    // ------------------------------------------------------------
    [Fact]
    public void ShouldBehaveAsScopedService()
    {
        using var provider = BuildProvider();
        using var scope = provider.CreateScope();

        var s1 = scope.ServiceProvider.GetRequiredService<ICurrentUser>();
        var s2 = scope.ServiceProvider.GetRequiredService<ICurrentUser>();

        s1.Should().BeSameAs(s2);
    }

    // ------------------------------------------------------------
    // GUARDRAIL 3 — Should have exactly one registration
    // ------------------------------------------------------------
    [Fact]
    public void ShouldHaveSingleEffectiveRegistration()
    {
        using var provider = BuildProvider();
        using var scope = provider.CreateScope();

        scope.ServiceProvider
            .GetServices<ICurrentUser>()
            .Should()
            .HaveCount(1);
    }

    // ------------------------------------------------------------
    // GUARDRAIL 4 — Should read user ID from claims
    // ------------------------------------------------------------
    [Fact]
    public void ShouldReadUserIdFromClaims()
    {
        var userId = Guid.NewGuid();

        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[] { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) },
                    "test"
                )
            )
        };

        var accessor = new HttpContextAccessor { HttpContext = httpContext };
        var service = new AuthenticatedUserService(accessor);

        service.Id.Should().Be(userId);
    }
}
