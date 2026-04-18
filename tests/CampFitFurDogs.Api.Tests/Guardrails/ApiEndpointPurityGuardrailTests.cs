using FluentAssertions;
using System.Reflection;

namespace CampFitFurDogs.Api.Tests.Guardrails;

public class ApiEndpointPurityGuardrailTests
{
    [Fact]
    public void Api_Endpoints_Should_Not_Return_Domain_Or_Infrastructure_Types()
    {
        var apiAssembly = typeof(CampFitFurDogs.Api.AssemblyMarker).Assembly;

        var forbiddenPrefixes = new[]
        {
            "CampFitFurDogs.Domain",
            "CampFitFurDogs.Infrastructure",
            "Microsoft.EntityFrameworkCore"
        };

        // Find all endpoint classes (static classes with Map methods)
        var endpointTypes = apiAssembly
            .GetTypes()
            .Where(t =>
                t.IsClass &&
                t.IsAbstract &&
                t.IsSealed && // static class
                t.GetMethods().Any(m => m.Name.StartsWith("Map")))
            .ToList();

        foreach (var endpoint in endpointTypes)
        {
            var mapMethods = endpoint
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(m => m.Name.StartsWith("Map"))
                .ToList();

            foreach (var method in mapMethods)
            {
                var returnType = method.ReturnType;

                // Skip void or IEndpointRouteBuilder
                if (returnType == typeof(void) ||
                    returnType.Name.Contains("IEndpointRouteBuilder"))
                    continue;

                var ns = returnType.Namespace;

                if (ns == null)
                    continue;

                var offenders = forbiddenPrefixes
                    .Where(f => ns.StartsWith(f))
                    .ToList();

                offenders.Should().BeEmpty(
                    $"{endpoint.Name}.{method.Name} returns {returnType.Name}, which leaks Domain or Infrastructure types");
            }
        }
    }
}
