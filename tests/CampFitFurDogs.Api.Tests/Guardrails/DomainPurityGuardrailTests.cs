using FluentAssertions;
using System.Reflection;

namespace CampFitFurDogs.Api.Tests.Guardrails;

public class DomainPurityGuardrailTests
{
    [Fact]
    public void Domain_Should_Remain_Pure()
    {
        var domainAssembly = typeof(CampFitFurDogs.Domain.AssemblyMarker).Assembly;

        // Namespaces the Domain layer must NEVER reference
        var forbiddenPrefixes = new[]
        {
            "CampFitFurDogs.Application",
            "CampFitFurDogs.Infrastructure",
            "CampFitFurDogs.Api",
            "Microsoft.EntityFrameworkCore",
            "Microsoft.Extensions.DependencyInjection",
            "Microsoft.Extensions.Logging",
            "System.Text.Json",
            "System.Text.Json.Serialization",
            "Newtonsoft.Json",
            "System.ComponentModel.DataAnnotations",
            "Microsoft.AspNetCore"
        };

        var offenders = domainAssembly
            .GetTypes()
            .SelectMany(t =>
                t.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                    .Select(f => f.FieldType.Namespace)
                .Concat(
                    t.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                        .Select(p => p.PropertyType.Namespace)
                )
                .Concat(
                    t.GetInterfaces().Select(i => i.Namespace)
                )
                .Concat(
                    new[] { t.BaseType?.Namespace }.Where(ns => ns != null)!
                )
            )
            .Where(ns => ns != null && forbiddenPrefixes.Any(f => ns.StartsWith(f)))
            .Distinct()
            .ToList();

        offenders.Should().BeEmpty(
            "Domain must remain pure and free of Application, Infrastructure, API, EF Core, DI, logging, serialization, validation, or ASP.NET dependencies");
    }
}
