# Immutable Context Builder Architecture — Developer Guide

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

- step engines  
- mutable context objects  
- ordered step lists  
- ad‑hoc orchestration logic  
- dispatcher pipelines for non‑CQRS flows  

Builders provide:

- determinism  
- purity  
- immutability  
- strong typing  
- testability  
- governance alignment  

Builders are orchestration primitives, not business logic containers.

---

# 2. Conceptual Model

````text
TRequest
    ↓
ImmutableContextBuilder
    ↓
TContext (immutable snapshots)
    ↓
TResult
````

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

- accept an immutable request  
- create an initial immutable context  
- apply a deterministic sequence of transformations  
- validate invariants at each step  
- produce an immutable result  
- never mutate state  
- never depend on runtime ordering  
- never depend on environment variables  
- never perform HTTP (Application builders)  
- never perform persistence (Frank builders)  

Builders are **pure orchestration**, not logic containers.

---

# 4. What Builders Replace

Builders replace:

- step engines  
- mutable contexts  
- ordered step lists  
- dispatcher pipelines for non‑CQRS flows  
- large procedural “god methods”  

Builders eliminate ordering bugs, mutation bugs, and state corruption.

---

# 5. What Builders Do *Not* Replace

Builders do **not** replace:

- CQRS handlers  
- domain logic  
- infrastructure logic  
- API endpoints  
- hosting modules  
- startup modules  

Builders are for **pure, deterministic, synchronous transformations**.

---

# 6. Builder Interface

A builder implements:

````csharp
public interface IImmutableContextBuilder<TRequest, TContext, TResult>
{
    Task<TResult> BuildAsync(TRequest request, CancellationToken ct);
}
````

Internally, a builder:

1. creates an initial context  
2. applies a sequence of pure transformations  
3. produces a final result  

Each transformation:

- accepts a context  
- returns a new context  
- never mutates the old one  

---

# 7. Lifecycle

The builder lifecycle is:

````text
1. Validate request
2. Create initial context
3. Apply transformations in deterministic order
4. Validate invariants at each step
5. Produce final result
````

Each step produces a new immutable context snapshot.

Builders must not:

- skip steps  
- reorder steps  
- conditionally remove steps  
- mutate previous contexts  

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

- ordering bugs  
- step bugs  
- accidental data loss  
- inconsistent state  

---

# 9. Error Handling

Builders must:

- fail fast  
- fail deterministically  
- fail with invariant‑specific errors  
- never swallow exceptions  
- never return partial contexts  

Errors must be:

- contextual  
- immutable  
- predictable  

---

# 10. Examples

## 10.1 Frank Authentication Callback Pipeline

````text
FrankAuthCallbackRequest
    → OidcAuthCallbackContext
        → FrankAuthCallbackResult
````

Frank pipeline performs:

- configuration validation  
- token exchange  
- userinfo retrieval  
- claims normalization  

All pure protocol logic.

---

## 10.2 Application Authentication Callback Pipeline

````text
ApplicationAuthCallbackRequest
    → ApplicationAuthCallbackContext
        → ApplicationAuthCallbackResult
````

Application pipeline performs:

- identity resolution  
- session creation  
- cookie value computation  
- redirect computation  

All pure business logic.

---

# 11. Testing Strategy

## 11.1 Unit Tests  
Test each transformation in isolation.

## 11.2 Builder Tests  
Test the full builder end‑to‑end.

## 11.3 Guardrail Tests  
Ensure:

- immutability  
- no mutation  
- no overwriting  
- no clearing  
- deterministic behavior  

## 11.4 Integration Tests  
Used when builders participate in API flows (e.g., authentication callback).

---

# 12. When to Use a Builder

Use a builder when:

- you have a multi‑stage transformation  
- each stage is pure  
- each stage depends on the previous stage  
- you want deterministic, testable behavior  
- you want strong typing  
- you want to avoid dispatcher pipelines  

Examples:

- authentication callback  
- payment provider callback  
- webhook processing  
- multi‑stage validation  
- multi‑stage enrichment  
- multi‑stage normalization  
- multi‑stage session creation  
- multi‑stage redirect logic  

---

# 13. When *Not* to Use a Builder

Do **not** use a builder when:

- performing CQRS  
- performing domain logic  
- performing infrastructure logic  
- performing HTTP in Application  
- performing persistence in Frank  
- needing branching or workflow semantics  
- needing long‑running processes  

Builders are for **pure, deterministic, synchronous transformations**.

---

# 14. Summary

`ImmutableContextBuilder` is a core architectural primitive.

It provides:

- determinism  
- purity  
- immutability  
- strong typing  
- testability  
- safety  
- governance alignment  

It is the foundation for all new multi‑stage flows.
