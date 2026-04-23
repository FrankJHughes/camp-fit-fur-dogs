using FluentAssertions;

namespace CampFitFurDogs.Architecture.Tests;

public class HandlerPurityGuardrailTests
{
    [Fact]
    public void Handlers_Should_Not_Depend_On_Infrastructure()
    {
        var appAssembly = typeof(CampFitFurDogs.Application.AssemblyMarker).Assembly;

        var handlerTypes = appAssembly
            .GetTypes()
            .Where(t => t.Name.EndsWith("Handler"))
            .ToList();

        var forbiddenPrefixes = new[]
        {
            "CampFitFurDogs.Infrastructure",
            "Microsoft.EntityFrameworkCore"
        };

        foreach (var handler in handlerTypes)
        {
            var namespaces = handler
                .GetConstructors()
                .SelectMany(c => c.GetParameters().Select(p => p.ParameterType.Namespace))
                .Where(ns => ns != null)
                .Distinct();

            var offenders = namespaces
                .Where(ns => forbiddenPrefixes.Any(f => ns!.StartsWith(f)))
                .ToList();

            offenders.Should().BeEmpty($"{handler.Name} must not depend directly on Infrastructure");
        }
    }
}
