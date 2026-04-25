using Microsoft.AspNetCore.Routing;
using SharedKernel.Api;

namespace SharedKernel.Api.Tests.Fakes;

public sealed class FakeEndpoint2 : IEndpoint
{
    public static bool WasMapped { get; private set; }

    public static void Reset() => WasMapped = false;

    public void Map(IEndpointRouteBuilder app)
    {
        WasMapped = true;
    }
}
