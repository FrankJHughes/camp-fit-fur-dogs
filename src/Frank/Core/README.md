# Frank.Core — Foundational Library

**Frank.Core** is the foundational **library** of the Frank architecture.  
It provides the domain‑independent building blocks that every vertical slice,
application service, infrastructure adapter, and API endpoint relies on.

Unlike the Api, Platform, Application, or Infrastructure layers, **Frank.Core is
not a layer** — it is a **pure library** containing primitives, abstractions,
contracts, and utilities that shape the entire system.

Frank.Core is intentionally stable, minimal, dependency‑free, and reusable.

---

## Purpose of Frank.Core

Frank.Core provides:

- **Shared primitives** used across all layers  
- **Cross‑cutting abstractions** (clock, ID generation, outbox contracts, etc.)  
- **Domain‑agnostic utilities**  
- **Architectural conventions** (vertical slice patterns, purity rules)  
- **Common interfaces** for infrastructure and application layers  
- **Foundational building blocks** for middleware and platform subsystems  

It is the “kernel” of the system — everything depends on it, but it depends on
nothing.

---

## Folder Structure

A typical Core folder looks like:

```
Core/
├── Application/
│   ├── Commands/
│   ├── Queries/
│   ├── Behaviors/
│   └── Abstractions/
├── Infrastructure/
│   ├── Clock/
│   ├── Email/
│   ├── Outbox/
│   └── Persistence/
└── Common/
    ├── Errors/
    ├── Results/
    ├── Utilities/
    └── Contracts/
```

(Your exact structure may vary depending on modules.)

---

## Key Responsibilities

### 1. Application Primitives

Frank.Core defines the primitives used by the Application layer:

- Command and Query interfaces  
- Handler contracts  
- Pipeline behaviors  
- Validation abstractions  
- Result types (`Success`, `Failure`, `NotFound`, etc.)  
- Error modeling  

These primitives enforce consistency across all vertical slices.

---

### 2. Infrastructure Abstractions

Frank.Core defines interfaces that infrastructure must implement:

- Clock abstraction  
- ID generator  
- Outbox message contract  
- Email sender contract  
- Persistence abstractions  
- External service interfaces  

This ensures infrastructure is **pluggable**, **testable**, and **replaceable**.

---

### 3. Common Utilities

Frank.Core provides shared utilities:

- Functional helpers  
- Mapping helpers  
- Serialization helpers  
- Lightweight value objects  
- Guard clauses  
- Domain‑agnostic helpers  

These utilities reduce duplication and enforce architectural consistency.

---

### 4. Cross‑Cutting Contracts

Frank.Core defines contracts used across Api, Platform, Middleware, Application,
and Infrastructure:

- Observation context contracts  
- Error contracts  
- Result envelopes  
- Pipeline purity rules  
- Architectural constraints  

These contracts ensure that all layers communicate consistently.

---

## How Frank.Core Fits Into the Architecture

Frank.Core sits at the foundation of the architecture:

```
[ API ]
   ↓
[ Platform ]
   ↓
[ Middleware ]
   ↓
[ Application ]
   ↓
[ Infrastructure ]
   ↓
[ Frank.Core (Library) ]
```

Everything depends on Frank.Core.  
Frank.Core depends on nothing.

This ensures:

- Maximum stability  
- Maximum testability  
- Maximum reusability  
- Maximum architectural clarity  

---

## Design Principles

- **Dependency‑free**  
  Frank.Core must not depend on Application, Infrastructure, Platform, or Api.

- **Pure abstractions**  
  No business logic, no environment logic, no infrastructure logic.

- **Stable contracts**  
  Core types change rarely and carefully.

- **Cross‑layer consistency**  
  All layers rely on the same primitives and conventions.

- **Testability**  
  Core abstractions make every layer easier to test.

---

## Notes

- Frank.Core is the most stable library — changes ripple through the entire system.  
- Keep it small, focused, and intentional.  
- Avoid domain‑specific logic; keep Frank.Core domain‑agnostic.  
- All vertical slices should rely on Core primitives for consistency.

---
