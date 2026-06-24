using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

using CampFitFurDogs.Infrastructure.Data;
using CampFitFurDogs.TestUtilities.Contexts;
using CampFitFurDogs.TestUtilities.Factories;

namespace CampFitFurDogs.Api.Tests.Guardrails;

public class DbContextGuardrailTests : IAsyncLifetime
{
    private PostgreSqlContainer _postgres = default!;

    public async Task InitializeAsync()
    {
        _postgres = new PostgreSqlBuilder("postgres:16-alpine").Build();
        await _postgres.StartAsync();
    }

    public async Task DisposeAsync()
    {
        if (_postgres is not null)
            await _postgres.DisposeAsync();
    }

    private ApiFactory CreateFactory()
    {
        var ctx = new ApiContext()
            .WithDatabase(true, _postgres)
            .WithCookieAuthOnly(false);

        return new ApiFactory(ctx);
    }

    // ------------------------------------------------------------
    // GUARDRAIL 1 — Should use Npgsql provider
    // ------------------------------------------------------------
    [Fact]
    public void ShouldUseNpgsqlProvider()
    {
        var factory = CreateFactory();

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        db.Database.ProviderName.Should().Contain("Npgsql");
    }

    // ------------------------------------------------------------
    // GUARDRAIL 2 — Should have exactly one DbContextOptions registration
    // ------------------------------------------------------------
    [Fact]
    public void ShouldHaveSingleEffectiveRegistration()
    {
        var factory = CreateFactory();

        using var scope = factory.Services.CreateScope();

        var all = scope.ServiceProvider
            .GetServices<DbContextOptions<AppDbContext>>()
            .ToList();

        all.Should().HaveCount(1);
    }
}
