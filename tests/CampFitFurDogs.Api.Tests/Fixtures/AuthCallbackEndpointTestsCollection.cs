using CampFitFurDogs.TestUtilities.Factories;
using CampFitFurDogs.TestUtilities.Fixtures;

namespace CampFitFurDogs.Api.Tests.Fixtures;

[CollectionDefinition("Auth Callback Endpoint Tests", DisableParallelization = true)]
public class AuthCallbackEndpointTestsCollection :
    ICollectionFixture<CampFitFurDogsApiFactory>,
    ICollectionFixture<PostgresFixture>
{
}
