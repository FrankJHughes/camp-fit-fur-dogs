using System.Net;
using System.Net.Http.Json;
using CampFitFurDogs.TestUtilities;
using CampFitFurDogs.TestUtilities.Contexts;
using CampFitFurDogs.TestUtilities.Factories;
using FluentAssertions;
using Frank.Abstractions.ExceptionHandling;
using Testcontainers.PostgreSql;
using Xunit;

namespace CampFitFurDogs.Integration.Tests.Customers;

public class CreateCustomer_DuplicateEmailTests : IAsyncLifetime
{
    private PostgreSqlContainer _postgres = default!;
    private ApiFactory _api = default!;
    private HttpClient _client = default!;

    // ------------------------------------------------------------
    // TEST INITIALIZATION
    // ------------------------------------------------------------
    public async Task InitializeAsync()
    {
        _postgres = new PostgreSqlBuilder("postgres:16-alpine").Build();
        await _postgres.StartAsync();

        var ctx = new ApiContext()
            .WithDatabase(true, _postgres)
            .WithCookieAuthOnly(false); // This endpoint is anonymous

        _api = new ApiFactory(ctx);

        _client = _api.CreateClient(new ApiClientContext());
    }

    public async Task DisposeAsync()
    {
        if (_postgres is not null)
            await _postgres.DisposeAsync();
    }

    // ------------------------------------------------------------
    // TEST: DUPLICATE EMAIL RETURNS 409 + PROBLEM DETAILS
    // ------------------------------------------------------------
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

        // First request succeeds
        var first = await _client.PostAsJsonAsync("/api/customers", request);
        first.StatusCode.Should().Be(HttpStatusCode.Created);

        // Second request fails with 409
        var second = await _client.PostAsJsonAsync("/api/customers", request);
        second.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var problem = await second.Content.ReadFromJsonAsync<ProblemDetails>();

        problem.Should().NotBeNull();
        problem!.Title.Should().Be("Duplicate Email");
        problem.Detail.Should().Contain(email);
        problem.Status.Should().Be(409);
    }
}
