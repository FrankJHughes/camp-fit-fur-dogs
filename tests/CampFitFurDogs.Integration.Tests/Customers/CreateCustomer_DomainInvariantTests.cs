using CampFitFurDogs.TestUtilities.Factories;
using CampFitFurDogs.TestUtilities.Fixtures;

namespace CampFitFurDogs.Integration.Tests.Customers;

[Collection("API With Postgres")]
public class CreateCustomer_DomainInvariantTests : ApiWithPostgresTestBase
{
    public CreateCustomer_DomainInvariantTests(
        CampFitFurDogsApiFactory factory,
        PostgresFixture fixture)
        : base(factory, fixture)
    {
    }


    // Add tests here...
}
