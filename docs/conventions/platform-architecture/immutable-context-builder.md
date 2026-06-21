# immutable-context-builder.md

# Immutable Context Builder (Frank)

The **Immutable Context Builder** is the deterministic execution pattern used in
Frank for multi‑stage, append‑only transformations of immutable context objects.

It is currently used exclusively in:

- `ApplicationAuthCallbackContextBuilder`
- `OidcAuthCallbackCallbackContextBuilder`

These builders define the **canonical pattern** for how multi‑stage pipelines
should operate when transforming immutable contexts.

This document describes the builder pattern as it exists today and establishes
the architectural direction for future engines that may adopt it.

---

## Purpose

The Immutable Context Builder exists to:

- orchestrate multi‑stage transformations  
- enforce immutability and purity  
- guarantee deterministic execution  
- eliminate hidden state mutation  
- provide a safe, testable pipeline model  
- ensure each stage contributes predictably  

Builders are the **execution mechanism** for immutable context flows.

---

## Builder Characteristics

### 1. Deterministic  
Given the same:

- input context  
- stages  
- configuration  

the builder must always produce the same output.

### 2. Stage‑Driven  
Builders execute a sequence of stages:

```
Stage 1 → Stage 2 → Stage 3 → … → Final Context
```

Each stage:

- receives an immutable context  
- returns a **new** immutable context  
- must not mutate previous state  

### 3. Pure  
Stages must not:

- perform I/O  
- mutate shared state  
- depend on global state  
- read environment variables  
- access DI containers  

Builders enforce **pure functional transformations**.

### 4. Append‑Only  
Stages may add information but may never:

- remove values  
- override previous contributions  
- mutate existing values  

---

## Builder Lifecycle

1. Builder is created with an initial context  
2. Stages are selected (based on configuration or pipeline rules)  
3. Builder executes each stage in order  
4. Each stage returns a new context  
5. Builder validates the final context  
6. Builder returns the final immutable context  

This ensures:

- no mutation  
- no side effects  
- predictable behavior  

---

## Stage Rules

A stage must:

- accept the current immutable context  
- return a new immutable context  
- validate its own contributions  
- never mutate the input context  
- never depend on product‑specific types  

A stage must not:

- perform business logic  
- perform persistence  
- perform HTTP calls  
- read environment variables  
- weaken security guarantees  

Stages are **pure transformers**.

---

## Error Handling

Builders must:

- fail fast  
- provide actionable diagnostics  
- never allow partial execution  
- emit structured errors  

Stages must not swallow exceptions.

---

## Test Harness Integration

Builders enable:

- deterministic pipeline testing  
- stage‑level testing  
- context snapshot comparison  
- reproducible callback flows  

This is essential for US‑176 (Test Harness Migration).

---

## Architectural Direction

Although the Immutable Context Builder is currently used only for authentication
callback pipelines, it represents the **preferred pattern** for deterministic,
multi‑stage transformations.

Future adoption requires explicit stories:

- **US‑216 — Refactor HostingEngine to use ImmutabilityContext**  
- **US‑217 — Refactor StartupEngine to use ImmutabilityContext**  

These stories define the expansion path but are **not yet implemented**.

---

## Enforcement

Builder rules are enforced through:

- guardrail tests  
- stage validation  
- conventions governance  

Immutable Context Builders must remain **pure**, **deterministic**, and
**append‑only**.
