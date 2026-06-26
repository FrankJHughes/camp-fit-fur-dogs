using System.Security.Claims;
using FluentAssertions;
using Frank.Abstractions.Identity;
using Frank.Infrastructure.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace CampFitFurDogs.Api.Tests.Guardrails;

public class CurrentUserGuardrailTests
{
    private static ServiceProvider BuildProvider()
    {
        var services = new ServiceCollection();
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, AuthenticatedUser>();
        return services.BuildServiceProvider();
    }

    // ------------------------------------------------------------
    // GUARDRAIL 1 — Should resolve AuthenticatedUser
    // ------------------------------------------------------------
    [Fact]
    public void ShouldResolveAuthenticatedUser()
    {
        using var provider = BuildProvider();
        using var scope = provider.CreateScope();

        scope.ServiceProvider
            .GetRequiredService<ICurrentUser>()
            .Should()
            .BeOfType<AuthenticatedUser>();
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
        var service = new AuthenticatedUser(accessor);

        service.Id.Should().Be(userId);
    }
}
