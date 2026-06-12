using CampFitFurDogs.TestUtilities.Factories;
using CampFitFurDogs.TestUtilities.Fixtures;

namespace CampFitFurDogs.Api.Tests.Fixtures;

[CollectionDefinition("API With Postgres")]
public class ApiWithPostgresCollection :
    ICollectionFixture<CampFitFurDogsApiFactory>,
    ICollectionFixture<PostgresFixture>
{
}
