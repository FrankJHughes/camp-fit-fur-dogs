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

public class CreateCustomer_PasswordHashTests : IAsyncLifetime
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
    // TEST: PASSWORD IS HASHED, NOT RAW
    // ------------------------------------------------------------
    [Fact]
    public async Task CreateCustomer_StoresHashedPassword_NotRaw()
    {
        var request = new
        {
            FirstName = "Frank",
            LastName = "Hughes",
            Email = "hash-test@example.com",
            Phone = "916-555-1234",
            Password = "SuperSecure123!"
        };

        var response = await _client.PostAsJsonAsync("/api/customers", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        // Extract ID from Location header
        var id = response.Headers.Location!.ToString().Split('/').Last();

        // Resolve DB context from test host
        using var scope = _api.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var customerId = CustomerId.From(Guid.Parse(id));
        var customer = await db.Set<Customer>().FindAsync(customerId);

        customer.Should().NotBeNull();

        // Assert password is hashed
        customer!.PasswordHash!.Value.Should().NotBe("SuperSecure123!");
        customer.PasswordHash.Value.Should().MatchRegex(@"^\$2[aby]\$");
    }
}
