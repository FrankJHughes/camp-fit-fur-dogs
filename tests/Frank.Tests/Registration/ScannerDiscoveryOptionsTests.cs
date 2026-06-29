using System.Reflection;
using FluentAssertions;
using Frank.Registration;
using Frank.Registration.Shapes;
using Xunit;

namespace Frank.Registration.Tests;

// ------------------------------------------------------------
// Test types used for scanning
// ------------------------------------------------------------

public interface IFoo { }
public interface IBar { }
public interface IBaz { }
public interface IGeneric<T> { }

public class FooImpl : IFoo { }
public class FooImpl2 : IFoo { }
public class BarImpl : IBar { }
public class BazImpl : IBaz { }
public class IgnoredImpl : IFoo { }
public class GenericImpl<T> : IGeneric<T> { }

public class ScannerDiscoveryOptionsTests
{

    private static IEnumerable<RelevantInterfaceGroup> Scan(DiscoveryOptions options)
        => Scanner.Scan(new[] { typeof(IFoo).Assembly }, options);

    // ------------------------------------------------------------
    // 1. Interface inclusion
    // ------------------------------------------------------------
    [Fact]
    public void Scanner_ShouldIncludeInterfaces_MatchingInclusionPredicate()
    {
        var options = new DiscoveryOptions()
            .IncludeInterfaces(i => i.Name is nameof(IFoo) or nameof(IBar));

        var results = Scan(options).ToList();

        results.Select(r => r.RelevantInterface.Name)
            .Should().BeEquivalentTo(new[] { nameof(IFoo), nameof(IBar) });
    }

    // ------------------------------------------------------------
    // 2. Interface exclusion
    // ------------------------------------------------------------
    [Fact]
    public void Scanner_ShouldExcludeInterfaces_MatchingExclusionPredicate()
    {
        var options = new DiscoveryOptions()
            .IncludeInterfaces(i => i.Name.StartsWith("I"))
            .ExcludeInterfaces(i => i.Name == nameof(IBaz));

        var results = Scan(options).ToList();

        results.Select(r => r.RelevantInterface.Name)
            .Should().NotContain(nameof(IBaz));
    }

    // ------------------------------------------------------------
    // 3. Implementation inclusion
    // ------------------------------------------------------------
    [Fact]
    public void Scanner_ShouldIncludeImplementations_MatchingInclusionPredicate()
    {
        var options = new DiscoveryOptions()
            .IncludeInterfaces(i => i.Name == nameof(IFoo))
            .IncludeImplementations(t => t.Name.EndsWith("Impl"));

        var results = Scan(options).ToList();

        var fooGroup = results.Single(r => r.RelevantInterface.Name == nameof(IFoo));

        fooGroup.Implementations
            .Select(i => i.ImplementingClass.Name)
            .Should().BeEquivalentTo(new[]
            {
                nameof(FooImpl),
                nameof(IgnoredImpl)
            });
    }

    // ------------------------------------------------------------
    // 4. Implementation exclusion
    // ------------------------------------------------------------
    [Fact]
    public void Scanner_ShouldExcludeImplementations_MatchingExclusionPredicate()
    {
        var options = new DiscoveryOptions()
            .IncludeInterfaces(i => i.Name == nameof(IFoo))
            .IncludeImplementations(t => t.Name.EndsWith("Impl"))
            .ExcludeImplementations(t => t.Name == nameof(IgnoredImpl));

        var results = Scan(options).ToList();

        var fooGroup = results.Single(r => r.RelevantInterface.Name == nameof(IFoo));

        fooGroup.Implementations
            .Select(i => i.ImplementingClass.Name)
            .Should().BeEquivalentTo(new[]
            {
                nameof(FooImpl)
            });
    }

    // ------------------------------------------------------------
    // 5. Interfaces with no implementations still appear
    // ------------------------------------------------------------
    [Fact]
    public void Scanner_ShouldReturnInterfaces_EvenWhenNoImplementationsMatch()
    {
        var options = new DiscoveryOptions()
            .IncludeInterfaces(i => i.Name is nameof(IFoo) or nameof(IBaz))
            .ExcludeImplementations(_ => true); // exclude all implementations

        var results = Scan(options).ToList();

        results.Select(r => r.RelevantInterface.Name)
            .Should().BeEquivalentTo(new[] { nameof(IFoo), nameof(IBaz) });

        results.SelectMany(r => r.Implementations)
            .Should().BeEmpty();
    }

