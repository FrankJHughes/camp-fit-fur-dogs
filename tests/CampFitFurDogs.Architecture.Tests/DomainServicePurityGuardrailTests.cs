using FluentAssertions;

namespace CampFitFurDogs.Architecture.Tests;

public class DomainServicePurityGuardrailTests
{
    [Fact]
    public void Domain_Services_Should_Not_Depend_On_External_Layers()
    {
        var domainAssembly = typeof(CampFitFurDogs.Domain.AssemblyMarker).Assembly;

        var forbiddenPrefixes = new[]
        {
            "CampFitFurDogs.Application",
            "CampFitFurDogs.Infrastructure",
            "CampFitFurDogs.Api",
            "Microsoft.EntityFrameworkCore",
            "Microsoft.Extensions",
            "System.Text.Json",
            "Newtonsoft.Json",
            "System.ComponentModel.DataAnnotations",
            "Microsoft.AspNetCore"
        };

        var domainServices = domainAssembly
            .GetTypes()
            .Where(t => t.Name.EndsWith("Service"))
            .ToList();

        foreach (var service in domainServices)
        {
            var namespaces = service
                .GetMethods()
                .SelectMany(m => m.GetParameters().Select(p => p.ParameterType.Namespace))
                .Concat(service.GetInterfaces().Select(i => i.Namespace))
                .Where(ns => ns != null)
                .Distinct();

            var offenders = namespaces
                .Where(ns => ns != null && forbiddenPrefixes.Any(f => ns!.StartsWith(f)))
                .ToList();

            offenders.Should().BeEmpty($"{service.Name} must remain pure");
        }
    }
}
