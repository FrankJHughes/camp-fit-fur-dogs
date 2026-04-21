using System.Linq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CampFitFurDogs.Infrastructure.Data;

using CampFitFurDogs.Api.Tests.Fixtures;

namespace CampFitFurDogs.Api.Tests.Guardrails;

public class TestcontainersGuardrailTests : ApiTestBase
{
    public TestcontainersGuardrailTests(CampFitFurDogsApiFactory factory, PostgresFixture fixture)
        : base(factory, fixture){ }

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
