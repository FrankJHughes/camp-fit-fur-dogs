# Immutable Context Builder Guide  
*(Aligned With Authentication Callback Refactor)*

`ImmutableContextBuilder<TRequest, TContext, TResult>` is a **deterministic, pure, invariant‑checked pipeline primitive** used to implement multi‑stage transformations without mutable state, step engines, or dispatcher pipelines.

It is now a **core architectural mechanism** used across:

- Frank Auth Callback Pipeline (protocol)
- Application Auth Callback Pipeline (business)
- Identity Mapping
- Session Creation
- Redirect Computation
- Any multi‑stage transformation requiring purity + determinism

This guide documents **how ImmutableContextBuilder works today**, how it is used, and when to use it.

---

# Purpose

`ImmutableContextBuilder` exists to solve a specific architectural need:

> **Perform multi‑stage transformations in a deterministic, pure, strongly‑typed, and testable way — without mutable state or step engines.**

It replaces:

- Step engines  
- Ordered step lists  
- Mutable context objects  
- Ad‑hoc orchestration logic  
- Dispatcher pipelines for non‑CQRS flows  

It provides:

- Determinism  
- Purity  
- Immutability  
- Strong typing  
- Testability  
- Governance alignment  

---

# High‑Level Model

```
TRequest
    ↓
ImmutableContextBuilder
    ↓
TContext (immutable snapshots)
    ↓
TResult
```

### TRequest  
Input parameters for the pipeline.  
Immutable.

### TContext  
Internal working state.  
Immutable snapshots created at each stage.

### TResult  
Final output.  
Immutable.

---

# Responsibilities

A builder must:

- Accept an immutable request  
- Create an initial immutable context  
- Apply a deterministic sequence of transformations  
- Produce an immutable result  
- Never mutate state  
- Never depend on runtime ordering  
- Never depend on environment variables  
- Never perform HTTP (Application builders)  
- Never perform persistence (Frank builders)  

Builders are **pure orchestration**, not business logic containers.

---

# What Builders Replace

Builders replace:

- Step engines  
- Mutable contexts  
- Ordered step lists  
- Dispatcher pipelines for non‑CQRS flows  
- “God methods” that do too much  

---

# What Builders Do *Not* Replace

Builders do **not** replace:

- CQRS handlers  
- Domain logic  
- Infrastructure logic  
- API endpoints  
- Hosting providers  

They are strictly for **pure, deterministic, synchronous transformations**.

---

# Anatomy of a Builder

A builder implements:

```csharp
public interface IImmutableContextBuilder<TRequest, TContext, TResult>
{
    Task<TResult> BuildAsync(TRequest request, CancellationToken ct);
}
```

Internally, a builder:

1. Creates an initial context  
2. Applies a sequence of pure transformations  
3. Produces a final result  

Each transformation:

- Accepts a context  
- Returns a new context  
- Never mutates the old one  

---

# Example: Frank Auth Callback Pipeline

```
FrankAuthCallbackRequest
    → OidcAuthCallbackContext
        → FrankAuthCallbackResult
```

Frank pipeline performs:

- Configuration validation  
- Token exchange  
- Userinfo retrieval  
- Claims normalization  

All pure protocol logic.

---

# Example: Application Auth Callback Pipeline

```
ApplicationAuthCallbackRequest
    → ApplicationAuthCallbackContext
        → ApplicationAuthCallbackContextBuilderResult
```

Application pipeline performs:

- Identity resolution  
- Session creation  
- Cookie value computation  
- Redirect computation  

All pure business logic.

---

# Invariant Enforcement

Builders enforce:

- **No mutation** — every state change produces a new context  
- **No clearing** — once a field is set, it cannot be unset  
- **No overwriting** — once a field is set, it cannot be replaced  
- **No nulls** — contexts must always be valid  
- **No illegal transitions** — domain invariants must hold  

This prevents:

- Ordering bugs  
- Step bugs  
- Accidental data loss  
- Inconsistent state  

---

# Testing Strategy

### Unit Tests  
Test each transformation in isolation.

### Builder Tests  
Test the full builder end‑to‑end.

### Guardrail Tests  
Ensure:
- Immutability  
- No mutation  
- No overwriting  
- No clearing  
- Deterministic behavior  

### Integration Tests  
Used when builders participate in API flows (e.g., authentication callback).

---

# When to Use a Builder

Use a builder when:

- You have a multi‑stage transformation  
- Each stage is pure  
- Each stage depends on the previous stage  
- You want deterministic, testable behavior  
- You want strong typing  
- You want to avoid dispatcher pipelines  

Examples:

- Authentication callback  
- Payment provider callback  
- Webhook processing  
- Multi‑stage validation  
- Multi‑stage enrichment  
- Multi‑stage normalization  
- Multi‑stage session creation  
- Multi‑stage redirect logic  

---

# When *Not* to Use a Builder

Do **not** use a builder when:

- You are performing CQRS  
- You are performing domain logic  
- You are performing Infrastructure logic  
- You are performing HTTP in Application  
- You are performing persistence in Frank  
- You need branching or workflow semantics  
- You need long‑running processes  

Builders are for **pure, deterministic, synchronous transformations**.

---

# Summary

`ImmutableContextBuilder` is now a **core architectural primitive**.

It provides:

- Determinism  
- Purity  
- Immutability  
- Testability  
- Safety  
- Strong typing  
- Governance alignment  

It replaces the old step engine and is the foundation for all new multi‑stage flows.

