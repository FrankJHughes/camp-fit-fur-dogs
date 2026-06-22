using Frank.Testing.Contexts;

namespace Frank.TestUtilities.Contexts;

public sealed record ApiClientContext : MutatedWebApplicationClientContext<ApiClientContext>
{
}
