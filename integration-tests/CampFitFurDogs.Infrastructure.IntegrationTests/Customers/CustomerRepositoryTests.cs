using CampFitFurDogs.Domain.Customers;
using CampFitFurDogs.Infrastructure.IntegrationTests.Fixtures;

namespace CampFitFurDogs.Infrastructure.IntegrationTests.Customers;

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
            firstName: FirstName.From("Test"),
            lastName: LastName.From("User"),
            email: Email.From("test@example.com"),
            externalId: ExternalId.From("auth0|1234567890"),
            phone: PhoneNumber.From("916-555-5555")
        );

        var customers = _fixture.DbContext.Set<Customer>();
        customers.Add(customer);
        await _fixture.DbContext.SaveChangesAsync();

        var loaded = await customers.FindAsync(customer.Id);

        Assert.NotNull(loaded);
        Assert.Equal(FirstName.From("Test"), loaded!.FirstName);
        Assert.Equal(ExternalId.From("auth0|1234567890"), loaded.ExternalId);
    }
}
