using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using CampFitFurDogs.Infrastructure.Data;
using CampFitFurDogs.Integration.Tests.Fixtures;
using CampFitFurDogs.Domain.Customers;

namespace CampFitFurDogs.Integration.Tests.Customers;

public class CreateCustomer_PersistenceTests : IClassFixture<PostgresFixture>, IDisposable
{
    private readonly CampFitFurDogsApiFactory _factory;
    private readonly HttpClient _client;
    private readonly PostgresFixture _db;

    public CreateCustomer_PersistenceTests(PostgresFixture db)
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
    public async Task CreateCustomer_PersistsCustomerInDatabase()
    {
        var request = new
        {
            FirstName = "Frank",
            LastName = "Hughes",
            Email = "persist-test@example.com",
            Phone = "555-1234",
            Password = "SuperSecure123!"
        };

        var response = await _client.PostAsJsonAsync("/api/customers", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var id = response.Headers.Location!.ToString().Split('/').Last();

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var customerId = CustomerId.From(Guid.Parse(id));
        var customer = await db.Set<Customer>().FindAsync(customerId);

        customer.Should().NotBeNull();
        customer!.Email.Value.Should().Be("persist-test@example.com");
        customer.FirstName.Should().Be("Frank");
        customer.LastName.Should().Be("Hughes");
    }
}
