using FluentAssertions;

namespace CampFitFurDogs.Architecture.Tests;

public class DomainExceptionPurityGuardrailTests
{
    [Fact]
    public void Domain_Exceptions_Should_Remain_Pure()
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

        var exceptions = domainAssembly
            .GetTypes()
            .Where(t => typeof(Exception).IsAssignableFrom(t))
            .ToList();

        foreach (var ex in exceptions)
        {
            var namespaces = ex
                .GetConstructors()
                .SelectMany(c => c.GetParameters().Select(p => p.ParameterType.Namespace))
                .Where(ns => ns != null)
                .Distinct();

            var offenders = namespaces
                .Where(ns => ns != null && forbiddenPrefixes.Any(f => ns!.StartsWith(f)))
                .ToList();

            offenders.Should().BeEmpty($"{ex.Name} must remain pure");
        }
    }
}
