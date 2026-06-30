# Frank Guides – Developer – Dependency Registration Guide

The Registration engine provides a **governed, convention‑based, validated DI registration pipeline** driven by `[Registration]` on interfaces and **explicit discovery rules** defined through `DiscoveryOptions`.

It is the foundation of Frank’s **governed, rule‑driven registration** for:

- command handlers  
- query handlers  
- event handlers  
- validators  
- endpoints  
- hosting providers  
- test seams  
- any governed interface pattern  

---

# 1. RegistrationAttribute

```csharp
[AttributeUsage(AttributeTargets.Interface, Inherited = false)]
public sealed class RegistrationAttribute(ServiceLifetime lifetime) : Attribute
{
    public ServiceLifetime Lifetime { get; } = lifetime;
    public int MaxRegistrationCount { get; set; } = int.MaxValue;
    public int MinRegistrationCount { get; set; } = 0;
    public bool RegisterConcreteType { get; init; } = false;
}
```

**Semantics:**

- **Lifetime** — DI lifetime for all implementations  
- **MinRegistrationCount / MaxRegistrationCount** — required bounds  
- **RegisterConcreteType** — also register the concrete class  

Any interface marked with `[Registration]` becomes a **governed contract**.

---

# 2. DiscoveryOptions

`DiscoveryOptions` defines **what to include** and **what to exclude** during scanning.

```csharp
public sealed class DiscoveryOptions
{
    internal List<Func<TypeInfo, bool>> InterfaceInclusionPredicates { get; } = [];
    internal List<Func<TypeInfo, bool>> InterfaceExclusionPredicates { get; } = [];
    internal List<Func<TypeInfo, bool>> ImplementationInclusionPredicates { get; } = [];
    internal List<Func<TypeInfo, bool>> ImplementationExclusionPredicates { get; } = [];

    public DiscoveryOptions IncludeInterface(Func<TypeInfo, bool> predicate) { ... }
    public DiscoveryOptions ExcludeInterface(Func<TypeInfo, bool> predicate) { ... }
    public DiscoveryOptions IncludeImplementation(Func<TypeInfo, bool> predicate) { ... }
    public DiscoveryOptions ExcludeImplementation(Func<TypeInfo, bool> predicate) { ... }

    internal bool ShouldIncludeInterface(TypeInfo iface) =>
        InterfaceInclusionPredicates.Any(p => p(iface))
        && !InterfaceExclusionPredicates.Any(p => p(iface));

    internal bool ShouldIncludeImplementation(TypeInfo impl) =>
        ImplementationInclusionPredicates.Any(p => p(impl))
        && !ImplementationExclusionPredicates.Any(p => p(impl));
}
```

**Key properties of DiscoveryOptions:**

- **Inclusion is opt‑in**  
- **Exclusion overrides inclusion**  
- **Predicates operate on `TypeInfo`**  
- **Separate pipelines for interfaces and implementations**  

---

# 3. Scanner

```csharp
public static class Scanner
{
    public static IEnumerable<RelevantInterfaceGroup> Scan(
        IEnumerable<Assembly> assemblies,
        DiscoveryOptions options)
    {
        // Phase 1: discover interfaces
        var interfaces = assemblies
            .Distinct()
            .SelectMany(a => a.DefinedTypes)
            .Where(t => t.IsInterface)
            .Where(options.ShouldIncludeInterface)
            .ToList();

        // Phase 2: discover implementations
        var implementations = assemblies
            .Distinct()
            .SelectMany(a => a.DefinedTypes)
            .Where(IsConcreteClassType)
            .Where(options.ShouldIncludeImplementation)
            .SelectMany(
                ct => ct.ImplementedInterfaces,
                (ct, iface) => new Implementation(ct, iface))
            .ToList();

        // Phase 3: left‑join interfaces → implementations
        return interfaces
            .LeftJoin(
                implementations,
                iface => GetComparisonKey(iface.AsType()),
                impl => GetComparisonKey(impl.ImplementedInterface),
                (iface, impl) => (iface, impl))
            .GroupBy(
                row => row.iface,
                row => row.impl,
                (iface, impls) =>
                    new RelevantInterfaceGroup(
                        iface,
                        impls.Where(i => i != null)!));
    }

    private static bool IsConcreteClassType(TypeInfo t) =>
        t.IsClass && !t.IsAbstract && !t.ContainsGenericParameters;

    private static Type GetComparisonKey(Type type) =>
        type.IsGenericType ? type.GetGenericTypeDefinition() : type;
}
```

