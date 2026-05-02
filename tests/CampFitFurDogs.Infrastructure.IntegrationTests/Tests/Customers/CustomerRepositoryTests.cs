using System.Threading.Tasks;
using Xunit;
using CampFitFurDogs.Infrastructure.IntegrationTests.Fixtures;
using CampFitFurDogs.Domain.Customers;

namespace CampFitFurDogs.Infrastructure.IntegrationTests.Tests.Customers;

public class CustomerRepositoryTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;

    public CustomerRepositoryTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Can_Create_And_Retrieve_Customer()
    {
        var customer = Customer.Create(
            "Test",
            "User",
            Email.From("test@example.com"),
            PhoneNumber.From("555-5555"),
            PasswordHash.Create("P@ssw0rd!")
        );

        var customers = _fixture.DbContext.Set<Customer>();
        customers.Add(customer);
        await _fixture.DbContext.SaveChangesAsync();

        var loaded = await customers.FindAsync(customer.Id);

        Assert.NotNull(loaded);
        Assert.Equal("Test", loaded!.FirstName);
    }
}
