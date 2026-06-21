# Frank AutoRegistration — Tester Guide

This guide describes how to test the AutoRegistration engine end‑to‑end.  
Testers validate scanning, planning, validation, formatting, and DI registration behavior.

The engine consists of:

- `AutoRegisterAttribute`
- `Scanner`
- `Planner`
- `Validator`
- `Formatter`
- `Registrar`
- `Orchestrator`
- `AddAutoRegistration`

Each component has deterministic, testable behavior.

---

# 1. What Testers Validate

Testers must ensure:

- Relevant interfaces are discovered correctly  
- Implementations are matched correctly (including generics)  
- Plans are generated correctly  
- Min/max registration constraints are enforced  
- Violations are surfaced with correct formatting  
- DI registrations match the plan  
- Concrete type registration works when enabled  
- The orchestrator fails fast on violations  
- The orchestrator registers everything when valid  

---

# 2. Required Test Types

## 2.1 Scanner Tests

### What to validate

- Interfaces with `[AutoRegister]` are discovered  
- Interfaces without the attribute are ignored  
- Concrete classes are discovered correctly  
- Abstract classes and open generics are ignored  
- Implementations are matched to interfaces  
- Generic interfaces use generic type definition matching  

### Patterns

- Create a test assembly with known shapes  
- Assert `RelevantInterfaceGroup` contents  
- Assert generic matching:  
  - `IFoo<T>` implemented by `FooInt : IFoo<int>`  
  - Scanner must group them correctly  

---

## 2.2 Planner Tests

### What to validate

- Planner reads `AutoRegisterAttribute` correctly  
- Planner groups implementations by implemented interface  
- Planner produces one `Plan` per implemented interface  
- Planner preserves all implementing classes  

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

### Patterns

- Set `MinRegistrationCount = 1`, `MaxRegistrationCount = 1`  
- Provide 0, 1, and 2 implementations  
- Assert violation count and values  

---

## 2.4 Formatter Tests

### What to validate

- Interface names are formatted correctly  
- Generic interface names drop the backtick suffix  
- Implementing classes are listed  
- Output is readable and stable  

### Patterns

- Create a violation with:  
  - generic interface  
  - multiple implementing classes  
- Assert formatted output contains:  
  - interface name  
  - min/max/actual counts  
  - class names  

---

## 2.5 Registrar Tests

### What to validate

- Each implementing class is registered for the interface  
- Lifetime matches the attribute  
- Concrete type registration occurs when enabled  
- No duplicate registrations beyond what the plan specifies  

### Patterns

- Build a `Plan` manually  
- Run `Registrar.Register`  
- Inspect `IServiceCollection` contents  

---

## 2.6 Orchestrator Tests

### What to validate

- Full pipeline runs in correct order  
- Violations throw `InvalidOperationException`  
- Valid plans result in DI registrations  
- Exception message matches `Formatter` output  

### Patterns

- Provide assemblies with:  
  - valid shapes → expect success  
  - invalid shapes → expect exception  

---

# 3. Negative Tests

Testers must ensure:

- Scanner does not include abstract classes  
- Scanner does not include open generics  
- Planner does not produce null plans  
- Validator does not throw  
- Registrar does not register types not in the plan  
- Orchestrator does not swallow exceptions  

---

# 4. Test Isolation Requirements

- Use isolated test assemblies or dynamically generated ones  
- Do not rely on global/static state  
- Do not reuse `IServiceCollection` across tests  
- Ensure deterministic ordering of types (avoid reflection nondeterminism by controlling test assemblies)  

---

# 5. Recommended Testing Patterns

### 5.1 Synthetic Test Assemblies  
Use small, purpose‑built assemblies containing:

- 1–3 interfaces  
- 1–5 implementations  
- known generic shapes  

### 5.2 Reflection‑Based Test Fixtures  
Generate types dynamically to test edge cases.

### 5.3 DI Collection Inspection  
After registration, inspect:

- service type  
- implementation type  
- lifetime  

### 5.4 Snapshot Testing for Formatter  
Ensure violation messages remain stable.

---

# 6. Anti‑Patterns (Tests Must Reject)

- Interfaces marked with `[AutoRegister]` but not included in scanning  
- Implementations not matched due to generic mismatch  
- Plans with incorrect implementing classes  
- Missing violations when counts are out of range  
- Incorrect DI lifetimes  
- Missing concrete type registration when enabled  
- Orchestrator silently ignoring violations  

---

# 7. Summary

Testers ensure that AutoRegistration:

- discovers governed interfaces  
- finds all valid implementations  
- builds correct plans  
- enforces min/max constraints  
- formats violations clearly  
- registers services correctly  
- fails fast when constraints are violated  

This unified Tester Guide covers everything needed to validate the AutoRegistration engine end‑to‑end.
