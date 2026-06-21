# Query Dispatcher — Tester Guide

The Query Dispatcher capability provides a **validated, deterministic, DI‑driven query execution pipeline**.  
Testers validate that:

- queries are validated before execution  
- handlers are resolved correctly  
- validation failures surface as exceptions  
- handler execution is deterministic  
- DI registration rules are enforced  
- no handler is skipped or duplicated  

This guide describes how to test the capability end‑to‑end.

---

## 1. What Testers Validate

Testers must ensure:

- Validators run **before** handlers  
- All validators for a query are executed  
- Validation failures throw `ValidationException`  
- Handlers are resolved deterministically from DI  
- Exactly one handler exists per query  
- Query handlers return the correct type  
- Exceptions thrown by handlers propagate correctly  
- No handler is invoked if validation fails  
- Dynamic dispatch works correctly  

---

## 2. Required Test Types

### 2.1 Validation Pipeline Tests

#### What to validate

- All validators for a query are discovered  
- Validators run in DI order (not guaranteed, but consistent)  
- ValidationContext is created correctly  
- Validation failures throw `ValidationException`  
- Validation success allows handler execution  

#### Test scenarios

- Query with no validators → handler executes  
- Query with one validator → validator runs  
- Query with multiple validators → all run  
- Validator failure → handler is **not** executed  
- Validator throwing exception → dispatcher surfaces it  

---

### 2.2 Handler Resolution Tests

#### What to validate

- Dispatcher resolves the correct handler type  
- Exactly one handler exists per query  
- Missing handler → `InvalidOperationException`  
- Multiple handlers (should not happen with AutoRegistration) → DI failure  

#### Test scenarios

- Query with handler → executes  
- Query without handler → throws  
- Query with mismatched handler signature → throws  

---

### 2.3 Execution Pipeline Tests

#### Queries with return values

Validate:

- Handler returns the expected type  
- Dispatcher returns the handler’s result  
- Exceptions propagate  

#### Queries without return values  
(Not applicable — all queries return a value.)

---

### 2.4 Dynamic Dispatch Tests

The dispatcher uses:

```csharp
((dynamic)handler).HandleAsync((dynamic)query, ct)
```

Testers must ensure:

- Dynamic dispatch resolves the correct method  
- Incorrect method signatures cause failures  
- Generic constraints are respected  

#### Test scenarios

- Correct handler signature → success  
- Incorrect signature → runtime binder exception  
- Handler returning null → test should fail  

---

### 2.5 DI Lifetime Tests

Handlers and dispatcher are **scoped**.

Testers must validate:

- New scope → new handler instance  
- Same scope → same handler instance  
- Validators follow DI lifetime rules  

---

### 2.6 Error Propagation Tests

Testers must ensure:

- Validation errors → `ValidationException`  
- Handler errors → propagate unchanged  
- DI resolution errors → propagate unchanged  
- No errors are swallowed  

#### Test scenarios

- Handler throws domain exception → dispatcher surfaces it  
- Handler throws unexpected exception → dispatcher surfaces it  
- Validator throws → dispatcher surfaces it  

---

## 3. Test Isolation Requirements

- Use fake handlers  
- Use fake validators  
- Do not rely on real DI containers outside test scope  
- Use a fresh DI scope per test  
- Avoid static/global state  
- Avoid shared service providers across tests  

---

## 4. Recommended Testing Patterns

### 4.1 Fake Validators

```csharp
public sealed class FakeValidator : AbstractValidator<TestQuery>
{
    public FakeValidator()
    {
        RuleFor(x => x.Value).NotEmpty();
    }
}
```

Validate:

- Validator runs  
- Validation failure stops handler  

---

### 4.2 Fake Handlers

```csharp
public sealed class FakeHandler : IQueryHandler<TestQuery, string>
{
    public Task<string> HandleAsync(TestQuery query, CancellationToken ct)
        => Task.FromResult("ok");
}
```

Validate:

- Handler is invoked  
- Result is returned  

---

### 4.3 Handler Invocation Tracking

```csharp
public sealed class TrackingHandler : IQueryHandler<TestQuery, string>
{
    public bool Called { get; private set; }

    public Task<string> HandleAsync(TestQuery query, CancellationToken ct)
    {
        Called = true;
        return Task.FromResult("done");
    }
}
```

Validate:

- Called = true when validation succeeds  
- Called = false when validation fails  

---

### 4.4 Multiple Validator Tests

Ensure:

- All validators run  
- All errors are aggregated into a single `ValidationException`  

---

### 4.5 Missing Handler Tests

Validate:

- Dispatcher throws when no handler is registered  
- Error message is clear  

---

### 4.6 Wrong Handler Signature Tests

Validate:

- Handler with wrong generic signature causes runtime failure  
- Dispatcher does not silently skip handlers  

---

## 5. Anti‑Patterns (Tests Must Reject)

- Tests that rely on handler execution order (only one handler is allowed)  
- Tests that assume validators run in a specific order  
- Tests that swallow exceptions  
- Tests that rely on static/global state  
- Tests that assume DI registration order matters  
- Tests that allow multiple handlers for the same query  

---

## 6. Summary

Testers ensure that the Query Dispatcher:

- runs validators before handlers  
- resolves handlers deterministically  
- enforces DI registration rules  
- executes queries correctly  
- propagates errors safely  
- behaves consistently across scopes  
- never executes handlers when validation fails  

This Tester Guide covers everything needed to validate the Query Dispatcher capability end‑to‑end.
