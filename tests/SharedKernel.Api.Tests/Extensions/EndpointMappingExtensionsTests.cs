using FluentAssertions;
using SharedKernel.Api;
using SharedKernel.Api.Tests.Fakes;
using Xunit;

namespace SharedKernel.Api.Tests.Extensions;

public sealed class EndpointMappingExtensionsTests
{
    [Fact]
    public void MapDiscoveredEndpoints_calls_into_EndpointDiscovery()
    {
        var assembly = typeof(FakeEndpoint).Assembly;
        EndpointDiscovery.RegisterEndpointsFromAssembly(assembly);

        var routeBuilder = new FakeRouteBuilder();

        routeBuilder.MapDiscoveredEndpoints();

        var ep = new FakeEndpoint();
        ep.WasMapped.Should().BeTrue();
    }
}
