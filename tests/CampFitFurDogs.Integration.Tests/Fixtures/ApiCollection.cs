namespace CampFitFurDogs.Integration.Tests.Fixtures;

[CollectionDefinition("api")]
public class ApiCollection :
    ICollectionFixture<CampFitFurDogsApiFactory>,
    ICollectionFixture<PostgresFixture>
{
}
