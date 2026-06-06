using System.Net;
using System.Net.Http.Json;
using CampFitFurDogs.Api.Tests.Fixtures;
using CampFitFurDogs.TestUtilities.Builders;
using CampFitFurDogs.TestUtilities.Factories;
using CampFitFurDogs.TestUtilities.Fixtures;
using FluentAssertions;

namespace CampFitFurDogs.Api.Tests.Customers;

public class CreateCustomerEndpointTests : ApiTestBase
{
    private readonly HttpClient _client;

    public CreateCustomerEndpointTests(CampFitFurDogsApiFactory factory, PostgresFixture fixture)
        : base(factory, fixture)
    {
        _client = Factory.CreateClient();
    }

    public sealed record CreateCustomerResponse(Guid CustomerId);

    // ───────────────────────────────────────────────────────────────
    // AC‑1: Successful creation returns 201 + CustomerId
    // ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateCustomer_ShouldReturn201AndCustomerId()
    {
        var request = ApiRequestFixtures.Customer();

        var healthResponse = await _client.GetAsync("/api/health");
        Console.WriteLine("Health status: " + healthResponse.StatusCode);
        Console.WriteLine("Health body: " + await healthResponse.Content.ReadAsStringAsync());

        var response = await _client.PostAsJsonAsync("/api/customers", request);
        var responseBody = await response.Content.ReadAsStringAsync();
        Console.WriteLine("Response body: " + responseBody);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var body = await response.Content.ReadFromJsonAsync<CreateCustomerResponse>();
        body.Should().NotBeNull();
        body!.CustomerId.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task CreateCustomer_ShouldReturn409_WhenEmailAlreadyExists()
    {
        var email = EmailFixtures.Unique("duplicate").Value;

        var request = new CustomerBuilder()
            .WithEmail(email)
            .BuildApiRequest();

        var first = await _client.PostAsJsonAsync("/api/customers", request);
        first.StatusCode.Should().Be(HttpStatusCode.Created);

        var second = await _client.PostAsJsonAsync("/api/customers", request);
        second.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    // ───────────────────────────────────────────────────────────────
    // AC‑2: Validation returns 400 with helpful messages
    // ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateCustomer_WithEmptyFirstName_Returns400()
    {
        var request = new CustomerBuilder()
            .WithFirstName("")
            .BuildApiRequest();

        var response = await _client.PostAsJsonAsync("/api/customers", request);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateCustomer_WithEmptyLastName_Returns400()
    {
        var request = new CustomerBuilder()
            .WithLastName("")
            .BuildApiRequest();

        var response = await _client.PostAsJsonAsync("/api/customers", request);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateCustomer_WithInvalidEmail_Returns400()
    {
        var request = new CustomerBuilder()
            .WithEmail("not-an-email")
            .BuildApiRequest();

        var response = await _client.PostAsJsonAsync("/api/customers", request);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateCustomer_WithEmptyPhone_Succeeds_WhenPhoneIsOptional()
    {
        var request = new
        {
            FirstName = "Frank",
            LastName = "Hughes",
            Email = "frank@example.com",
            Phone = "",
            Password = "SuperSecure123!"
        };

        var response = await _client.PostAsJsonAsync("/api/customers", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task CreateCustomer_WithEmptyPassword_Returns400()
    {
        var request = new CustomerBuilder()
            .WithPassword("")
            .BuildApiRequest();

        var response = await _client.PostAsJsonAsync("/api/customers", request);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateCustomer_ValidationError_ContainsHelpfulMessage()
    {
        var request = new CustomerBuilder()
            .WithFirstName("")
            .BuildApiRequest();

        var response = await _client.PostAsJsonAsync("/api/customers", request);
        var body = await response.Content.ReadAsStringAsync();

        body.Should().ContainEquivalentOf("error");
        body.Should().NotContainEquivalentOf("exception");
        body.Should().NotContainEquivalentOf("stack");
    }

    // ───────────────────────────────────────────────────────────────
    // AC‑4: Successful creation confirms next steps
    // ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateCustomer_SuccessResponse_ContainsCustomerId()
    {
        var request = ApiRequestFixtures.Customer();

        var response = await _client.PostAsJsonAsync("/api/customers", request);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var body = await response.Content.ReadFromJsonAsync<CreateCustomerResponse>();
        body.Should().NotBeNull();
        body!.CustomerId.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task CreateCustomer_SuccessResponse_HasLocationHeader()
    {
        var request = ApiRequestFixtures.Customer();

        var response = await _client.PostAsJsonAsync("/api/customers", request);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.ToString().Should().StartWith("/api/customers/");
    }

    // ───────────────────────────────────────────────────────────────
    // AC‑5: No internal system concepts exposed
    // ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateCustomer_SuccessResponse_DoesNotExposeInternals()
    {
        var request = ApiRequestFixtures.Customer();

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
        var request = new CustomerBuilder()
            .WithFirstName("")
            .BuildApiRequest();

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
        var email = EmailFixtures.Unique("ac5-dup").Value;

        var request = new CustomerBuilder()
            .WithEmail(email)
            .BuildApiRequest();

        await _client.PostAsJsonAsync("/api/customers", request);

        var response = await _client.PostAsJsonAsync("/api/customers", request);
        var body = await response.Content.ReadAsStringAsync();

        body.Should().NotContainEquivalentOf("stackTrace");
        body.Should().NotContainEquivalentOf("EmailAlreadyExistsException");
    }
}
