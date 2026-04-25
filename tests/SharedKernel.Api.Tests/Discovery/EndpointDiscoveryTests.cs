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
        FakeEndpoint.Reset();
        FakeEndpoint2.Reset();

        var assembly = typeof(FakeEndpoint).Assembly;
        EndpointDiscovery.AddEndpoints(assembly);

        var routeBuilder = new FakeRouteBuilder();
        EndpointDiscovery.MapEndpoints(routeBuilder);

        FakeEndpoint.WasMapped.Should().BeTrue();
        FakeEndpoint2.WasMapped.Should().BeTrue();
    }

    [Fact]
    public void MapDiscoveredEndpoints_invokes_Map_on_each_discovered_endpoint()
    {
        FakeEndpoint.Reset();
        FakeEndpoint2.Reset();

        var assembly = typeof(FakeEndpoint).Assembly;
        EndpointDiscovery.AddEndpoints(assembly);

        var routeBuilder = new FakeRouteBuilder();
        EndpointDiscovery.MapEndpoints(routeBuilder);

        FakeEndpoint.WasMapped.Should().BeTrue();
        FakeEndpoint2.WasMapped.Should().BeTrue();
    }
}
