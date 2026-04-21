using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

using CampFitFurDogs.Api;
using CampFitFurDogs.Api.Tests.Fixtures;

namespace CampFitFurDogs.Api.Tests.Customers;

public class CreateCustomerEndpointTests : ApiTestBase
{
    private readonly HttpClient _client;

    public CreateCustomerEndpointTests(CampFitFurDogsApiFactory factory, PostgresFixture fixture)
        : base(factory, fixture)
    {
        _client = Factory.CreateClient();
    }

    [Fact]
    public async Task CreateCustomer_ShouldReturn201AndCustomerId()
    {
        var request = new
        {
            FirstName = "Frank",
            LastName = "Hughes",
            Email = $"frank-{Guid.NewGuid()}@example.com",
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
        var uniqueEmail = $"duplicate-{Guid.NewGuid()}@example.com";

        var request = new
        {
            FirstName = "Frank",
            LastName = "Hughes",
            Email = uniqueEmail,
            Phone = "555-1234",
            Password = "SuperSecure123!"
        };

        // First creation — unique email guarantees 201
        var first = await _client.PostAsJsonAsync("/api/customers", request);
        first.StatusCode.Should().Be(HttpStatusCode.Created);

        // Second creation — same email guarantees 409
        var second = await _client.PostAsJsonAsync("/api/customers", request);
        second.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    public sealed record CreateCustomerResponse(Guid CustomerId);

    // ── AC-2: Validation returns 400 with helpful messages ──

    [Fact]
    public async Task CreateCustomer_WithEmptyFirstName_Returns400()
    {
        var request = new
        {
            FirstName = "",
            LastName = "Hughes",
            Email = $"val-{Guid.NewGuid()}@example.com",
            Phone = "555-1234",
            Password = "SuperSecure123!"
        };

        var response = await _client.PostAsJsonAsync("/api/customers", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateCustomer_WithEmptyLastName_Returns400()
    {
        var request = new
        {
            FirstName = "Frank",
            LastName = "",
            Email = $"val-{Guid.NewGuid()}@example.com",
            Phone = "555-1234",
            Password = "SuperSecure123!"
        };

        var response = await _client.PostAsJsonAsync("/api/customers", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateCustomer_WithInvalidEmail_Returns400()
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
    }

    [Fact]
    public async Task CreateCustomer_WithEmptyPhone_Returns400()
    {
        var request = new
        {
            FirstName = "Frank",
            LastName = "Hughes",
            Email = $"val-{Guid.NewGuid()}@example.com",
            Phone = "",
            Password = "SuperSecure123!"
        };

        var response = await _client.PostAsJsonAsync("/api/customers", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateCustomer_WithEmptyPassword_Returns400()
    {
        var request = new
        {
            FirstName = "Frank",
            LastName = "Hughes",
            Email = $"val-{Guid.NewGuid()}@example.com",
            Phone = "555-1234",
            Password = ""
        };

        var response = await _client.PostAsJsonAsync("/api/customers", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateCustomer_ValidationError_ContainsHelpfulMessage()
    {
        var request = new
        {
            FirstName = "",
            LastName = "Hughes",
            Email = $"val-{Guid.NewGuid()}@example.com",
            Phone = "555-1234",
            Password = "SuperSecure123!"
        };

        var response = await _client.PostAsJsonAsync("/api/customers", request);
        var body = await response.Content.ReadAsStringAsync();

        // EG-02: No blame — message should guide, not accuse
        body.Should().ContainEquivalentOf("error");
        body.Should().NotContainEquivalentOf("exception");
        body.Should().NotContainEquivalentOf("stack");
    }

    // ── AC-4: Successful creation confirms next steps ──

    [Fact]
    public async Task CreateCustomer_SuccessResponse_ContainsCustomerId()
    {
        var request = new
        {
            FirstName = "Frank",
            LastName = "Hughes",
            Email = $"ac4-{Guid.NewGuid()}@example.com",
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
    public async Task CreateCustomer_SuccessResponse_HasLocationHeader()
    {
        var request = new
        {
            FirstName = "Frank",
            LastName = "Hughes",
            Email = $"ac4-loc-{Guid.NewGuid()}@example.com",
            Phone = "555-1234",
            Password = "SuperSecure123!"
        };

        var response = await _client.PostAsJsonAsync("/api/customers", request);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.ToString().Should().StartWith("/api/customers/");
    }

    // ── AC-5: No internal system concepts exposed ──

    [Fact]
    public async Task CreateCustomer_SuccessResponse_DoesNotExposeInternals()
    {
        var request = new
        {
            FirstName = "Frank",
            LastName = "Hughes",
            Email = $"ac5-{Guid.NewGuid()}@example.com",
            Phone = "555-1234",
            Password = "SuperSecure123!"
        };

        var response = await _client.PostAsJsonAsync("/api/customers", request);
        var body = await response.Content.ReadAsStringAsync();

        body.Should().NotContainEquivalentOf("aggregateVersion");
        body.Should().NotContainEquivalentOf("domainEvents");
        body.Should().NotContainEquivalentOf("passwordHash");
        body.Should().NotContainEquivalentOf("EF Core");
    }

    [Fact]
    public async Task CreateCustomer_ErrorResponse_DoesNotExposeInternals()
    {
        var request = new
        {
            FirstName = "",
            LastName = "Hughes",
            Email = $"ac5-err-{Guid.NewGuid()}@example.com",
            Phone = "555-1234",
            Password = "SuperSecure123!"
        };

        var response = await _client.PostAsJsonAsync("/api/customers", request);
        var body = await response.Content.ReadAsStringAsync();

        body.Should().NotContainEquivalentOf("stackTrace");
        body.Should().NotContainEquivalentOf("innerException");
        body.Should().NotContainEquivalentOf("ArgumentException");
        body.Should().NotContainEquivalentOf("NullReferenceException");
    }

    [Fact]
    public async Task CreateCustomer_ConflictResponse_DoesNotExposeInternals()
    {
        var uniqueEmail = $"ac5-dup-{Guid.NewGuid()}@example.com";

        var request = new
        {
            FirstName = "Frank",
            LastName = "Hughes",
            Email = uniqueEmail,
            Phone = "555-1234",
            Password = "SuperSecure123!"
        };

        await _client.PostAsJsonAsync("/api/customers", request);
        var response = await _client.PostAsJsonAsync("/api/customers", request);
        var body = await response.Content.ReadAsStringAsync();

        body.Should().NotContainEquivalentOf("stackTrace");
        body.Should().NotContainEquivalentOf("EmailAlreadyExistsException");
    }
}