    // ------------------------------------------------------------
    // 6. Multiple implementations grouped correctly
    // ------------------------------------------------------------
    [Fact]
    public void Scanner_ShouldGroupMultipleImplementations_UnderSameInterface()
    {
        var options = new DiscoveryOptions()
            .IncludeInterfaces(i => i.Name == nameof(IFoo))
            .IncludeImplementations(t => t.Name is nameof(FooImpl) or nameof(FooImpl2));

        var results = Scan(options).ToList();

        var fooGroup = results.Single(r => r.RelevantInterface.Name == nameof(IFoo));

        fooGroup.Implementations
            .Select(i => i.ImplementingClass.Name)
            .Should().BeEquivalentTo(new[]
            {
                nameof(FooImpl),
                nameof(FooImpl2)
            });
    }

    // ------------------------------------------------------------
    // 7. Generic interface matching
    // ------------------------------------------------------------
    [Fact]
    public void Scanner_ShouldMatchGenericInterfaces_ByGenericTypeDefinition()
    {
        var options = new DiscoveryOptions();

        // Match the generic interface by its generic type definition
        options.IncludeInterfaces(i =>
            i.IsGenericType &&
            i.GetGenericTypeDefinition() == typeof(IGeneric<>));

        // Match the generic implementation by its generic type definition
        options.IncludeImplementations(t =>
            t.IsGenericType &&
            t.GetGenericTypeDefinition() == typeof(GenericImpl<>));

        var results = Scan(options).ToList();

        var genericGroup = results.Single(r =>
            r.RelevantInterface.IsGenericType &&
            r.RelevantInterface.GetGenericTypeDefinition() == typeof(IGeneric<>));

        genericGroup.Implementations
            .Select(i => i.ImplementingClass.GetGenericTypeDefinition())
            .Should().ContainSingle()
            .Which.Should().Be(typeof(GenericImpl<>));
    }

    // ------------------------------------------------------------
    // 8. Multiple assemblies merged correctly
    // ------------------------------------------------------------
    [Fact]
    public void Scanner_ShouldMergeTypesAcrossAssemblies()
    {
        var thisAssembly = typeof(IFoo).Assembly;
        var mscorlib = typeof(string).Assembly; // irrelevant assembly

        var options = new DiscoveryOptions()
            .IncludeInterfaces(i => i.Name == nameof(IFoo))
            .IncludeImplementations(t => t.Name == nameof(FooImpl));

        var results = Scanner.Scan(new[] { thisAssembly, mscorlib }, options).ToList();

        var fooGroup = results.Single(r => r.RelevantInterface.Name == nameof(IFoo));

        fooGroup.Implementations
            .Select(i => i.ImplementingClass.Name)
            .Should().BeEquivalentTo(new[] { nameof(FooImpl) });
    }

    // ------------------------------------------------------------
    // Only IncludeInterface called → no results
    // ------------------------------------------------------------
    [Fact]
    public void Scanner_ShouldReturnNoResults_WhenOnlyInterfacesAreIncluded()
    {
        var options = new DiscoveryOptions()
            .IncludeInterfaces(i => i.Name == nameof(IFoo));

        var results = Scan(options).ToList();

        // No implementations included → no interface groups produced
        results.Should().HaveCount(1);
        results[0].Implementations.Should().BeEmpty();
    }

    // ------------------------------------------------------------
    // Only IncludeImplementation called → no results
    // ------------------------------------------------------------
    [Fact]
    public void Scanner_ShouldReturnNoResults_WhenOnlyImplementationsAreIncluded()
    {
        var options = new DiscoveryOptions()
            .IncludeImplementations(t => t.Name.EndsWith("Impl"));

        var results = Scan(options).ToList();

        // No interfaces included → no interface groups produced
        results.Should().BeEmpty();
    }

    // ------------------------------------------------------------
    // Neither IncludeInterfaces nor IncludeImplementations called → no results
    // ------------------------------------------------------------
    [Fact]
    public void Scanner_ShouldReturnNoResults_WhenNoIncludePredicatesAreSpecified()
    {
        var options = new DiscoveryOptions(); // no includes at all

        var results = Scan(options).ToList();

        results.Should().BeEmpty();
    }
}
