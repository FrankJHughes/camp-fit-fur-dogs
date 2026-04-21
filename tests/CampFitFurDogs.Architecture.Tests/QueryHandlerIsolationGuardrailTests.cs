using System.Reflection;
using FluentAssertions;

namespace CampFitFurDogs.Architecture.Tests;

public class QueryHandlerIsolationGuardrailTests
{
    private static readonly Assembly ApplicationAssembly =
        typeof(CampFitFurDogs.Application.AssemblyMarker).Assembly;

    [Fact]
    public void Query_handlers_should_not_depend_on_repository_interfaces()
    {
        var queryHandlerInterface = typeof(
            SharedKernel.Abstractions.IQueryHandler<,>);

        var violations = ApplicationAssembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface
                        && t.GetInterfaces().Any(i =>
                            i.IsGenericType
                            && i.GetGenericTypeDefinition() == queryHandlerInterface))
            .SelectMany(handler =>
                handler.GetConstructors()
                    .SelectMany(c => c.GetParameters())
                    .Where(p => p.ParameterType.IsInterface
                                && p.ParameterType.Name.EndsWith("Repository",
                                    StringComparison.Ordinal))
                    .Select(p => $"{handler.Name} depends on {p.ParameterType.Name}"))
            .ToList();

        violations.Should().BeEmpty(
            "query handlers must use slice-scoped reader interfaces, "
            + "not aggregate repository interfaces. Violations: "
            + string.Join("; ", violations));
    }
}
