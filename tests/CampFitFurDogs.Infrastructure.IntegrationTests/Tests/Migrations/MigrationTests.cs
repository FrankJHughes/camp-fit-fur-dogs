using System.Threading.Tasks;
using Xunit;
using CampFitFurDogs.Infrastructure.IntegrationTests.Fixtures;

namespace CampFitFurDogs.Infrastructure.IntegrationTests.Tests.Migrations;

public class MigrationTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;

    public MigrationTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Can_Apply_All_Migrations()
    {
        await _fixture.DbContext.Database.EnsureCreatedAsync();
    }
}
