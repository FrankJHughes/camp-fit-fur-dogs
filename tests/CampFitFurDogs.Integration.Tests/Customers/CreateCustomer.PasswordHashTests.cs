using System.Net;
using System.Net.Http.Json;
using CampFitFurDogs.Domain.Customers;
using CampFitFurDogs.Infrastructure.Data;
using CampFitFurDogs.Integration.Tests.Fixtures;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace CampFitFurDogs.Integration.Tests.Customers;

public class CreateCustomer_PasswordHashTests : IClassFixture<PostgresFixture>, IDisposable
{
    private readonly CampFitFurDogsApiFactory _factory;
    private readonly HttpClient _client;

    public CreateCustomer_PasswordHashTests(PostgresFixture db)
    {
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

        var id = response.Headers.Location!.ToString().Split('/').Last();

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var customerId = CustomerId.From(Guid.Parse(id));
        var customer = await db.Set<Customer>().FindAsync(customerId);

        customer.Should().NotBeNull();

        customer!.PasswordHash!.Value.Should().NotBe("SuperSecure123!");
        customer.PasswordHash.Value.Should().MatchRegex(@"^\$2[aby]\$");
    }
}
