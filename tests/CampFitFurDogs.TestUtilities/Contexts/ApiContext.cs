using System.Reflection;
using Frank.Testing.Contexts;

namespace CampFitFurDogs.TestUtilities.Contexts;

public sealed record ApiContext : MutatedWebApplicationContext<ApiContext>
{
    public ApiContext()
    {
        EndpointAssemblies =
        [
            ..base.EndpointAssemblies,
            typeof(CampFitFurDogs.TestUtilities.AssemblyMarker).Assembly
        ];
    }
}
