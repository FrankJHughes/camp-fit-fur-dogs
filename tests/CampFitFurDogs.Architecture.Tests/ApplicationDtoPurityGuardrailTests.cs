using FluentAssertions;

namespace CampFitFurDogs.Architecture.Tests;

public class ApplicationDtoPurityGuardrailTests
{
    [Fact]
    public void Application_Dtos_Should_Not_Reference_Infrastructure()
    {
        var appAssembly = typeof(CampFitFurDogs.Application.AssemblyMarker).Assembly;

        var dtoTypes = appAssembly
            .GetTypes()
            .Where(t =>
                t.IsClass &&
                !t.IsAbstract &&
                (
                    t.Name.EndsWith("Dto") ||
                    t.Name.EndsWith("Model") ||
                    t.Name.EndsWith("Response")
                ))
            .ToList();

        var forbiddenPrefixes = new[]
        {
            "CampFitFurDogs.Infrastructure",
            "Microsoft.EntityFrameworkCore"
        };

        foreach (var dto in dtoTypes)
        {
            var namespaces = dto
                .GetProperties().Select(p => p.PropertyType.Namespace)
                .Concat(dto.GetFields().Select(f => f.FieldType.Namespace))
                .Where(ns => ns != null)
                .Distinct();

            var offenders = namespaces
                .Where(ns => ns != null && forbiddenPrefixes.Any(f => ns!.StartsWith(f)))
                .ToList();

            offenders.Should().BeEmpty($"{dto.Name} must not reference Infrastructure");
        }
    }
}
