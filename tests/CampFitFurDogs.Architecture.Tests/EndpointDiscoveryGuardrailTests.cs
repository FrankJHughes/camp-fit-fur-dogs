using System.Reflection;
using FluentAssertions;

namespace CampFitFurDogs.Architecture.Tests;

public class EndpointDiscoveryGuardrailTests
{
    private static readonly Assembly ApiAssembly =
        typeof(CampFitFurDogs.Api.AssemblyMarker).Assembly;

    [Fact]
    public void Api_assembly_should_contain_at_least_one_IEndpoint_implementation()
    {
        var implementations = ApiAssembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface
                        && typeof(SharedKernel.Api.IEndpoint).IsAssignableFrom(t))
            .ToList();

        implementations.Should().NotBeEmpty(
            "the Api assembly must contain at least one IEndpoint implementation "
            + "for auto-discovery to function");
    }

    [Fact]
    public void All_endpoint_classes_should_implement_IEndpoint()
    {
        var nonConforming = ApiAssembly.GetTypes()
            .Where(t => !t.IsInterface
                        && t.Name.EndsWith("Endpoint", StringComparison.Ordinal)
                        && !typeof(SharedKernel.Api.IEndpoint).IsAssignableFrom(t))
            .Select(t => t.FullName)
            .ToList();

        nonConforming.Should().BeEmpty(
            "every class ending in 'Endpoint' must implement IEndpoint for "
            + "auto-discovery, but these do not: "
            + string.Join(", ", nonConforming));
    }
}
