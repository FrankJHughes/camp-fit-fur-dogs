using CampFitFurDogs.TestUtilities.Fixtures;

namespace CampFitFurDogs.Api.Tests.Fixtures;

[CollectionDefinition("postgres")]
public class PostgresCollection : ICollectionFixture<PostgresFixture> { }
