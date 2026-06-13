using System.Net;
using System.Net.Http.Json;
using CampFitFurDogs.Domain.Customers;
using CampFitFurDogs.Infrastructure.Data;
using CampFitFurDogs.TestUtilities;
using CampFitFurDogs.TestUtilities.Contexts;
using CampFitFurDogs.TestUtilities.Factories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using Xunit;

namespace CampFitFurDogs.Integration.Tests.Customers;

public class CreateCustomer_SuccessTests : IAsyncLifetime
{
    private PostgreSqlContainer _postgres = default!;
    private ApiFactory _api = default!;
    private HttpClient _client = default!;

    private sealed record CreateCustomerResponse(Guid CustomerId);

    // ------------------------------------------------------------
    // TEST INITIALIZATION
    // ------------------------------------------------------------
    public async Task InitializeAsync()
    {
        _postgres = new PostgreSqlBuilder("postgres:16-alpine").Build();
        await _postgres.StartAsync();

        var ctx = new ApiContext()
            .WithDatabase(true, _postgres)
            .WithCookieAuthOnly(false); // CreateCustomer is anonymous

        _api = new ApiFactory(ctx);

        _client = _api.CreateClient(new ApiClientContext());
    }

    public async Task DisposeAsync()
    {
        if (_postgres is not null)
            await _postgres.DisposeAsync();
    }

    // ------------------------------------------------------------
    // TEST: SUCCESSFUL CUSTOMER CREATION
    // ------------------------------------------------------------
    [Fact]
    public async Task CreateCustomer_SuccessfullyPersistsCustomer_AndReturns201()
    {
        var email = $"success-{Guid.NewGuid()}@Example.COM"; // test normalization

        var request = new
        {
            FirstName = "Frank",
            LastName = "Hughes",
            Email = email,
            Phone = " 916-555-1234 ", // tests trimming + normalization
            Password = "SuperSecure123!"
        };

        //
        // 1. Send request
        //
        var response = await _client.PostAsJsonAsync("/api/customers", request);

        //
        // 2. Assert HTTP response
        //
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var body = await response.Content.ReadFromJsonAsync<CreateCustomerResponse>();
        body.Should().NotBeNull();
        body!.CustomerId.Should().NotBe(Guid.Empty);

        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.ToString()
            .Should().Be($"/api/customers/{body.CustomerId}");

        //
        // 3. Load persisted entity
        //
        await using var scope = _api.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var customer = await db.Set<Customer>()
            .FirstOrDefaultAsync(c => c.Id == CustomerId.From(body.CustomerId));

        customer.Should().NotBeNull();

        //
        // 4. Domain-level assertions
        //
        customer!.FirstName.Value.Should().Be("Frank");
        customer.LastName.Value.Should().Be("Hughes");

        // Email normalized
        customer.Email.Value.Should().Be(email.ToLowerInvariant());

        // Phone normalized (10 digits → +1XXXXXXXXXX)
        customer.Phone!.Value.Should().Be("+19165551234");

        // Password hashed (not plaintext)
        customer.PasswordHash!.Value.Should().NotBe(request.Password);
        customer.PasswordHash.Verify(request.Password).Should().BeTrue();
    }
}
