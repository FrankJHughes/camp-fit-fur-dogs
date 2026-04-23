using FluentAssertions;
using Microsoft.AspNetCore.Routing;
using SharedKernel.Api;
using SharedKernel.Api.Tests.Fakes;
using Xunit;

namespace SharedKernel.Api.Tests.Discovery;

public sealed class EndpointDiscoveryTests
{
    [Fact]
    public void RegisterEndpointsFromAssembly_discovers_all_IEndpoint_implementations()
    {
        var assembly = typeof(FakeEndpoint).Assembly;

        EndpointDiscovery.RegisterEndpointsFromAssembly(assembly);

        var routeBuilder = new FakeRouteBuilder();
        EndpointDiscovery.MapDiscoveredEndpoints(routeBuilder);

        var endpoints = new IEndpoint[]
        {
            new FakeEndpoint(),
            new FakeEndpoint2()
        };

        endpoints.All(e => e.WasMapped).Should().BeTrue();
    }

    [Fact]
    public void MapDiscoveredEndpoints_invokes_Map_on_each_discovered_endpoint()
    {
        var assembly = typeof(FakeEndpoint).Assembly;
        EndpointDiscovery.RegisterEndpointsFromAssembly(assembly);

        var routeBuilder = new FakeRouteBuilder();

        EndpointDiscovery.MapDiscoveredEndpoints(routeBuilder);

        var ep1 = new FakeEndpoint();
        var ep2 = new FakeEndpoint2();

        ep1.WasMapped.Should().BeTrue();
        ep2.WasMapped.Should().BeTrue();
    }
}
