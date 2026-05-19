using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using CampFitFurDogs.Domain.Customers;
using CampFitFurDogs.Infrastructure.Data;
using CampFitFurDogs.Integration.Tests.Fixtures;

namespace CampFitFurDogs.Integration.Tests.Customers;

public class CreateCustomer_DuplicateEmailTests : IClassFixture<PostgresFixture>, IDisposable
{
    private readonly CampFitFurDogsApiFactory _factory;
    private readonly HttpClient _client;
    private readonly PostgresFixture _db;

    public CreateCustomer_DuplicateEmailTests(PostgresFixture db)
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
    public async Task CreateCustomer_Fails_WhenEmailAlreadyExists()
    {
        var email = $"frank-{Guid.NewGuid()}@example.com";

        var first = new
        {
            FirstName = "Frank",
            LastName = "Hughes",
            Email = email,
            Phone = "555-1234",
            Password = "SuperSecure123!"
        };

        var second = new
        {
            FirstName = "Other",
            LastName = "Person",
            Email = email, // same email
            Phone = "555-9999",
            Password = "AnotherSecure123!"
        };

        //
        // 1. First request succeeds
        //
        var firstResponse = await _client.PostAsJsonAsync("/api/customers", first);
        firstResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        //
        // 2. Second request fails
        //
        var secondResponse = await _client.PostAsJsonAsync("/api/customers", second);

        secondResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var problem = await secondResponse.Content.ReadFromJsonAsync<ProblemDetails>();
        problem!.Title.Should().Be("Duplicate Email");
        problem.Detail.Should().Contain(email);
    }
}
