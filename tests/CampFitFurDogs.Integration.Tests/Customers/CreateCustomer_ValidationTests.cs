using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using CampFitFurDogs.Domain.Customers;
using CampFitFurDogs.Infrastructure.Data;
using CampFitFurDogs.Integration.Tests.Fixtures;

namespace CampFitFurDogs.Integration.Tests.Customers;

public class CreateCustomer_ValidationTests : IClassFixture<PostgresFixture>, IDisposable
{
    private readonly CampFitFurDogsApiFactory _factory;
    private readonly HttpClient _client;
    private readonly PostgresFixture _db;

    public CreateCustomer_ValidationTests(PostgresFixture db)
    {
        _db = db;

        _factory = new CampFitFurDogsApiFactory();
        _factory.UseContainer(db.Container);

        _client = _factory.CreateClient();
    }

    public void Dispose()
    {
        _factory.Dispose();
        _client.Dispose();
        GC.SuppressFinalize(this);
    }

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
        problem.Errors.Should().ContainKey("Email");
    }

    [Fact]
    public async Task CreateCustomer_Fails_WhenPasswordIsTooShort()
    {
        var request = new
        {
            FirstName = "Frank",
            LastName = "Hughes",
            Email = "frank@example.com",
            Phone = "555-1234",
            Password = "123"
        };

        var response = await _client.PostAsJsonAsync("/api/customers", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problem!.Title.Should().Be("Validation Error");
        problem.Errors.Should().ContainKey("Password");
    }

    [Fact]
    public async Task CreateCustomer_Fails_WhenRequiredFieldsAreMissing()
    {
        var request = new
        {
            FirstName = "",
            LastName = "",
            Email = "",
            Phone = "",
            Password = ""
        };

        var response = await _client.PostAsJsonAsync("/api/customers", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        problem!.Title.Should().Be("Validation Error");
        problem.Errors.Should().ContainKey("FirstName");
        problem.Errors.Should().ContainKey("LastName");
        problem.Errors.Should().ContainKey("Email");
        problem.Errors.Should().ContainKey("Password");
    }
}
