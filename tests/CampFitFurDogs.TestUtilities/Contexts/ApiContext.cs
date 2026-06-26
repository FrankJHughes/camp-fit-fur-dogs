using Frank.Testing.Contexts;

namespace CampFitFurDogs.TestUtilities.Contexts;

public sealed record ApiContext
    : MutatedWebApplicationContext<ApiContext>
{ }