**Scanner responsibilities:**

- Enumerate all interfaces in the scan boundary  
- Apply inclusion/exclusion rules  
- Enumerate all concrete classes  
- Match implementations to governed interfaces  
- Produce `RelevantInterfaceGroup` objects  

---

# 4. Planner

```csharp
public static class Planner
{
    public static List<Plan> Plan(IEnumerable<RelevantInterfaceGroup> groups) => ...
}
```

**Responsibilities:**

- Read `[Registration]` metadata  
- Build a `Plan` per governed interface  
- Associate implementations with their interface  
- Preserve generic‑definition matching rules  

---

# 5. Validator

```csharp
public sealed class Validator
{
    public static IReadOnlyList<Violation> SurfaceViolations(List<Plan> plans) => ...
}
```

**Responsibilities:**

- Compare actual implementation count to min/max  
- Emit `Violation` objects when constraints fail  
- Ensure platform‑level correctness  

---

# 6. Formatter

```csharp
public sealed class Formatter
{
    public static string Format(IReadOnlyList<Violation> violations) => ...
}
```

**Responsibilities:**

- Produce human‑readable error messages  
- Include interface name, required range, actual count, and implementation list  

---

# 7. Registrar

```csharp
public sealed class Registrar
{
    public static void Register(IServiceCollection services, IEnumerable<Plan> plans) => ...
}
```

**Responsibilities:**

- Register each implementation for each governed interface  
- Respect lifetime from `[Registration]`  
- Optionally register concrete types  

---

# 8. Orchestrator

```csharp
public sealed class Orchestrator
{
    public static void Orchestrate(
        IServiceCollection services,
        IEnumerable<Assembly> assemblies,
        DiscoveryOptions discoveryOptions)
    {
        var groups = Scanner.Scan(assemblies, discoveryOptions);
        var plans = Planner.Plan(groups);
        var violations = Validator.SurfaceViolations(plans!);

        if (violations.Count > 0)
            throw new InvalidOperationException(Formatter.Format(violations));

        Registrar.Register(services, plans);
    }
}
```

**Pipeline:**

1. Scan  
2. Plan  
3. Validate  
4. Fail fast  
5. Register  

---

# 9. AddRegistration (Pattern Used by AddFrankCommand)

```csharp
public static IServiceCollection AddFrankCommand(
    this IServiceCollection services,
    IEnumerable<Assembly> assemblies,
    Action<DiscoveryOptions>? configure = null)
{
    services.AddScoped<ICommandDispatcher, CommandDispatcher>();

    var options = new DiscoveryOptions();

    options.IncludeInterface(iface =>
        HasRegistrationAttribute(iface) &&
        iface.IsGenericType &&
        (
            iface.GetGenericTypeDefinition() == typeof(ICommandHandler<>) ||
            iface.GetGenericTypeDefinition() == typeof(ICommandHandler<,>)
        ));

    options.IncludeImplementation(impl =>
        impl.ImplementedInterfaces.Any(i =>
            i.IsGenericType &&
            (
                i.GetGenericTypeDefinition() == typeof(ICommandHandler<>) ||
                i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>)
            )));

    configure?.Invoke(options);

    Orchestrator.Orchestrate(
        services,
        [typeof(Frank.AssemblyMarker).Assembly, .. assemblies],
        options);

    return services;
}
```

This is the **canonical pattern** for all Frank subsystems.

---

# 10. Developer Responsibilities

- Mark interfaces with `[Registration]`  
- Provide implementations in scanned assemblies  
- Configure `DiscoveryOptions` to include the correct interfaces and implementations  
- Respect min/max constraints  
- Avoid manual DI registration for governed interfaces  

---

# 11. Invariants & Anti‑Patterns

**Invariants:**

- Inclusion is opt‑in  
- Exclusion overrides inclusion  
- Governed interfaces must meet min/max counts  
- Generic interfaces matched by definition  
- Only concrete, non‑abstract classes are considered  

**Anti‑patterns:**

- Forgetting to include assemblies  
- Forgetting to include interface predicates  
- Forgetting to include implementation predicates  
- Marking interfaces with `[Registration]` but providing no implementations  
- Manually registering governed interfaces  

---
