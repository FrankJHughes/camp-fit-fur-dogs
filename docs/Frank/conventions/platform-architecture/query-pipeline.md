# query-pipeline.md

# Query Pipeline Architecture (Frank)

The **Query Pipeline** is the deterministic, read‑only execution pipeline
responsible for handling all **non‑mutating** operations in Frank‑based systems.

Queries represent **intent to read state**.  
The Query Pipeline ensures that this intent is executed:

- safely  
- predictably  
- without side effects  
- with enforced cross‑cutting rules  

The pipeline is platform‑level — not product‑level — and applies uniformly across
all slices and all products.

---

## Purpose

The Query Pipeline exists to:

- separate reads from writes (CQRS boundary)  
- guarantee that queries do not mutate state  
- provide a consistent model for data retrieval  
- apply cross‑cutting behaviors (validation, caching, logging)  
- unify error semantics for read operations  
- ensure queries cannot bypass platform rules  

Queries are the **only** mechanism for reading state through the application
layer.

---

## Query Pipeline Flow

The pipeline executes in the following deterministic order:

1. **Query Creation**  
   A query is created as a pure data object with no behavior.

2. **Validation Stage**  
   Validators run before the handler.  
   They must not mutate state or perform I/O.

3. **Handler Resolution**  
   Exactly one handler must exist for each query.

4. **Caching Stage (Optional)**  
   If enabled, the pipeline may resolve the response from cache.

5. **Handler Execution**  
   The handler performs the read operation.

6. **Post‑Processing Behaviors**  
   - caching (store)  
   - logging  
   - metrics / tracing  

7. **Response Shaping**  
   The pipeline returns a strongly‑typed response or a shaped error.

This flow is **immutable**, **ordered**, and **enforced**.

---

## Query Rules

### 1. Queries Are Pure Data
Queries must:

- be immutable  
- contain no behavior  
- contain no domain logic  
- contain no infrastructure logic  
- contain only the data required to perform the read  

Queries must not:

- validate themselves  
- perform authorization  
- perform I/O  
- contain business rules  

Queries are **intent**, not behavior.

---

### 2. Query Handlers Are Read‑Only

Handlers must:

- perform the read operation  
- map data to DTOs  
- return a typed response  

Handlers must not:

- mutate state  
- publish domain events  
- write to the database  
- enqueue messages  
- modify caches directly (pipeline handles this)  
- access HTTP context  
- depend on product‑specific infrastructure  

Handlers are **read‑only application logic**.

---

### 3. Query Validators Enforce Input Correctness

Validators must:

- validate query shape  
- validate invariants that do not require I/O  
- return structured validation errors  

Validators must not:

- perform database lookups  
- perform HTTP calls  
- perform authorization  
- mutate state  

Validators are **pure, synchronous, and deterministic**.

---

## Cross‑Cutting Behaviors

The Query Pipeline applies cross‑cutting behaviors in a strict order:

1. **Validation Behavior**  
2. **Authorization Behavior** (if applicable)  
3. **Caching Behavior (Read)**  
4. **Handler Execution Behavior**  
5. **Caching Behavior (Write‑Back)**  
6. **Logging Behavior**  
7. **Error Shaping Behavior**

Behaviors must:

- be deterministic  
- be ordered  
- be pure (except handler execution, which is read‑only)  
- never bypass the pipeline  

Behaviors must not:

- mutate shared state (other than cache where explicitly allowed)  
- depend on product code  
- break purity rules  

---

## Caching Semantics

Caching is a **pipeline concern**, not a handler concern.

The pipeline may:

- resolve responses from cache  
- store responses in cache  
- invalidate or update cache entries based on configuration  

Handlers must not:

- talk to cache directly  
- embed caching logic  
- depend on cache lifetime  

Caching must be:

- explicit  
- configurable  
- observable  

---

## Error Architecture (Query‑Specific)

Queries may fail with:

- validation errors  
- authorization errors  
- not‑found conditions  
- infrastructure failures  

The pipeline must:

- shape errors into a consistent format  
- never leak internal exceptions  
- never expose stack traces  
- never expose infrastructure details  

Errors must be:

- structured  
- typed  
- predictable  

---

## Response Semantics

Query responses must be:

- strongly typed  
- deterministic  
- free of infrastructure details  
- free of domain entities (DTOs only)  

Handlers must not return:

- domain entities  
- EF entities  
- infrastructure types  
- raw exceptions  

---

## Testability

The Query Pipeline must be fully testable:

- handler tests  
- validator tests  
- pipeline tests  
- caching behavior tests  
- error shaping tests  

The test harness (US‑176) must support:

- fake repositories  
- fake caches  
- fake clocks  

---

## Prohibitions

Queries must not:

- mutate state  
- perform commands  
- bypass the pipeline  
- depend on HTTP context  
- depend on product‑specific infrastructure  

Handlers must not:

- write to the database  
- publish domain events  
- enqueue messages  
- perform cross‑cutting concerns directly  

Validators must not:

- perform I/O  
- perform authorization  

---

## Enforcement

The Query Pipeline architecture is enforced through:

- guardrail tests  
- handler resolution rules  
- validator discovery rules  
- behavior ordering rules  
- conventions governance  

The pipeline must remain:

- **read‑only**  
- **deterministic**  
- **platform‑level**  
- **product‑agnostic**  
