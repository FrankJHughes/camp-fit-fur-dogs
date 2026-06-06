using CampFitFurDogs.TestUtilities.Factories;
using CampFitFurDogs.TestUtilities.Fixtures;

namespace CampFitFurDogs.Api.Tests.Fixtures;

[CollectionDefinition("api")]
public class ApiCollection :
    ICollectionFixture<CampFitFurDogsApiFactory>,
    ICollectionFixture<PostgresFixture>
{
}
