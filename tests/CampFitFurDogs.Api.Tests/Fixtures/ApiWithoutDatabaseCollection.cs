using CampFitFurDogs.TestUtilities.Factories;

namespace CampFitFurDogs.Api.Tests.Fixtures;

[CollectionDefinition("API Without Database")]
public class ApiWithoutDatabaseCollection :
    ICollectionFixture<CampFitFurDogsApiFactory>
{
}
