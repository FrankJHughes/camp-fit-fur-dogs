using System.Linq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CampFitFurDogs.Infrastructure.Data;

namespace CampFitFurDogs.Api.Tests.Guardrails;

public class TestcontainersGuardrailTests : IClassFixture<CampFitFurDogsApiFactory>
{
    private readonly CampFitFurDogsApiFactory _factory;

    public TestcontainersGuardrailTests(CampFitFurDogsApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Database_ShouldBeReachable()
    {
        using var scope = _factory.Server.Services
            .GetRequiredService<IServiceScopeFactory>()
            .CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var canConnect = await db.Database.CanConnectAsync();
        canConnect.Should().BeTrue();
    }
}
