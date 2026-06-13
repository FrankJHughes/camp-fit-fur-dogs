using System.Net;
using System.Net.Http.Json;
using CampFitFurDogs.TestUtilities;
using CampFitFurDogs.TestUtilities.Contexts;
using CampFitFurDogs.TestUtilities.Factories;
using FluentAssertions;
using Testcontainers.PostgreSql;
using Xunit;

namespace CampFitFurDogs.Api.Tests.Guardrails;

public class RouteMappingGuardrailTests : IAsyncLifetime
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

    private HttpClient CreateClient()
    {
        var ctx = new ApiContext()
            .WithDatabase(true, _postgres)
            .WithCookieAuthOnly(false);

        var api = new ApiFactory(ctx);
        return api.CreateClient(new ApiClientContext());
    }

    // ------------------------------------------------------------
    // GUARDRAIL — Route must be mapped (POST /api/dogs must not 404)
    // ------------------------------------------------------------
    [Fact]
    public async Task PostDogs_ShouldNotReturn404()
    {
        var client = CreateClient();

        var request = new
        {
            Name = "Test",
            Breed = "Test",
            DateOfBirth = "2020-01-01",
            Sex = "Female"
        };

        var response = await client.PostAsJsonAsync("/api/dogs", request);

        response.StatusCode.Should().NotBe(HttpStatusCode.NotFound);
    }
}
