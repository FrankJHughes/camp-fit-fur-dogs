using System.Net.Http.Json;
using CampFitFurDogs.Infrastructure.Identity;
using CampFitFurDogs.TestUtilities.Factories;
using CampFitFurDogs.TestUtilities.Fixtures;
using FluentAssertions;
using Frank.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace CampFitFurDogs.Api.Tests.Guardrails;

[Collection("API With Postgres")]
public class CurrentUserServiceGuardrailTests
    : ApiWithPostgresTestBase
{
    public CurrentUserServiceGuardrailTests(
        CampFitFurDogsApiFactory factory,
        PostgresFixture fixture)
        : base(factory, fixture)
    {
    }

    private static async Task SignInAsync(HttpClient client, Guid userId)
    {
        var payload = new { sub = userId.ToString() };
        var response = await client.PostAsJsonAsync("/__test__/sign-in", payload);
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public void ShouldResolveAuthenticatedUserService()
    {
        using var scope = Factory.Services.CreateScope();

        scope.ServiceProvider
            .GetRequiredService<ICurrentUser>()
            .Should()
            .BeOfType<AuthenticatedUserService>();
    }

    [Fact]
    public void ShouldBehaveAsScopedService()
    {
        using var scope = Factory.Services.CreateScope();

        var s1 = scope.ServiceProvider.GetRequiredService<ICurrentUser>();
        var s2 = scope.ServiceProvider.GetRequiredService<ICurrentUser>();

        s1.Should().BeSameAs(s2);
    }

    [Fact]
    public void ShouldHaveSingleEffectiveRegistration()
    {
        using var scope = Factory.Services.CreateScope();

        scope.ServiceProvider
            .GetServices<ICurrentUser>()
            .Should()
            .HaveCount(1);
    }

    [Fact]
    public async Task ShouldReadUserIdFromClaims()
    {
        var expectedUserId = Guid.NewGuid();
        var client = CreateClient();

        await SignInAsync(client, expectedUserId);

        var response = await client.GetAsync("/__test__/current-user-id");
        response.EnsureSuccessStatusCode();

        var actualUserId = Guid.Parse(await response.Content.ReadAsStringAsync());
        actualUserId.Should().Be(expectedUserId);
    }
}
