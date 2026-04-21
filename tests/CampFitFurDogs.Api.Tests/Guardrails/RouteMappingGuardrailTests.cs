using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

using CampFitFurDogs.Api.Tests.Fixtures;

namespace CampFitFurDogs.Api.Tests.Guardrails;

public class RouteMappingGuardrailTests : ApiTestBase
{
    public RouteMappingGuardrailTests(CampFitFurDogsApiFactory factory, PostgresFixture fixture)
        : base(factory, fixture){ }

    [Fact]
    public async Task PostDogs_ShouldNotReturn404()
    {
        var client = Factory.CreateClient();

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
