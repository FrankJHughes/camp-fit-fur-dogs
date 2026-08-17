using Frank.Testing.Factories;
using Frank.TestUtilities.Contexts;

namespace Frank.TestUtilities.Factories;

public sealed class ApiFactory
    : MutatedWebApplicationFactory<Program, ApiContext, ApiClientContext>
{
    public ApiFactory(ApiContext ctx) : base(ctx)
    {
    }
}
