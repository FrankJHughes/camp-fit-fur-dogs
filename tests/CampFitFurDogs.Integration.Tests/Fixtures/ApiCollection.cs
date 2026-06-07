using CampFitFurDogs.TestUtilities.Fixtures;

namespace CampFitFurDogs.Integration.Tests.Fixtures;

[CollectionDefinition("API Collection")]
public class ApiCollection :
    ICollectionFixture<ApiFactoryFixture>,
    ICollectionFixture<PostgresFixture>
{
}
