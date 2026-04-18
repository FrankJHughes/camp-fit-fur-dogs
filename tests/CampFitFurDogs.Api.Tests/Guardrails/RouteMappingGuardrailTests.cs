using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace CampFitFurDogs.Api.Tests.Guardrails;

public class RouteMappingGuardrailTests
    : GuardrailTestBase, IClassFixture<CampFitFurDogsApiFactory>
{
    public RouteMappingGuardrailTests(CampFitFurDogsApiFactory factory)
        : base(factory) { }

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