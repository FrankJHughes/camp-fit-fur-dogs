using System.Linq;
using CampFitFurDogs.Infrastructure.Data;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CampFitFurDogs.Api.Tests.Guardrails;

public class DbContextGuardrailTests
    : GuardrailTestBase, IClassFixture<CampFitFurDogsApiFactory>
{
    public DbContextGuardrailTests(CampFitFurDogsApiFactory factory)
        : base(factory) { }

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
