using FluentAssertions;
using Frank.Api.Endpoints;
using Frank.Api.Tests.Fakes;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Frank.Api.Tests.Discovery;

public sealed class EndpointDiscoveryTests
{
    [Fact]
    public void AddFrankEndpoints_discovers_all_IEndpoint_implementations()
    {
        // Arrange
        FakeEndpoint.Reset();
        FakeEndpoint2.Reset();

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
        FakeEndpoint2.WasMapped.Should().BeTrue();
    }

    [Fact]
    public void MapFrankEndpoints_invokes_Map_on_each_discovered_endpoint()
    {
        // Arrange
        FakeEndpoint.Reset();
        FakeEndpoint2.Reset();

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
        FakeEndpoint2.WasMapped.Should().BeTrue();
    }
}
