using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using CampFitFurDogs.TestUtilities.Factories;
using CampFitFurDogs.TestUtilities.Fixtures;

namespace CampFitFurDogs.Integration.Tests.Customers;

[Collection("API Collection")]
public class CreateCustomer_DuplicateEmailTests
{
    private readonly CampFitFurDogsApiFactory _factory;
    private readonly HttpClient _client;

    public CreateCustomer_DuplicateEmailTests(ApiFactoryFixture factoryFixture, PostgresFixture postgresFixture)
    {
        _factory = factoryFixture.Factory;
        _factory.UseContainer(postgresFixture.Container);
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task CreateCustomer_Fails_WhenEmailAlreadyExists()
    {
        var email = $"dup-{Guid.NewGuid()}@example.com";

        var request = new
        {
            FirstName = "Frank",
            LastName = "Hughes",
            Email = email,
            Phone = "916-555-1234",
            Password = "SuperSecure123!"
        };

        //
        // 1. First request succeeds
        //
        var first = await _client.PostAsJsonAsync("/api/customers", request);
        first.StatusCode.Should().Be(HttpStatusCode.Created);

        //
        // 2. Second request fails with 409 + ProblemDetails
        //
        var second = await _client.PostAsJsonAsync("/api/customers", request);
        second.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var problem = await second.Content.ReadFromJsonAsync<ProblemDetails>();

        problem.Should().NotBeNull();
        problem!.Title.Should().Be("Duplicate Email");
        problem.Detail.Should().Contain(email);
        problem.Status.Should().Be(409);
    }
}
