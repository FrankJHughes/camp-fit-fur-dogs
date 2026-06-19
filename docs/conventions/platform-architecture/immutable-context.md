# immutable-context.md

# Immutable Context (Frank)

The **Immutable Context** is the foundational pattern used in Frank for
representing structured, validated, and append‑only data passed between
components that must not mutate shared state.

Today, Immutable Context is used by:

- `ApplicationAuthCallbackContext`
- `OidcAuthCallbackCallbackContext`

These contexts demonstrate the **canonical pattern** for how immutable,
strongly‑typed, deterministic data should flow through callback pipelines.

This document defines the conventions for Immutable Contexts as they exist
today, and establishes the architectural direction for future engines that may
adopt this pattern.

---

## Purpose

Immutable Contexts exist to:

- prevent hidden state mutation  
- ensure deterministic behavior  
- enforce purity across callback flows  
- provide a safe, append‑only data model  
- improve testability and reproducibility  
- guarantee that callbacks cannot corrupt shared state  

Immutable Contexts are the **contract** between callback handlers and the
platform.

---

## Characteristics of an Immutable Context

### 1. Immutable  
Once created, a context instance cannot be modified.  
All changes produce a **new** context instance.

### 2. Strongly Typed  
Contexts are not dictionaries or key/value bags.  
They are:

- typed  
- validated  
- self‑describing  

This prevents runtime surprises and enforces correctness.

### 3. Append‑Only  
Handlers may **add** information but may never:

- remove values  
- mutate existing values  
- override previously contributed values  

### 4. Declarative  
Contexts describe:

- what is known  
- what has been validated  
- what the callback pipeline has contributed  

They do not contain behavior.

---

## Current Implementations

### `ApplicationAuthCallbackContext`
Used during application‑level authentication callbacks.

It provides:

- immutable request metadata  
- immutable authentication state  
- append‑only contributions from callback handlers  

### `OidcAuthCallbackCallbackContext`
Used during OIDC authentication callback flows.

It provides:

- immutable OIDC parameters  
- immutable token information  
- append‑only contributions from OIDC handlers  

These two contexts define the **reference implementation** for all future
Immutable Context usage.

---

## Context Lifecycle (Current Pattern)

1. Engine creates the **initial context**  
2. Handler A receives context → returns new context  
3. Handler B receives new context → returns new context  
4. Engine validates final context  
5. Engine completes the callback pipeline  

This ensures:

- no mutation  
- no shared state  
- deterministic ordering  

---

## Composition Rules

Immutable Contexts may contain:

- request metadata  
- authentication state  
- validated parameters  
- handler contributions  
- capability flags  

They must not contain:

- business logic  
- mutable collections  
- runtime services  
- DI containers  
- product‑specific types  

Contexts are **data**, not behavior.

---

## Validation

Immutable Contexts must be validated:

- required fields present  
- required parameters bound  
- no missing contributions  
- no invalid state transitions  

Validation failures must:

- fail fast  
- provide actionable diagnostics  
- never allow partial callback execution  

---

## Extension Rules

Handlers may extend a context by:

- adding new typed values  
- adding validated data  
- adding metadata  

Handlers must not:

- remove values  
- mutate existing values  
- override other handlers’ contributions  

Extensions must be additive and safe.

---

## Test Harness Integration

Immutable Contexts enable:

- deterministic callback testing  
- context snapshot comparison  
- handler isolation  
- reproducible authentication flows  

This is essential for US‑176 (Test Harness Migration).

---

## Architectural Direction

Although Immutable Context is currently used only for authentication callback
contexts, it represents the **preferred pattern** for deterministic,
side‑effect‑free pipelines.

Future adoption requires explicit stories:

- **US‑216 — Refactor HostingEngine to use ImmutabilityContext**  
- **US‑217 — Refactor StartupEngine to use ImmutabilityContext**  

These stories define the expansion path but are **not yet implemented**.

---

## Enforcement

Immutable Context rules are enforced through:

- guardrail tests  
- handler validation  
- conventions governance  

Immutable Contexts must remain **pure**, **predictable**, and
**append‑only**.
