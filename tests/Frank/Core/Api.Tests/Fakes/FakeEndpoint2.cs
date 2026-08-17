#nullable enable

using Frank.Core.Application.Abstractions.Endpoints;
using Microsoft.AspNetCore.Routing;

namespace Frank.Core.Api.Tests.Fakes;

public sealed class FakeEndpoint2 : IEndpoint
{
    public static bool WasMapped { get; private set; }

    public static void Reset() => WasMapped = false;

    public void Map(RouteGroupBuilder api)
    {
        WasMapped = true;
    }
}
