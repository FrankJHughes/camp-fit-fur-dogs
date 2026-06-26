using Frank.Testing.Contexts;
using Microsoft.Extensions.Configuration;

namespace CampFitFurDogs.TestUtilities.Contexts;

public sealed record ApiContext
    : MutatedWebApplicationContext<ApiContext>
{ }
