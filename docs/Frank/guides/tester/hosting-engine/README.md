# Frank Hosting Engine — Tester Guide

This guide describes how to test the Hosting Engine capability end‑to‑end.  
Testers validate module ordering, activation logic, override merging, and configuration injection.

The Hosting Engine consists of:

- `HostingModuleAttribute`
- `IHostingModule`
- `HostingEngine`
- `HostingConfigurationProvider`
- `HostingOverridesConfigurationSource`
- `AddHostingEngine`

Each component has deterministic behavior that must be validated.

---

# 1. What Testers Validate

Testers must ensure:

- Hosting modules are discovered and ordered correctly  
- `IsActive` controls module participation  
- `GetConfigurationOverridesAsync` is invoked only for active modules  
- Overrides are merged deterministically (later modules win)  
- Overrides are injected into configuration via `HostingOverridesConfigurationSource`  
- No module can break the hosting pipeline  
- The engine behaves correctly with zero, one, or many modules  

---

# 2. Required Test Types

## 2.1 Module Ordering Tests

### What to validate

- Modules with `[HostingModule(order)]` are sorted ascending  
- Modules without the attribute default to order `0`  
- Mixed modules (with and without attributes) sort correctly  

### Patterns

- Create fake modules with known orders  
- Instantiate `HostingEngine` with them  
- Assert `_modules` ordering  

---

## 2.2 Activation Tests (`IsActive`)

### What to validate

- Inactive modules are skipped  
- Active modules run normally  
- `IsActive` is evaluated in order  
- No overrides are collected from inactive modules  

### Patterns

- Fake module returning `false` → ensure it is skipped  
- Fake module returning `true` → ensure overrides are collected  

---

## 2.3 Override Collection Tests

### What to validate

- `GetConfigurationOverridesAsync` is called only for active modules  
- Returned dictionaries are merged into a single dictionary  
- Keys from later modules overwrite earlier ones  

### Patterns

- Module A returns `{ "Key": "A" }`  
- Module B returns `{ "Key": "B" }`  
- Assert final override is `"B"`  

---

## 2.4 Configuration Injection Tests

### What to validate

- When merged overrides are non‑empty, a `HostingOverridesConfigurationSource` is added  
- When merged overrides are empty, nothing is added  
- The provider loads values correctly into configuration  

### Patterns

- Build a `WebApplicationBuilder`  
- Run `ApplyHostingEnvironmentConfigurationAsync`  
- Assert configuration contains expected keys  

---

## 2.5 Provider Tests

### HostingConfigurationProvider

Validate:

- `Load()` copies all key/value pairs into `Data`  
- Null values are preserved  
- No exceptions thrown for empty dictionaries  

### HostingOverridesConfigurationSource

Validate:

- `Build()` returns a `HostingConfigurationProvider`  
- Provider receives the same dictionary instance  

---

## 2.6 Negative Tests

Testers must ensure:

- Modules throwing exceptions do not crash the engine (unless intended)  
- `GetConfigurationOverridesAsync` returning null is handled or rejected  
- Duplicate keys are resolved deterministically  
- Modules without attributes still run (order = 0)  
- No module can modify the builder directly  

---

# 3. Test Isolation Requirements

- Use fake modules with deterministic behavior  
- Avoid relying on environment variables  
- Do not reuse the same `WebApplicationBuilder` across tests  
- Ensure module order is controlled explicitly  
- Avoid real configuration sources unless necessary  

---

# 4. Recommended Testing Patterns

### 4.1 Fake Hosting Modules  
Create simple in‑memory modules:

````csharp
public sealed class FakeModule : IHostingModule
{
    public bool Active { get; set; }
    public IDictionary<string, string?> Overrides { get; set; } = new Dictionary<string, string?>();

    public bool IsActive(WebApplicationBuilder builder) => Active;

    public Task<IDictionary<string, string?>> GetConfigurationOverridesAsync(WebApplicationBuilder builder)
        => Task.FromResult(Overrides);
}
````

### 4.2 Override Merging Snapshot Tests  
Validate final merged dictionary shape.

### 4.3 Configuration Injection Tests  
After applying the engine, assert:

````csharp
builder.Configuration["SomeKey"] == "SomeValue"
````

### 4.4 Ordering Tests  
Use modules with explicit `[HostingModule(order)]` attributes.

---

# 5. Anti‑Patterns (Tests Must Reject)

- Modules that throw inside `IsActive`  
- Modules that throw inside `GetConfigurationOverridesAsync`  
- Modules that mutate the builder  
- Modules that return null override dictionaries  
- Tests that rely on implicit ordering  
- Tests that assume modules run in registration order  

---

# 6. Summary

Testers ensure that the Hosting Engine:

- orders modules deterministically  
- activates modules correctly  
- merges overrides safely  
- injects configuration overrides properly  
- handles edge cases without breaking  
- behaves consistently across environments  

This unified Tester Guide covers everything needed to validate the Hosting Engine end‑to‑end.
