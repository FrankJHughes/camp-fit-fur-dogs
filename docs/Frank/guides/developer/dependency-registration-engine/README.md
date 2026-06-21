# Frank AutoRegistration — Developer Guide

The AutoRegistration engine provides a **convention‑based, validated DI registration pipeline** driven by `[AutoRegister]` on interfaces.

It is composed of:

- `AutoRegisterAttribute` — marks interfaces as auto‑registration contracts  
- `Scanner` — discovers relevant interfaces and their implementations  
- `Planner` — builds registration plans per interface  
- `Validator` — enforces min/max registration counts  
- `Formatter` — formats violations into a human‑readable message  
- `Registrar` — applies plans to `IServiceCollection`  
- `Orchestrator` — coordinates the full pipeline  
- `AddAutoRegistration` — extension entrypoint for consumers  

---

## 1. AutoRegisterAttribute

````csharp
[AttributeUsage(AttributeTargets.Interface, Inherited = false)]
public sealed class AutoRegisterAttribute(ServiceLifetime lifetime) : Attribute
{
    public ServiceLifetime Lifetime { get; } = lifetime;
    public int MaxRegistrationCount { get; set; } = int.MaxValue;
    public int MinRegistrationCount { get; set; } = 0;
    public bool RegisterConcreteType { get; init; } = false;
}
````

**Semantics:**

- **Lifetime** — DI lifetime for registrations  
- **MinRegistrationCount / MaxRegistrationCount** — required bounds  
- **RegisterConcreteType** — also register the concrete class  

Any interface marked with `[AutoRegister]` becomes a **governed contract**.

---

## 2. Scanner

````csharp
public static class Scanner
{
    public static IEnumerable<RelevantInterfaceGroup> Scan(Assembly[] assemblies) => ...
}
````

**Responsibilities:**

- Find all interfaces decorated with `[AutoRegister]`  
- Find all concrete classes  
- Match implementations to interfaces (generic‑aware)  
- Produce `RelevantInterfaceGroup`  

---

## 3. Planner

````csharp
public static class Planner
{
    public static List<Plan> Plan(IEnumerable<RelevantInterfaceGroup> groups) => ...
}
````

**Responsibilities:**

- Read `[AutoRegister]` from each interface  
- Group implementations by implemented interface  
- Produce a `Plan` per interface  

---

## 4. Validator

````csharp
public sealed class Validator
{
    public static IReadOnlyList<Violation> SurfaceViolations(List<Plan> plans) => ...
}
````

**Responsibilities:**

- Compare actual implementation count to min/max  
- Emit `Violation` objects when constraints fail  

---

## 5. Formatter

````csharp
public sealed class Formatter
{
    public static string Format(IReadOnlyList<Violation> violations) => ...
}
````

**Responsibilities:**

- Convert violations into readable error messages  
- Show interface name, required range, actual count, and classes  

---

## 6. Registrar

````csharp
public sealed class Registrar
{
    public static void Register(IServiceCollection services, IEnumerable<Plan> plans) => ...
}
````

**Responsibilities:**

- Register each implementing class for each interface  
- Respect lifetime  
- Optionally register concrete types  

---

## 7. Orchestrator

````csharp
public sealed class Orchestrator
{
    public static void Orchestrate(IServiceCollection services, Assembly[] assemblies)
    {
        var groups = Scanner.Scan(assemblies);
        var plans = Planner.Plan(groups);
        var violations = Validator.SurfaceViolations(plans!);

        if (violations.Count > 0)
        {
            throw new InvalidOperationException(Formatter.Format(violations));
        }

        Registrar.Register(services, plans);
    }
}
````

**Pipeline:**

1. Scan  
2. Plan  
3. Validate  
4. Fail fast if violations  
5. Register  

---

## 8. AddAutoRegistration

````csharp
public static partial class AutoRegistrationExtensions
{
    public static IServiceCollection AddAutoRegistration(
        this IServiceCollection services,
        params Assembly[] assemblies)
    {
        Orchestrator.Orchestrate(services, assemblies);
        return services;
    }
}
````

Usage:

````csharp
services.AddAutoRegistration(typeof(SomeType).Assembly);
````

---

## 9. Developer Responsibilities

- Mark interfaces with `[AutoRegister]`  
- Provide implementations  
- Ensure assemblies are included in scanning  
- Respect min/max constraints  

---

## 10. Invariants & Anti‑Patterns

**Invariants:**

- All governed interfaces must meet min/max implementation counts  
- Violations prevent startup  
- Generic interfaces matched by definition  
- Only concrete, non‑generic classes are considered  

**Anti‑patterns:**

- Forgetting to include assemblies  
- Marking interfaces without implementations  
- Relying on manual DI registration for governed interfaces  
- Using `RegisterConcreteType` without understanding the extra registration  

