using CampFitFurDogs.Api.Tests.Fixtures;
using CampFitFurDogs.Infrastructure.Data;
using CampFitFurDogs.TestUtilities.Factories;
using CampFitFurDogs.TestUtilities.Fixtures;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace CampFitFurDogs.Api.Tests.Guardrails;

[Collection("API With Postgres")]
public class TestcontainersGuardrailTests : ApiWithPostgresTestBase
{
    public TestcontainersGuardrailTests(CampFitFurDogsApiFactory factory, PostgresFixture fixture)
        : base(factory, fixture) { }

    [Fact]
    public async Task Database_ShouldBeReachable()
    {
        using var scope = Factory.Server.Services
            .GetRequiredService<IServiceScopeFactory>()
            .CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var canConnect = await db.Database.CanConnectAsync();
        canConnect.Should().BeTrue();
    }
}
