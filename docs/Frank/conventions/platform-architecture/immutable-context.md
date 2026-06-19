# immutable-context.md

# Immutable Context (Frank)

The **Immutable Context** is the foundational data‑exchange mechanism used by all
Frank engines — Hosting, Startup, and Application.  
It ensures deterministic behavior, eliminates side effects, and enforces strict
separation of concerns across modules.

A context is:

- immutable  
- strongly typed  
- validated  
- engine‑specific  
- module‑safe  

Modules **never mutate** a context.  
They always return a **new** context.

---

## Purpose

Immutable contexts exist to:

- guarantee deterministic module execution  
- prevent hidden state mutation  
- enforce purity across modules  
- support safe parallelization  
- enable deep validation  
- improve testability  
- ensure predictable startup and hosting behavior  

Contexts are the **contract** between modules and engines.

---

## Context Characteristics

### 1. Immutable  
Once created, a context cannot be modified.  
Modules must return a **new** context instance when adding or transforming data.

### 2. Strongly Typed  
Contexts are not dictionaries or bags of values.  
They are:

- typed  
- validated  
- self‑describing  

This prevents runtime surprises.

### 3. Engine‑Scoped  
Each engine defines its own context type:

- `HostingContext`  
- `StartupContext`  
- `ApplicationContext`  

Contexts must not leak across engine boundaries.

### 4. Declarative  
Contexts describe:

- what is known  
- what is configured  
- what modules have contributed  
- what remains required  

They do not contain behavior.

---

## Context Lifecycle

Contexts flow through the engine pipeline:

1. Engine creates the **initial context**  
2. Module A receives context → returns new context  
3. Module B receives new context → returns new context  
4. Engine validates final context  
5. Engine emits lifecycle events  
6. Engine hands off context to next layer (if applicable)

This ensures:

- no mutation  
- no shared state  
- no side effects  
- deterministic ordering  

---

## Context Composition

Contexts may contain:

- configuration values  
- environment information  
- module contributions  
- validated settings  
- resolved secrets  
- engine metadata  
- dependency graphs  
- capability flags  

Contexts must not contain:

- business logic  
- product‑specific types  
- mutable collections  
- runtime services  
- open generics  
- DI containers  

Contexts are **data**, not behavior.

---

## Context Validation

Each engine validates its context:

- required fields present  
- required configuration bound  
- required services registered  
- module dependencies satisfied  
- no circular dependencies  
- no forbidden capabilities  
- no missing contributions  

Validation failures must:

- fail fast  
- provide actionable diagnostics  
- never allow partial startup  

---

## Context Extension

Modules may extend a context by:

- adding new typed values  
- adding capability flags  
- adding validated configuration  
- adding module metadata  

Modules must not:

- remove values  
- mutate existing values  
- override other modules’ contributions  
- introduce product‑specific types  

Extensions must be additive and safe.

---

## Context and Test Harness

Immutable contexts enable:

- deterministic test startup  
- module‑level testing  
- context snapshot comparison  
- dependency graph inspection  
- configuration injection  
- module isolation  

This is essential for US‑176 (Test Harness Migration).

---

## Prohibitions

Contexts must not:

- contain mutable state  
- expose setters  
- reference product code  
- depend on slices  
- contain business logic  
- perform I/O  
- read environment variables directly  
- bypass engine validation  

Contexts must remain **pure**, **predictable**, and **platform‑level**.

---

## Enforcement

Immutable context rules are enforced through:

- guardrail tests  
- module validation  
- dependency analysis  
- conventions governance  

Immutable contexts are the **backbone** of deterministic module execution.
