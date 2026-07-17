# Startup Engine — Tester Guide

This guide describes how to test the Startup Engine capability end‑to‑end.  
Testers validate module ordering, builder‑phase behavior, app‑phase behavior, DI availability rules, and deterministic execution.

The Startup Engine consists of:

- `IStartupModule`
- `StartupModuleAttribute`
- `StartupEngine`
- `AddStartupEngine`

Each component has deterministic behavior that must be validated.

---

## 1. What Testers Validate

Testers must ensure:

- Startup modules are ordered correctly  
- `Add()` is executed for all modules in order  
- `Use()` is executed for all modules in order  
- Modules without attributes default to order `1000`  
- DI is **not** available during the Add phase  
- DI **is** available during the Use phase  
- Exceptions inside modules surface clearly  
- The engine does not skip modules  
- The engine does not reorder modules based on DI registration order  

---

## 2. Required Test Types

### 2.1 Module Ordering Tests

#### What to validate

- `[StartupModule(order)]` controls ordering  
- Modules without attributes default to `1000`  
- Ordering is stable and deterministic  

#### Patterns

- Create fake modules with known orders  
- Instantiate `StartupEngine` with them  
- Assert `_modules` ordering  

---

### 2.2 Add Phase Tests (`AddAll`)

#### What to validate

- All modules’ `Add()` methods are called  
- They are called in sorted order  
- Exceptions propagate with module context  
- **DI is not available** during this phase  

#### Patterns

- Fake modules that record call order  
- Fake module that throws → assert exception message includes module name  
- Attempting to resolve services inside `Add()` should fail  

Example invalid behavior to test against:

````csharp
// ❌ INVALID — DI container not built yet
var svc = builder.Services.BuildServiceProvider().GetRequiredService<IMyService>();
````

---

### 2.3 Use Phase Tests (`UseAll`)

#### What to validate

- All modules’ `Use()` methods are called  
- They are called in sorted order  
- Exceptions propagate with module context  
- **DI is available** during this phase  

#### Patterns

- Fake modules that record call order  
- Fake module that resolves a service successfully  
- Fake module that throws → assert exception message includes module name  

---

### 2.4 Attribute Defaulting Tests

#### What to validate

- Modules without `[StartupModule]` receive order `1000`  
- Mixed modules sort correctly  

#### Patterns

- Module A: `[StartupModule(10)]`  
- Module B: no attribute  
- Module C: `[StartupModule(5)]`  
- Expected order: C → A → B  

---

### 2.5 DI Availability Tests

#### Add Phase

- Attempting to resolve services must fail  
- Attempting to call `BuildServiceProvider()` must be flagged as invalid  
- Only service registration should occur  

#### Use Phase

- Service resolution should succeed  
- Middleware and endpoints should be allowed  

---

### 2.6 Negative Tests

Testers must ensure:

- `Add()` throwing stops the pipeline  
- `Use()` throwing stops the pipeline  
- Modules are not silently skipped  
- Ordering does not depend on DI registration order  
- Null modules are not allowed (DI should prevent this)  
- Modules do not attempt to resolve services during Add  

---

## 3. Test Isolation Requirements

- Use fake modules with deterministic behavior  
- Do not rely on environment variables  
- Do not reuse the same `WebApplicationBuilder` across tests  
- Ensure module order is controlled explicitly  
- Avoid real middleware or real services in tests  
- Avoid building the DI container during Add phase  

---

## 4. Recommended Testing Patterns

### 4.1 Fake Startup Modules

````csharp
public sealed class FakeModule : IStartupModule
{
    public List<string> Calls { get; } = new();

    public void Add(WebApplicationBuilder builder)
        => Calls.Add("Add");

    public void Use(WebApplication app)
        => Calls.Add("Use");
}
````

### 4.2 Order‑Recording Modules

Give each module a name and record the order of calls.

### 4.3 Exception‑Throwing Modules

Validate that:

- The engine surfaces the exception  
- The exception identifies the module  
- No further modules run after the failure  

### 4.4 DI Availability Tests

#### Add Phase

````csharp
public void Add(WebApplicationBuilder builder)
{
    Assert.Throws<InvalidOperationException>(() =>
        builder.Services.BuildServiceProvider().GetRequiredService<IMyService>());
}
````

#### Use Phase

````csharp
public void Use(WebApplication app)
{
    var svc = app.Services.GetRequiredService<IMyService>(); // should succeed
}
````

### 4.5 Builder/App Separation Tests

Ensure:

- `Add()` is called before the app is built  
- `Use()` is called after the app is built  

---

## 5. Anti‑Patterns (Tests Must Reject)

- Tests that rely on DI registration order  
- Tests that assume modules run conditionally (Startup Engine has no activation logic)  
- Tests that assume modules can be skipped  
- Tests that assume ordering is alphabetical or by type name  
- Tests that swallow exceptions thrown by modules  
- Tests that allow DI resolution during Add phase  
- Tests that allow `BuildServiceProvider()` inside Add  

---

## 6. Summary

Testers ensure that the Startup Engine:

- orders modules deterministically  
- executes all `Add()` methods in order  
- executes all `Use()` methods in order  
- enforces DI availability rules  
- surfaces module failures clearly  
- behaves consistently across environments  
- does not depend on DI registration order  

This unified Tester Guide covers everything needed to validate the Startup Engine end‑to‑end.
