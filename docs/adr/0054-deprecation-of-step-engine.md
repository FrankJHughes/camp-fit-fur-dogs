# ADR‑0054 — Deprecation of Step Engine

## Status  
Accepted

## Context  
The original authentication callback implementation (US‑110, US‑111) used a **step‑engine architecture**:

- `IAuthCallbackStep` interface  
- Ordered step lists  
- Mutable callback context  
- Dispatcher‑driven execution  
- Steps mutating shared state  
- Steps depending on ordering guarantees  
- Steps mixing protocol and business logic  
- Steps leaking Infrastructure concerns into Application  

This pattern introduced several systemic problems:

### 1. **Mutable shared state**
Steps mutated a single context object, making it difficult to reason about:

- What data was set  
- When it was set  
- Whether it could be overwritten  
- Whether it could be cleared  
- Whether invariants were preserved  

### 2. **Ordering fragility**
The engine relied on strict ordering:

- If a step ran too early → missing data  
- If a step ran too late → overwritten data  
- If a step was inserted incorrectly → silent corruption  

### 3. **Cross‑layer leakage**
Steps frequently violated layering rules:

- Protocol logic inside Application  
- Business logic inside Frank  
- Persistence logic inside steps that should have been pure  

### 4. **Poor testability**
Testing required:

- Full step lists  
- Deep mocks  
- Complex harnesses  
- Manual context construction  
- Manual ordering verification  

### 5. **Difficult governance enforcement**
Guardrails could not reliably enforce:

- Purity  
- Immutability  
- Invariant correctness  
- Layering boundaries  

### 6. **Authentication callback complexity**
The authentication callback flow became increasingly fragile as more steps were added:

- Configuration validation  
- Token exchange  
- Userinfo retrieval  
- Identity resolution  
- Session creation  
- Redirect computation  

The step engine could not scale safely.

### 7. **Refactor pressure**
The introduction of the **ImmutableContextBuilder** pattern demonstrated:

- Stronger invariants  
- Pure transformations  
- Immutable state  
- Deterministic behavior  
- Clear separation of protocol vs business logic  
- Perfect testability  
- Perfect governance alignment  

The step engine became obsolete.

## Decision  
The **step engine is deprecated** and must not be used for any new development.

### 1. Step engine interfaces are deprecated
The following are now formally deprecated:

- `IAuthCallbackStep`  
- Any `*Step` classes  
- Any step lists  
- Any dispatcher‑driven step execution  
- Any mutable callback context types  

### 2. ImmutableContextBuilder replaces the step engine
All multi‑stage flows must use:

```
IImmutableContextBuilder<TRequest, TContext, TResult>
```

### 3. Authentication callback is permanently builder‑based
The canonical architecture is:

1. **Frank pipeline** — protocol  
2. **Application pipeline** — business  
3. **Api endpoint** — boundary  

### 4. No new step‑engine‑based flows may be created
All new multi‑stage transformations must use ImmutableContextBuilder.

### 5. Existing step‑engine flows must be migrated over time
Migration priority:

1. Authentication callback (completed)  
2. Identity mapping (completed)  
3. Session creation (completed)  
4. Any remaining step‑based flows (none currently)  

### 6. Guardrails enforce the deprecation
Guardrail tests must ensure:

- No new `*Step` classes  
- No new `IAuthCallbackStep` implementations  
- No new mutable context types  
- No new dispatcher‑driven step pipelines  

## Consequences  

### Positive  
- Eliminates mutable shared state  
- Eliminates ordering bugs  
- Enforces purity and immutability  
- Strongly typed request → context → result  
- Perfect alignment with Architecture Governance  
- Builders are trivial to test  
- Authentication callback is now deterministic and safe  
- Clear separation of protocol vs business logic  
- Guardrails can enforce invariants reliably  
- No more step engine complexity  

### Neutral  
- Requires developers to learn the builder pattern  
- Requires updates to documentation and conventions  

### Negative  
- Existing step‑engine flows require migration  
- Builders require more upfront structure (TRequest, TContext, TResult)  

---

## Summary  
CampFitFurDogs formally **deprecates the step engine** and standardizes **ImmutableContextBuilder** as the architectural foundation for all multi‑stage transformations.  
This decision eliminates ordering fragility, enforces immutability, improves testability, and aligns the system with modern governance and security requirements.
