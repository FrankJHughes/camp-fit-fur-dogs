# Frank ImmutableContextBuilderBase — Developer Guide

The `ImmutableContextBuilderBase<TContext, TStep>` capability is a **deterministic, rule‑driven, multi‑step pipeline executor** that transforms an immutable context through a sequence of well‑defined build steps.

It is the core mechanism used in Frank’s authentication, session, and multi‑stage processing pipelines.

This guide describes how the capability works, how to extend it, and the invariants developers must preserve.

---

# 1. Purpose

`ImmutableContextBuilderBase` exists to:

- execute a **series of ordered, conditional build steps**
- transform an immutable context through **pure, append‑only transitions**
- enforce **transition invariants** between steps
- emit **diagnostic events** for observability
- guarantee **deterministic pipeline execution**
- prevent mutation of immutable context fields

It is the backbone of all Frank pipelines that require:

- multi‑stage processing  
- conditional execution  
- strict invariants  
- deterministic behavior  
- traceable transitions  

---

# 2. Architectural Overview

## 2.1 Components

### **TContext : ImmutableContextBase**
A record representing the immutable state flowing through the pipeline.

### **TStep : IImmutableContextBuildStep<TContext>**
A pipeline step with:

- metadata (`Id`, `DisplayName`)
- a `CanExecute` predicate
- an asynchronous `ExecuteAsync` method

### **ImmutableContextBuilderBase**
The orchestrator that:

- selects the next executable step  
- executes it  
- validates the transition  
- emits start/end events  
- produces the final context  

---

# 3. Execution Model

Pipeline execution is performed by:

```csharp
await ProcessAsync(TContext ctx, CancellationToken ct)
```

### 3.1 Step Selection

```csharp
TrySelectNextStep(remaining, ctx, out var step)
```

Rules:

- Steps are executed **only if `CanExecute(ctx)` returns true**
- Steps are executed **at most once**
- Order is determined by the order of the `_steps` list
- Execution stops when no remaining step can execute

### 3.2 Step Execution

```csharp
var after = await step.ExecuteAsync(before, ct);
```

Each step:

- receives the **current immutable context**
- returns a **new immutable context**
- must not mutate the previous context

### 3.3 Transition Validation

After each step:

```csharp
AssertValidTransition(step, before, after);
```

This enforces invariants such as:

- immutable fields must not change  
- required fields must be populated  
- forbidden transitions must be rejected  

### 3.4 Diagnostics

Two events are emitted:

```csharp
EmitStartEvent(step, before);
EmitEndEvent(step, before, after, durationMs);
```

These produce:

- step ID  
- step name  
- phase (Start/End)  
- duration  
- before/after snapshots  

---

# 4. Invariants

Developers must enforce:

### **4.1 Immutability**
`TContext` is a record; steps must return a *new* instance.

### **4.2 Determinism**
Given:

- the same initial context  
- the same step list  
- the same `CanExecute` logic  

the pipeline must produce the same final context.

### **4.3 No Side Effects**
Steps must not:

- mutate global state  
- read environment variables  
- read system time  
- perform I/O  
- depend on randomness  

Unless explicitly injected.

### **4.4 Valid Transitions**
`AssertValidTransition` must enforce:

- immutable fields unchanged  
- required fields present  
- no illegal state regressions  

---

# 5. Extension Points

## 5.1 Implementing a Build Step

A step must implement:

```csharp
bool CanExecute(TContext context);
Task<TContext> ExecuteAsync(TContext context, CancellationToken ct);
IImmutableContextBuildStepMetadata Metadata { get; }
```

Rules:

- `CanExecute` must be pure  
- `ExecuteAsync` must return a new context  
- Metadata must uniquely identify the step  

## 5.2 Implementing a Builder

A builder must:

- inherit `ImmutableContextBuilderBase<TContext, TStep>`
- implement:
  - `AssertValidTransition`
  - `EmitStartEvent`
  - `EmitEndEvent`

Builders typically:

- initialize the starting context  
- call `ProcessAsync`  
- wrap the final context into a result object  

---

# 6. Diagnostics

Diagnostics are emitted via:

```csharp
ImmutableContextBuilderDiagnosticEvent
```

Fields:

- StepId  
- StepName  
- Phase  
- DurationMs  
- Before  
- After  

These events allow:

- tracing  
- debugging  
- observability  
- performance measurement  

---

# 7. Testing Requirements

Tests must validate:

### **7.1 Deterministic Execution**
Same inputs → same outputs.

### **7.2 Step Ordering**
Steps execute in list order, but only when `CanExecute` is true.

### **7.3 Transition Validity**
Invalid transitions must fail.

### **7.4 Immutability**
Steps must not mutate the previous context.

### **7.5 Diagnostics**
Start/End events must be emitted with correct metadata.

### **7.6 Step Execution Rules**
- Steps execute once  
- Steps that cannot execute are skipped  
- Execution stops when no remaining step can execute  

---

# 8. Anti‑Patterns

Never:

- mutate the context  
- allow steps to run twice  
- allow nondeterministic behavior  
- hide side effects  
- weaken transition validation  
- bypass `AssertValidTransition`  
- swallow exceptions  
- use global/static state  

---

# 9. Summary

`ImmutableContextBuilderBase` is a **deterministic, invariant‑checked, multi‑step pipeline engine**.

It ensures:

- pure transformations  
- strict immutability  
- deterministic execution  
- traceable transitions  
- safe, predictable pipelines  

It is the foundation for all multi‑stage processing in Frank.
