using Xunit;

namespace CampFitFurDogs.Api.Tests.Fixtures;

[CollectionDefinition("api")]
public class ApiCollection :
    ICollectionFixture<CampFitFurDogsApiFactory>,
    ICollectionFixture<PostgresFixture>
{
}
