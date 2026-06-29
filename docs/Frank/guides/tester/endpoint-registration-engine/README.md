
# Frank — Guides — Tester — Endpoint Registration Guide  
*How to test the current Endpoint Registration Engine.*

This guide documents how to test the **current, DI‑driven, Registration Engine–powered Endpoint Registration Engine** in Frank.

The Endpoint Registration Engine provides a **governed, deterministic mechanism** for discovering, instantiating, and mapping API endpoints using the **Registration Engine pipeline**:

- `Scanner`
- `Planner`
- `Validator`
- `Registrar`
- `Orchestrator`

Testers validate the **behavior of the engine itself** — not the behavior of the endpoints.

---

# 1. Current Architecture (Authoritative)

The modern endpoint system consists of:

- `IEndpoint` — contract for endpoint modules  
- `[Registration]` — governs endpoint interfaces  
- `DiscoveryOptions.IncludeInterface` — selects governed endpoint interfaces  
- `DiscoveryOptions.IncludeImplementation` — selects implementing classes  
- `Orchestrator.Orchestrate` — performs scanning, planning, validation, registration  
- DI activation — endpoints are instantiated via DI  
- Scoped lifetime — endpoints are scoped by default  
- `MapFrankEndpoints(app)` — maps all registered endpoints  

This is the system testers must validate.

---

# 2. What Exists Today (Testable Behavior)

### **Discovery**
- Only interfaces decorated with `[Registration]` are governed  
- Only classes implementing governed interfaces are included  
- Abstract classes and open generics are ignored  
- Discovery is deterministic (based on assembly order + type name order)  
- DiscoveryOptions filters must be honored  

### **Instantiation**
- Endpoints are instantiated via **DI**, not `Activator.CreateInstance`  
- Constructor injection is supported  
- Endpoints must be resolvable by DI  
- Endpoints must be stateless  

### **Mapping**
- Each endpoint’s `Map` method is invoked  
- Mapping order follows DI resolution order  
- Mapping is deterministic  
- Mapping is idempotent  

### **Registration Engine Behavior**
- Min/max constraints are enforced  
- Violations throw `InvalidOperationException`  
- Lifetime is determined by `[Registration]`  
- Concrete type registration occurs when enabled  

---

# 3. Required Test Types (Current Engine)

## 3.1 Registration Engine Tests (Endpoint‑Specific)

Validate:

- `[Registration]` is required for endpoint interfaces  
- Implementations are discovered correctly  
- Generic interface matching works  
- Min/max constraints are enforced  
- Violations are formatted correctly  
- DI registrations match the plan exactly  

### Example

```csharp
options.IncludeInterface(iface =>
    iface.IsGenericType &&
    iface.GetGenericTypeDefinition() == typeof(IEndpoint));
```

---

## 3.2 Discovery Tests

Validate:

- Only governed endpoint interfaces are included  
- Only classes implementing governed interfaces are included  
- Abstract classes are ignored  
- Open generics are ignored  
- DiscoveryOptions filters are honored  
- Discovery is deterministic  

### Example

```csharp
Orchestrator.Orchestrate(services, assemblies, options);
Assert.Contains(typeof(TestEndpoint), DiscoveredImplementations());
```

---

## 3.3 DI Activation Tests

Validate:

- Endpoints are instantiated via DI  
- Constructor injection works  
- Missing dependencies cause predictable DI failures  
- Scoped lifetime is respected  

### Example

```csharp
var endpoint = provider.GetRequiredService<IEndpoint>();
Assert.NotNull(endpoint);
```

---

## 3.4 Mapping Invocation Tests

Validate:

- `Map` is called exactly once per endpoint  
- Mapping order follows DI resolution order  
- Mapping does not throw  
- Mapping is idempotent  

### Example

```csharp
var endpoint = provider.GetRequiredService<TestEndpoint>();
endpoint.Map(app);
Assert.True(endpoint.WasMapped);
```

---

## 3.5 Integration Tests (Engine + Minimal API)

Validate:

- Endpoints register routes correctly  
- Routes appear in the endpoint data source  
- No duplicate routes are created  
- Mapping does not break middleware ordering  

---

# 4. Anti‑Patterns (Tests Must Reject)

Tests must reject assumptions about features that **do not exist yet**:

- automatic assembly scanning  
- endpoint ordering beyond DI order  
- grouping or versioning  
- metadata conventions  
- logging or diagnostics  
- endpoint statefulness  
- non‑DI activation  
- implicit discovery without `[Registration]`  

Tests must reflect the **current governed, DI‑driven engine**, not the deprecated one.

---

# 5. Summary

**Current State — Testers validate:**

- governed endpoint discovery  
- DI‑based endpoint instantiation  
- constructor injection  
- endpoint mapping  
- idempotency  
- statelessness  
- correct handling of invalid endpoint types  
- Registration Engine correctness (Scanner → Planner → Validator → Registrar → Orchestrator)

**Future Intent — Testers will validate:**

- grouping and versioning  
- automatic assembly scanning  
- metadata conventions  
- diagnostics and observability  

This Tester Guide reflects the **current, modern Endpoint Registration Engine** and prepares testers for future enhancements.

