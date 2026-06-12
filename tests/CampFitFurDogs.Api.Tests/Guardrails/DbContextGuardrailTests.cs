using CampFitFurDogs.Api.Tests.Fixtures;
using CampFitFurDogs.TestUtilities.Factories;
using CampFitFurDogs.TestUtilities.Fixtures;
using CampFitFurDogs.Infrastructure.Data;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CampFitFurDogs.Api.Tests.Guardrails;

[Collection("API With Postgres")]
public class DbContextGuardrailTests
    : ApiWithPostgresTestBase
{
    public DbContextGuardrailTests(CampFitFurDogsApiFactory factory, PostgresFixture fixture)
        : base(factory, fixture) { }

    [Fact]
    public void ShouldUseNpgsqlProvider()
    {
        using var scope = Factory.Services
            .GetRequiredService<IServiceScopeFactory>()
            .CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        db.Database.ProviderName.Should().Contain("Npgsql");
    }

    [Fact]
    public void ShouldHaveSingleEffectiveRegistration()
    {
        using var scope = Factory.Services
            .GetRequiredService<IServiceScopeFactory>()
            .CreateScope();

        var all = scope.ServiceProvider
            .GetServices<DbContextOptions<AppDbContext>>()
            .ToList();

        all.Should().HaveCount(1);
    }
}
