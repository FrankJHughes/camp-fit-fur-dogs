using Frank.Core.Api.Endpoints;
using Frank.Core.Api.Tests.Fakes;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Core.Api.Tests.Registration;

public sealed class EndpointDiscoveryTests
{
    [Fact]
    public void AddFrankEndpoints_discovers_all_IEndpoint_implementations()
    {
        // Arrange
        FakeEndpoint.Reset();
        FakeEndpoint2.Reset();

        var services = new ServiceCollection();

        services.AddFrankCoreApiEndpoints([
            typeof(FakeEndpoint).Assembly
        ]);

        var provider = services.BuildServiceProvider();

        var routeBuilder = new FakeRouteBuilder(provider);

        // Act
        routeBuilder.MapRegisteredApiEndpoints();

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

        services.AddFrankCoreApiEndpoints([
            typeof(FakeEndpoint).Assembly
        ]);

        var provider = services.BuildServiceProvider();

        var routeBuilder = new FakeRouteBuilder(provider);

        // Act
        routeBuilder.MapRegisteredApiEndpoints();

        // Assert
        FakeEndpoint.WasMapped.Should().BeTrue();
        FakeEndpoint2.WasMapped.Should().BeTrue();
    }
}
