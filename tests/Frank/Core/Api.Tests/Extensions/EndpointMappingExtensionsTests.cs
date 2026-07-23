using Frank.Core.Api.Endpoints;
using Frank.Core.Api.Tests.Fakes;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Core.Api.Tests.Extensions;

public sealed class EndpointMappingExtensionsTests
{
    [Fact]
    public void MapFrankEndpoints_invokes_Map_on_each_discovered_endpoint()
    {
        // Arrange
        FakeEndpoint.Reset();

        var services = new ServiceCollection();

        services.AddFrankCoreApiEndpoints([
            typeof(FakeEndpoint).Assembly
        ]);

        var provider = services.BuildServiceProvider();

        var routeBuilder = new FakeRouteBuilder(provider);

        // Act
        routeBuilder.MapFrankCoreApiEndpoints();

        // Assert
        FakeEndpoint.WasMapped.Should().BeTrue();
    }
}
