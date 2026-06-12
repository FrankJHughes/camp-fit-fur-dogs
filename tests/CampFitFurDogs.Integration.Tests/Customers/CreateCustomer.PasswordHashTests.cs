using System.Net;
using System.Net.Http.Json;
using CampFitFurDogs.Domain.Customers;
using CampFitFurDogs.Infrastructure.Data;
using CampFitFurDogs.TestUtilities.Factories;
using CampFitFurDogs.TestUtilities.Fixtures;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace CampFitFurDogs.Integration.Tests.Customers;

[Collection("API With Postgres")]
public class CreateCustomer_PasswordHashTests : ApiWithPostgresTestBase
{
    public CreateCustomer_PasswordHashTests(
        CampFitFurDogsApiFactory factory,
        PostgresFixture fixture)
        : base(factory, fixture)
    {
    }

    [Fact]
    public async Task CreateCustomer_StoresHashedPassword_NotRaw()
    {
        var client = CreateClient();

        var request = new
        {
            FirstName = "Frank",
            LastName = "Hughes",
            Email = "hash-test@example.com",
            Phone = "916-555-1234",
            Password = "SuperSecure123!"
        };

        var response = await client.PostAsJsonAsync("/api/customers", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var id = response.Headers.Location!.ToString().Split('/').Last();

        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var customerId = CustomerId.From(Guid.Parse(id));
        var customer = await db.Set<Customer>().FindAsync(customerId);

        customer.Should().NotBeNull();

        customer!.PasswordHash!.Value.Should().NotBe("SuperSecure123!");
        customer.PasswordHash.Value.Should().MatchRegex(@"^\$2[aby]\$");
    }
}
