using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using CampFitFurDogs.TestUtilities.Factories;
using CampFitFurDogs.TestUtilities.Fixtures;

namespace CampFitFurDogs.Integration.Tests.Customers;

[Collection("API Collection")]
public class CreateCustomer_ValidationTests
{
    private readonly CampFitFurDogsApiFactory _factory;
    private readonly HttpClient _client;

    public CreateCustomer_ValidationTests(ApiFactoryFixture factoryFixture, PostgresFixture postgresFixture)
    {
        _factory = factoryFixture.Factory;
        _factory.UseContainer(postgresFixture.Container);

        _client = _factory.CreateClient();
    }

    // ------------------------------------------------------------
    // 1. Invalid email
    // ------------------------------------------------------------
    [Fact]
    public async Task CreateCustomer_Fails_WhenEmailIsInvalid()
    {
        var request = new
        {
            FirstName = "Frank",
            LastName = "Hughes",
            Email = "not-an-email",
            Phone = "555-1234",
            Password = "SuperSecure123!"
        };

        var response = await _client.PostAsJsonAsync("/api/customers", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        problem!.Title.Should().Be("Validation Error");
        problem.Status.Should().Be(400);
        problem.Errors.Should().ContainKey("Email");
        problem.Errors["Email"].Should().NotBeEmpty();
    }

    // ------------------------------------------------------------
    // 2. Password too short (only validated when provided)
    // ------------------------------------------------------------
    [Fact]
    public async Task CreateCustomer_Fails_WhenPasswordIsTooShort()
    {
        var request = new
        {
            FirstName = "Frank",
            LastName = "Hughes",
            Email = "frank@example.com",
            Phone = "555-555-1234",
            Password = "123" // too short
        };

        var response = await _client.PostAsJsonAsync("/api/customers", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        problem!.Title.Should().Be("Validation Error");
        problem.Status.Should().Be(400);
        problem.Errors.Should().ContainKey("Password");
        problem.Errors["Password"].Should().NotBeEmpty();
    }

    // ------------------------------------------------------------
    // 3. Required fields missing (Password is NOT required)
    // ------------------------------------------------------------
    [Fact]
    public async Task CreateCustomer_Fails_WhenRequiredFieldsAreMissing()
    {
        var request = new
        {
            FirstName = "",
            LastName = "",
            Email = "",
            Phone = "",
            Password = "" // optional, so no error expected
        };

        var response = await _client.PostAsJsonAsync("/api/customers", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        problem!.Title.Should().Be("Validation Error");
        problem.Status.Should().Be(400);

        problem.Errors.Should().ContainKey("FirstName");
        problem.Errors.Should().ContainKey("LastName");
        problem.Errors.Should().ContainKey("Email");

        // Password is OPTIONAL → should NOT appear in validation errors
        problem.Errors.Should().NotContainKey("Password");

        problem.Errors["FirstName"].Should().NotBeEmpty();
        problem.Errors["LastName"].Should().NotBeEmpty();
        problem.Errors["Email"].Should().NotBeEmpty();
    }
}
