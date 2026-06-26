using FluentAssertions;
using Frank.Api.Endpoints;
using Frank.Api.Tests.Fakes;
using Xunit;

namespace Frank.Api.Tests.Extensions;

public sealed class EndpointMappingExtensionsTests
{
    [Fact]
    public void MapDiscoveredEndpoints_calls_into_EndpointDiscovery()
    {
        FakeEndpoint.Reset();

        var assembly = typeof(FakeEndpoint).Assembly;
        EndpointDiscovery.AddEndpoints(assembly);

        var routeBuilder = new FakeRouteBuilder();
        routeBuilder.MapEndpoints();

        FakeEndpoint.WasMapped.Should().BeTrue();
    }
}
