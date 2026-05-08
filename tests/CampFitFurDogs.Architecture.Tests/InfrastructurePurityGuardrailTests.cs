using FluentAssertions;

namespace CampFitFurDogs.Architecture.Tests;

public class InfrastructurePurityGuardrailTests
{
    [Fact]
    public void Infrastructure_Should_Not_Reference_Api_Or_Application_Abstractions()
    {
        var infraAssembly = typeof(CampFitFurDogs.Infrastructure.ServiceCollectionExtensions).Assembly;

        var forbiddenPrefixes = new[]
        {
            "CampFitFurDogs.Api",
            "CampFitFurDogs.Application." // blocks Application.*, but we will allow Abstractions explicitly
        };

        var allowedPrefixes = new[]
        {
            "CampFitFurDogs.Application.Abstractions"
        };

        var offenders = infraAssembly
            .GetTypes()
            .SelectMany(t =>
                t.GetFields().Select(f => f.FieldType.Namespace)
                .Concat(t.GetProperties().Select(p => p.PropertyType.Namespace))
                .Concat(t.GetInterfaces().Select(i => i.Namespace))
                .Concat(new[] { t.BaseType?.Namespace })
            )
            .Where(ns => ns != null)
            .Where(ns =>
                forbiddenPrefixes.Any(f => ns!.StartsWith(f)) &&
                !allowedPrefixes.Any(a => ns!.StartsWith(a))
            )
            .Distinct()
            .ToList();

        offenders.Should().BeEmpty("Infrastructure must not reference API or Application.Abstractions");
    }
}
