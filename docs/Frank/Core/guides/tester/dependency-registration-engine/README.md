
# Frank — Guides — Tester — Registration Engine Tester Guide

This guide describes how to test the **current Registration Engine** end‑to‑end.  
Testers validate scanning, planning, validation, and DI registration behavior across the full pipeline:

- `RegistrationAttribute`
- `DiscoveryOptions`
- `Scanner`
- `Planner`
- `Validator`
- `Registrar`
- `Orchestrator`

Each component has deterministic, testable behavior.

---

# 1. What Testers Validate

Testers must ensure:

- Governed interfaces (those with `[Registration]`) are discovered correctly  
- Implementations are matched correctly (including generics)  
- DiscoveryOptions filters are applied correctly  
- Plans are generated correctly  
- Min/max registration constraints are enforced  
- Violations are surfaced with correct formatting  
- DI registrations match the plan exactly  
- Concrete type registration works when enabled  
- The orchestrator fails fast on violations  
- The orchestrator registers everything when valid  
- Ordering is deterministic and stable  

---

# 2. Required Test Types

## 2.1 Scanner Tests

### What to validate

- Interfaces with `[Registration]` are discovered  
- Interfaces without the attribute are ignored  
- Implementations are discovered correctly  
- Abstract classes and open generics are ignored  
- Implementations are matched to governed interfaces  
- Generic interfaces use generic type definition matching  
- DiscoveryOptions filters are honored  

### Patterns

- Create a synthetic test assembly with known shapes  
- Assert `RelevantInterfaceGroup` contents  
- Assert generic matching:  
  - `IThing<T>` implemented by `ThingInt : IThing<int>`  
- Assert that excluded interfaces/implementations are not scanned  

---

## 2.2 Planner Tests

### What to validate

- Planner reads `RegistrationAttribute` correctly  
- Planner groups implementations by governed interface  
- Planner produces one `Plan` per governed interface  
- Planner preserves all implementing classes  
- Planner includes attribute metadata (lifetime, min/max, concrete registration)  

### Patterns

- Provide a `RelevantInterfaceGroup` with multiple implementations  
- Assert `Plan.ImplementedInterface`  
- Assert `Plan.ImplementingClasses`  
- Assert attribute values flow into the plan  

---

## 2.3 Validator Tests

### What to validate

- Min/max constraints are enforced  
- No violations when counts are within range  
- Violations emitted when out of range  
- Multiple violations surfaced correctly  
- Validator never throws — it only returns violations  

### Patterns

- Set `MinRegistrationCount = 1`, `MaxRegistrationCount = 1`  
- Provide 0, 1, and 2 implementations  
- Assert violation count and values  

---

## 2.4 Registrar Tests

### What to validate

- Each implementing class is registered for the interface  
- Lifetime matches the attribute  
- Concrete type registration occurs when enabled  
- No duplicate registrations beyond what the plan specifies  
- DI registration order is deterministic  

### Patterns

- Build a `Plan` manually  
- Run `Registrar.Register`  
- Inspect `IServiceCollection` contents  

Example inspection:

```csharp
var descriptor = services.Single(x => x.ServiceType == typeof(IMyInterface));
Assert.Equal(ServiceLifetime.Scoped, descriptor.Lifetime);
Assert.Equal(typeof(MyImplementation), descriptor.ImplementationType);
```

---

## 2.5 Orchestrator Tests

### What to validate

- Full pipeline runs in correct order  
- Violations throw `InvalidOperationException`  
- Valid plans result in DI registrations  
- Exception message matches formatted violations  
- DiscoveryOptions are applied end‑to‑end  

### Patterns

- Provide assemblies with:  
  - valid shapes → expect success  
  - invalid shapes → expect exception  
- Assert exception message contains formatted violations  

---

# 3. Negative Tests

Testers must ensure:

- Scanner does not include abstract classes  
- Scanner does not include open generics  
- Planner does not produce null plans  
- Validator does not throw  
- Registrar does not register types not in the plan  
- Orchestrator does not swallow exceptions  
- DiscoveryOptions filters do not accidentally include/exclude types incorrectly  

---

# 4. Test Isolation Requirements

- Use isolated test assemblies or dynamically generated ones  
- Do not rely on global/static state  
- Do not reuse `IServiceCollection` across tests  
- Ensure deterministic ordering of types (control test assemblies)  
- Avoid reflection nondeterminism by controlling type names  

---

# 5. Recommended Testing Patterns

### 5.1 Synthetic Test Assemblies  
Use small, purpose‑built assemblies containing:

- 1–3 governed interfaces  
- 1–5 implementations  
- known generic shapes  
- predictable type names  

### 5.2 Reflection‑Based Test Fixtures  
Generate types dynamically to test edge cases:

- open generics  
- abstract classes  
- multiple implementations  
- conflicting min/max constraints  

### 5.3 DI Collection Inspection  
After registration, inspect:

- service type  
- implementation type  
- lifetime  
- number of registrations  

### 5.4 Snapshot Testing for Violation Formatting  
Ensure violation messages remain stable and readable.

---

# 6. Anti‑Patterns (Tests Must Reject)

- Interfaces marked with `[Registration]` but not included in scanning  
- Implementations not matched due to generic mismatch  
- Plans with incorrect implementing classes  
- Missing violations when counts are out of range  
- Incorrect DI lifetimes  
- Missing concrete type registration when enabled  
- Orchestrator silently ignoring violations  
- Tests that assume DI registration order matters (it is deterministic but not semantically meaningful)  

---

# 7. Summary

Testers ensure that the Registration Engine:

- discovers governed interfaces  
- finds all valid implementations  
- builds correct plans  
- enforces min/max constraints  
- formats violations clearly  
- registers services correctly  
- applies DiscoveryOptions correctly  
- fails fast when constraints are violated  
- behaves deterministically  

This unified Tester Guide covers everything needed to validate the **current** Registration Engine end‑to‑end.

