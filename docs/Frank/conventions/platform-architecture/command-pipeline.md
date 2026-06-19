# command-pipeline.md

# Command Pipeline Architecture (Frank)

The **Command Pipeline** is the deterministic, side‑effect‑controlled execution
pipeline responsible for handling all write operations in Frank‑based systems.

Commands represent **intent to change state**.  
The Command Pipeline ensures that this intent is executed:

- safely  
- predictably  
- consistently  
- with enforced cross‑cutting rules  

The pipeline is platform‑level — not product‑level — and applies uniformly across
all slices and all products.

---

## Purpose

The Command Pipeline exists to:

- enforce purity and separation of concerns  
- guarantee deterministic handler execution  
- apply cross‑cutting behaviors (validation, logging, domain events)  
- unify error semantics  
- ensure commands cannot bypass platform rules  
- provide a consistent developer experience across slices  

Commands are the **only** mechanism for mutating state.

---

## Command Pipeline Flow

The pipeline executes in the following deterministic order:

1. **Command Creation**  
   A command is created as a pure data object with no behavior.

2. **Validation Stage**  
   Validators run before the handler.  
   They must not mutate state or perform I/O.

3. **Handler Resolution**  
   Exactly one handler must exist for each command.

4. **Handler Execution**  
   The handler performs the state‑changing operation.

5. **Domain Event Collection**  
   Any domain events raised during handler execution are collected.

6. **Post‑Processing Behaviors**  
   - domain event dispatch  
   - logging  
   - auditing  
   - outbox writing (if applicable)

7. **Response Shaping**  
   The pipeline returns a strongly‑typed response or a shaped error.

This flow is **immutable**, **ordered**, and **enforced**.

---

## Command Rules

### 1. Commands Are Pure Data
Commands must:

- be immutable  
- contain no behavior  
- contain no domain logic  
- contain no infrastructure logic  
- contain only the data required to perform the operation  

Commands must not:

- validate themselves  
- perform authorization  
- perform I/O  
- contain business rules  

Commands are **intent**, not behavior.

---

### 2. Command Handlers Are the Only Place Where State Changes
Handlers must:

- perform the state‑changing operation  
- raise domain events  
- return a typed response  

Handlers must not:

- perform cross‑cutting concerns directly  
- perform logging  
- perform validation  
- dispatch domain events  
- shape errors  
- access HTTP context  
- depend on product‑specific infrastructure  

Handlers are **pure application logic**.

---

### 3. Command Validators Enforce Input Correctness
Validators must:

- validate command shape  
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

The Command Pipeline applies cross‑cutting behaviors in a strict order:

1. **Validation Behavior**  
2. **Authorization Behavior** (if applicable)  
3. **Handler Execution Behavior**  
4. **Domain Event Dispatch Behavior**  
5. **Outbox Behavior**  
6. **Logging Behavior**  
7. **Error Shaping Behavior**

Behaviors must:

- be deterministic  
- be ordered  
- be pure (except handler execution)  
- never bypass the pipeline  

Behaviors must not:

- mutate shared state  
- depend on product code  
- break purity rules  

---

## Domain Events

Handlers may raise domain events.  
The pipeline is responsible for:

- collecting them  
- dispatching them  
- ensuring ordering  
- ensuring outbox consistency  

Handlers must not dispatch events directly.

---

## Error Architecture (Command‑Specific)

Commands may fail with:

- validation errors  
- authorization errors  
- domain rule violations  
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

Command responses must be:

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

The Command Pipeline must be fully testable:

- handler tests  
- validator tests  
- pipeline tests  
- domain event tests  
- error shaping tests  

The test harness (US‑176) must support:

- fake repositories  
- fake clocks  
- fake outbox  
- fake domain event dispatcher  

---

## Prohibitions

Commands must not:

- perform queries  
- perform I/O  
- mutate global state  
- bypass the pipeline  
- depend on HTTP context  
- depend on product‑specific infrastructure  

Handlers must not:

- perform cross‑cutting concerns  
- dispatch domain events  
- shape errors  
- log directly  

Validators must not:

- perform I/O  
- perform authorization  

---

## Enforcement

The Command Pipeline architecture is enforced through:

- guardrail tests  
- handler resolution rules  
- validator discovery rules  
- behavior ordering rules  
- conventions governance  

The pipeline must remain:

- **pure**  
- **deterministic**  
- **platform‑level**  
- **product‑agnostic**  
