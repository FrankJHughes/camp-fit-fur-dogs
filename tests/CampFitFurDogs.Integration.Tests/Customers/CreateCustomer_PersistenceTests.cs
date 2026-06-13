using System.Net;
using System.Net.Http.Json;
using CampFitFurDogs.Domain.Customers;
using CampFitFurDogs.Infrastructure.Data;
using CampFitFurDogs.TestUtilities;
using CampFitFurDogs.TestUtilities.Contexts;
using CampFitFurDogs.TestUtilities.Factories;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using Xunit;

namespace CampFitFurDogs.Integration.Tests.Customers;

public class CreateCustomer_PersistenceTests : IAsyncLifetime
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
    // TEST: CUSTOMER IS PERSISTED CORRECTLY
    // ------------------------------------------------------------
    [Fact]
    public async Task CreateCustomer_PersistsCustomerInDatabase()
    {
        var email = $"persist-test-{Guid.NewGuid()}@example.com";

        var request = new
        {
            FirstName = "Frank",
            LastName = "Hughes",
            Email = email,
            Phone = "916-555-1234",
            Password = "SuperSecure123!"
        };

        //
        // 1. Send request
        //
        var response = await _client.PostAsJsonAsync("/api/customers", request);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        //
        // 2. Extract ID from Location header
        //
        var id = response.Headers.Location!.ToString().Split('/').Last();
        var customerId = CustomerId.From(Guid.Parse(id));

        //
        // 3. Load persisted entity
        //
        using var scope = _api.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var customer = await db.Set<Customer>().FindAsync(customerId);

        //
        // 4. Assertions
        //
        customer.Should().NotBeNull();

        customer!.Id.Value.Should().Be(customerId.Value);
        customer.Email.Value.Should().Be(email.ToLowerInvariant());
        customer.FirstName.Value.Should().Be("Frank");
        customer.LastName.Value.Should().Be("Hughes");
        customer.Phone!.Value.Should().Be(PhoneNumber.From("916-555-1234").Value);

        // Password should be hashed (bcrypt)
        customer.PasswordHash!.Value.Should().NotBe(request.Password);
        customer.PasswordHash.Value.Should().MatchRegex(@"^\$2[aby]\$");
    }
}
