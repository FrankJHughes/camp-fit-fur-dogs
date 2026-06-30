using FluentAssertions;
using Frank.Api.Endpoints;
using Frank.Api.Tests.Fakes;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Frank.Api.Tests.Extensions;

public sealed class EndpointMappingExtensionsTests
{
    [Fact]
    public void MapFrankEndpoints_invokes_Map_on_each_discovered_endpoint()
    {
        // Arrange
        FakeEndpoint.Reset();

        var services = new ServiceCollection();

        services.AddFrankEndpoints([
            typeof(FakeEndpoint).Assembly
        ]);

        var provider = services.BuildServiceProvider();

        var routeBuilder = new FakeRouteBuilder(provider);

        // Act
        routeBuilder.MapFrankEndpoints();

        // Assert
        FakeEndpoint.WasMapped.Should().BeTrue();
    }
}
