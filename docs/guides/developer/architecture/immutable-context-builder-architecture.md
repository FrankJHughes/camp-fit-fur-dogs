# Immutable Context Builder Architecture

`ImmutableContextBuilder<TRequest, TContext, TResult>` is a deterministic, pure, invariant‑checked pipeline primitive used to implement multi‑stage transformations without mutable state, step engines, or dispatcher pipelines.

It is a core architectural mechanism used across:

- Frank Authentication Callback Pipeline (protocol)
- Application Authentication Callback Pipeline (business)
- Identity Mapping
- Session Creation
- Redirect Computation
- Any multi‑stage transformation requiring purity and determinism

This guide defines the builder pattern, lifecycle, invariants, purity rules, and usage guidelines.

---

# 1. Purpose

ImmutableContextBuilder exists to solve a specific architectural need:

> **Perform multi‑stage transformations in a deterministic, pure, strongly‑typed, and testable way — without mutable state or step engines.**

Builders replace:

- Step engines  
- Mutable context objects  
- Ordered step lists  
- Ad‑hoc orchestration logic  
- Dispatcher pipelines for non‑CQRS flows  

Builders provide:

- Determinism  
- Purity  
- Immutability  
- Strong typing  
- Testability  
- Governance alignment  

Builders are orchestration primitives, not business logic containers.

---

# 2. Conceptual Model

```
TRequest
    ↓
ImmutableContextBuilder
    ↓
TContext (immutable snapshots)
    ↓
TResult
```

## TRequest  
Immutable input parameters for the pipeline.

## TContext  
Immutable working state.  
Each transformation produces a new context snapshot.

## TResult  
Immutable final output.

---

# 3. Responsibilities

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

Builders are pure orchestration.

---

# 4. What Builders Replace

Builders replace:

- Step engines  
- Mutable contexts  
- Ordered step lists  
- Dispatcher pipelines for non‑CQRS flows  
- Large procedural “god methods”  

Builders eliminate ordering bugs, mutation bugs, and state corruption.

---

# 5. What Builders Do *Not* Replace

Builders do **not** replace:

- CQRS handlers  
- Domain logic  
- Infrastructure logic  
- API endpoints  
- Hosting modules  
- Startup modules  

Builders are for **pure, deterministic, synchronous transformations**.

---

# 6. Builder Interface

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

# 7. Lifecycle

The builder lifecycle is:

```
1. Validate request
2. Create initial context
3. Apply transformations in deterministic order
4. Validate invariants at each step
5. Produce final result
```

Each step produces a new immutable context snapshot.

Builders must not:

- Skip steps  
- Reorder steps  
- Conditionally remove steps  
- Mutate previous contexts  

---

# 8. Invariant Enforcement

Builders enforce strict invariants:

## 8.1 No Mutation  
Every state change produces a new context.

## 8.2 No Clearing  
Once a field is set, it cannot be unset.

## 8.3 No Overwriting  
Once a field is set, it cannot be replaced.

## 8.4 No Nulls  
Contexts must always be valid.

## 8.5 No Illegal Transitions  
Domain invariants must hold at every stage.

These invariants prevent:

- Ordering bugs  
- Step bugs  
- Accidental data loss  
- Inconsistent state  

---

# 9. Error Handling

Builders must:

- Fail fast  
- Fail deterministically  
- Fail with invariant‑specific errors  
- Never swallow exceptions  
- Never return partial contexts  

Errors must be:

- Contextual  
- Immutable  
- Predictable  

---

# 10. Examples

## 10.1 Frank Authentication Callback Pipeline

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

## 10.2 Application Authentication Callback Pipeline

```
ApplicationAuthCallbackRequest
    → ApplicationAuthCallbackContext
        → ApplicationAuthCallbackResult
```

Application pipeline performs:

- Identity resolution  
- Session creation  
- Cookie value computation  
- Redirect computation  

All pure business logic.

---

# 11. Testing Strategy

## 11.1 Unit Tests  
Test each transformation in isolation.

## 11.2 Builder Tests  
Test the full builder end‑to‑end.

## 11.3 Guardrail Tests  
Ensure:

- Immutability  
- No mutation  
- No overwriting  
- No clearing  
- Deterministic behavior  

## 11.4 Integration Tests  
Used when builders participate in API flows (e.g., authentication callback).

---

# 12. When to Use a Builder

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

# 13. When *Not* to Use a Builder

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

# 14. Summary

`ImmutableContextBuilder` is a core architectural primitive.

It provides:

- Determinism  
- Purity  
- Immutability  
- Strong typing  
- Testability  
- Safety  
- Governance alignment  

It replaces the step engine and is the foundation for all new multi‑stage flows.

