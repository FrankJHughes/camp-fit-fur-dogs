using Frank.Testing.Contexts;

namespace CampFitFurDogs.TestUtilities.Contexts;

public sealed record ApiClientContext
    : MutatedWebApplicationClientContext<ApiClientContext>
{
    public ApiClientContext()
    { }


}
