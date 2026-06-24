using Frank.Abstractions;
using Microsoft.AspNetCore.Routing;

namespace Frank.Api.Tests.Fakes;

public sealed class FakeEndpoint2 : IEndpoint
{
    public static bool WasMapped { get; private set; }

    public static void Reset() => WasMapped = false;

    public void Map(IEndpointRouteBuilder app)
    {
        WasMapped = true;
    }
}
