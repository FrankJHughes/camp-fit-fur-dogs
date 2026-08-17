# Registration

The **Registration** subsystem provides a fully automated, convention‑driven pipeline for discovering, validating, and registering services into the dependency injection container. It eliminates manual DI wiring by enforcing clear rules, predictable behavior, and compile‑time‑visible intent through attributes and structural shapes.

The system is built around four sequential phases:

1. **Scanning**  
   Assemblies are inspected to discover relevant interfaces and concrete implementations.

2. **Planning**  
   Discovered relationships are transformed into registration plans based on `RegistrationAttribute` rules.

3. **Validation**  
   Plans are checked to ensure implementation counts fall within required ranges.

4. **Registration**  
   Validated plans are applied to the DI container.

The entire pipeline is orchestrated by the `Orchestrator`.

---

## Goals

- **Zero manual DI wiring** for interfaces that opt into automatic registration.
- **Predictable, rule‑driven behavior** via `RegistrationAttribute`.
- **Clear diagnostics** when registration rules are violated.
- **Support for generic interfaces and open generic implementations.**
- **Separation of concerns**: scanning, planning, validation, and registration are isolated and testable.

---

## Key Components

### DiscoveryOptions
Configures which interfaces and implementations are included during scanning.  
Supports predicate‑based filtering for flexible inclusion rules.

---

### Scanner
Discovers:

- relevant interfaces (based on inclusion predicates)
- concrete implementations (based on inclusion predicates)
- interface → implementation relationships

Outputs `RelevantInterfaceGroup` shapes.

---

### Planner
Transforms `RelevantInterfaceGroup` items into `Plan` objects by:

- reading `RegistrationAttribute` from each interface
- grouping implementations by implemented interface (including generic variants)

---

### Validator
Evaluates each `Plan` and surfaces violations when:

```
MinRegistrationCount ≤ ActualRegistrationCount ≤ MaxRegistrationCount
```

is not satisfied.

Outputs `Violation` shapes.

---

### Registrar
Executes validated plans by adding `ServiceDescriptor` entries to the DI container.  
Supports optional concrete‑type registration.

---

### Orchestrator
Coordinates the entire pipeline:

1. Scan assemblies  
2. Plan registrations  
3. Validate plans  
4. Register services  

Throws an exception if any violations are found, using `Formatter` to produce readable diagnostics.

---

## Shapes

The Shapes folder contains immutable record types that carry data between pipeline stages:

- `Implementation`
- `ImplementedInterfaceGroup`
- `RelevantInterfaceGroup`
- `Plan`
- `Violation`

See the Shapes README for details.

---

## Usage

To enable automatic registration:

1. Apply `RegistrationAttribute` to an interface.
2. Configure `DiscoveryOptions` to include the interface and its implementations.
3. Call `Orchestrator.Orchestrate(...)` during application startup.

Example:

```csharp
Orchestrator.Orchestrate(
    services,
    AppDomain.CurrentDomain.GetAssemblies(),
    new DiscoveryOptions()
        .IncludeInterfaces(t => t.Namespace?.StartsWith("MyApp") == true)
        .IncludeImplementations(t => t.Namespace?.StartsWith("MyApp") == true)
);
```

---

## Design Principles

- **Attribute‑driven intent**  
  Interfaces explicitly opt into automatic registration.

- **Pipeline architecture**  
  Each stage is isolated, testable, and composable.

- **Immutability**  
  All shapes are records; no mutation occurs between stages.

- **Convention over configuration**  
  Minimal setup required once inclusion predicates are defined.

---

If you want, I can also generate:

- A diagram‑style architecture overview  
- A README for the entire Application layer  
- A README for the DiscoveryOptions subsystem  
- A developer guide explaining how to extend the registration pipeline

Just tell me what you want next.
