using FluentAssertions;
using System.Reflection;

namespace CampFitFurDogs.Api.Tests.Guardrails;

public class NoCircularNamespaceDependenciesGuardrailTests
{
    [Fact]
    public void Should_Not_Have_Circular_Namespace_Dependencies()
    {
        var assemblies = new[]
        {
            typeof(CampFitFurDogs.Domain.AssemblyMarker).Assembly,
            typeof(CampFitFurDogs.Application.DependencyInjection.DependencyInjection).Assembly,
            typeof(CampFitFurDogs.Infrastructure.DependencyInjection).Assembly,
            typeof(CampFitFurDogs.Api.AssemblyMarker).Assembly
        };

        // Build graph: namespace -> referenced namespaces
        var graph = new Dictionary<string, HashSet<string>>();

        foreach (var asm in assemblies)
        {
            foreach (var type in asm.GetTypes())
            {
                var from = type.Namespace;
                if (from == null) continue;

                if (!graph.ContainsKey(from))
                    graph[from] = new HashSet<string>();

                var referencedNamespaces =
                    type.GetFields().Select(f => f.FieldType.Namespace)
                    .Concat(type.GetProperties().Select(p => p.PropertyType.Namespace))
                    .Concat(type.GetInterfaces().Select(i => i.Namespace))
                    .Concat(new[] { type.BaseType?.Namespace })
                    .Where(ns => ns != null && ns != from)
                    .Distinct();

                foreach (var to in referencedNamespaces)
                    graph[from].Add(to!);
            }
        }

        // DFS cycle detection
        var cycles = new List<List<string>>();
        var visited = new HashSet<string>();
        var stack = new HashSet<string>();

        bool Dfs(string node, List<string> path)
        {
            if (stack.Contains(node))
            {
                var cycleStart = path.IndexOf(node);
                if (cycleStart >= 0)
                    cycles.Add(path.Skip(cycleStart).ToList());
                return true;
            }

            if (!graph.ContainsKey(node) || visited.Contains(node))
                return false;

            visited.Add(node);
            stack.Add(node);

            foreach (var next in graph[node])
            {
                Dfs(next, path.Concat(new[] { next }).ToList());
            }

            stack.Remove(node);
            return false;
        }

        foreach (var ns in graph.Keys)
            Dfs(ns, new List<string> { ns });

        // Filter out trivial one-way references (length < 2)
        var realCycles = cycles.Where(c => c.Count >= 2).ToList();

        realCycles.Should().BeEmpty("circular namespace dependencies are forbidden");
    }
}
