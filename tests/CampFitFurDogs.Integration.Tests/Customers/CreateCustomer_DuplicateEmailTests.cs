using System.Net;
using System.Net.Http.Json;
using CampFitFurDogs.TestUtilities.Factories;
using CampFitFurDogs.TestUtilities.Fixtures;
using FluentAssertions;
using Frank.Abstractions.ExceptionHandling;

[Collection("API With Postgres")]
public class CreateCustomer_DuplicateEmailTests : ApiWithPostgresTestBase
{
    public CreateCustomer_DuplicateEmailTests(
        CampFitFurDogsApiFactory factory,
        PostgresFixture fixture)
        : base(factory, fixture)
    {
    }

    [Fact]
    public async Task CreateCustomer_Fails_WhenEmailAlreadyExists()
    {
        var client = CreateClient();

        var email = $"dup-{Guid.NewGuid()}@example.com";

        var request = new
        {
            FirstName = "Frank",
            LastName = "Hughes",
            Email = email,
            Phone = "916-555-1234",
            Password = "SuperSecure123!"
        };

        var first = await client.PostAsJsonAsync("/api/customers", request);
        first.StatusCode.Should().Be(HttpStatusCode.Created);

        var second = await client.PostAsJsonAsync("/api/customers", request);
        second.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var problem = await second.Content.ReadFromJsonAsync<ProblemDetails>();

        problem.Should().NotBeNull();
        problem!.Title.Should().Be("Duplicate Email");
        problem.Detail.Should().Contain(email);
        problem.Status.Should().Be(409);
    }
}
