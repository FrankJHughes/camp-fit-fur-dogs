using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

using CampFitFurDogs.Api;

namespace CampFitFurDogs.Api.Tests.Customers;

public class CreateCustomerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public CreateCustomerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateCustomer_ShouldReturn201AndCustomerId()
    {
        var request = new
        {
            FirstName = "Frank",
            LastName = "Hughes",
            Email = "frank@example.com",
            Phone = "555-1234",
            Password = "SuperSecure123!"
        };

        var response = await _client.PostAsJsonAsync("/api/customers", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var body = await response.Content.ReadFromJsonAsync<CreateCustomerResponse>();
        body.Should().NotBeNull();
        body!.CustomerId.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task CreateCustomer_ShouldReturn409_WhenEmailAlreadyExists()
    {
        var request = new
        {
            FirstName = "Frank",
            LastName = "Hughes",
            Email = "duplicate@example.com",
            Phone = "555-1234",
            Password = "SuperSecure123!"
        };

        // First creation
        var first = await _client.PostAsJsonAsync("/api/customers", request);
        first.StatusCode.Should().Be(HttpStatusCode.Created);

        // Second creation
        var second = await _client.PostAsJsonAsync("/api/customers", request);
        second.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    public sealed record CreateCustomerResponse(Guid CustomerId);
}
