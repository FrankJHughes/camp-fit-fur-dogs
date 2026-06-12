using CampFitFurDogs.TestUtilities.Factories;

namespace CampFitFurDogs.Api.Tests.Fixtures;

[CollectionDefinition("Auth Login Endpoint Tests", DisableParallelization = true)]
public class AuthLoginEndpointTestsCollection :
    ICollectionFixture<CampFitFurDogsApiFactory>
{
}
