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
        FakeEndpoint.Reset();

        var assembly = typeof(FakeEndpoint).Assembly;
        EndpointDiscovery.AddEndpoints(assembly);

        var routeBuilder = new FakeRouteBuilder();
        routeBuilder.MapEndpoints();

        FakeEndpoint.WasMapped.Should().BeTrue();
    }
}
