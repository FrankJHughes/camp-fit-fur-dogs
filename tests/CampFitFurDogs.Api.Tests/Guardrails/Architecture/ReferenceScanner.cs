using System.Reflection;

namespace CampFitFurDogs.Api.Tests.Guardrails.Architecture;

public static class ReferenceScanner
{
    public static IEnumerable<string> FindForbiddenReferences(
        Assembly assembly,
        params string[] forbiddenNamespaces)
    {
        return assembly
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
                    new[] { t.BaseType?.Namespace }
                )
            )
            .Where(ns => ns != null)                 // ← filter nulls
            .Cast<string>()                           // ← safe cast
            .Where(ns => forbiddenNamespaces.Any(f => ns.StartsWith(f)))
            .Distinct();
    }
}
