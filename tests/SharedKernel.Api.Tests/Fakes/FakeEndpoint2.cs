using Microsoft.AspNetCore.Routing;
using SharedKernel.Api;

namespace SharedKernel.Api.Tests.Fakes;

public sealed class FakeEndpoint2 : IEndpoint
{
    public bool WasMapped { get; private set; }

    public void Map(IEndpointRouteBuilder app)
    {
        WasMapped = true;
    }
}
