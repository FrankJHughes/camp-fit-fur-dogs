using FluentAssertions;

namespace CampFitFurDogs.Architecture.Tests;

public class ApplicationPurityGuardrailTests
{
    [Fact]
    public void Application_Should_Not_Reference_Infrastructure_Or_Api()
    {
        var appAssembly = typeof(CampFitFurDogs.Application.DependencyInjection.DependencyInjection).Assembly;

        var forbiddenPrefixes = new[]
        {
            "CampFitFurDogs.Infrastructure",
            "CampFitFurDogs.Api",
            "Microsoft.EntityFrameworkCore"
        };

        var offenders = appAssembly
            .GetTypes()
            .SelectMany(t =>
                t.GetFields().Select(f => f.FieldType.Namespace)
                .Concat(t.GetProperties().Select(p => p.PropertyType.Namespace))
                .Concat(t.GetInterfaces().Select(i => i.Namespace))
                .Concat(new[] { t.BaseType?.Namespace })
            )
            .Where(ns => ns != null)
            .Where(ns => forbiddenPrefixes.Any(f => ns!.StartsWith(f)))
            .Distinct()
            .ToList();

        offenders.Should().BeEmpty("Application must not reference Infrastructure or API");
    }
}
