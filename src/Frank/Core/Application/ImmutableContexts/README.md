# ImmutableContexts

The **ImmutableContexts** folder contains the application‑layer infrastructure
for constructing immutable context objects through a declarative, step‑driven
pipeline. This subsystem provides the execution engine that evaluates build
steps, enforces immutability, validates transitions, and emits observability
events for each transformation.

This folder contains the core base class used by slices to implement their own
immutable context builders.

---

## Components

### ImmutableContextBuilderBase\<TContext, TStep\>

The `ImmutableContextBuilderBase` class provides the orchestration logic for
building immutable context instances. It executes a sequence of build steps,
each of which:

- inspects the current context  
- determines whether it can execute (`CanExecute`)  
- produces a new immutable context (`ExecuteAsync`)  

The builder ensures that each step produces a valid transition and that the
pipeline terminates when no further steps can execute.

#### Responsibilities

1. **Step selection**  
   Steps are selected dynamically based on whether they can execute given the
   current context. This allows flexible, dependency‑aware pipelines without
   explicit ordering.

2. **Step execution**  
   Each step is invoked asynchronously and must return a new immutable context
   instance.

3. **Transition validation**  
   The abstract `AssertValidTransition` method allows derived builders to enforce
   invariants between the “before” and “after” contexts.

4. **Observability**  
   The builder emits structured events at the start and end of each step,
   including:
   - step metadata  
   - context types  
   - execution duration  

   These events integrate with the application’s observation sink and system
   context.

---

## Design Principles

- **Immutability**  
  Every step produces a new context instance; no in‑place mutation is allowed.

- **Declarative execution**  
  Steps declare when they can execute; the builder determines the order.

- **Validation-first**  
  Each transition is checked for correctness via `AssertValidTransition`.

- **Observability**  
  Execution is fully instrumented with start/end events.

- **Extensibility**  
  Derived builders can customize transition rules and add additional behavior.

---

## How This Folder Fits Into the Application

Immutable contexts are used to model state that evolves through a well-defined
pipeline of transformations. This subsystem provides the mechanics for:

- domain orchestration  
- request/operation context construction  
- multi-step evaluation pipelines  
- deterministic, traceable state evolution  

Slices define the concrete context types and steps; this folder provides the
execution engine.

---

## Typical Usage

```csharp
var builder = new MyContextBuilder(steps, sink, systemContext);
var finalContext = await builder.BuildAsync(initialContext, ct);
```

Each step transforms the context until no further steps can execute.

---

## Notes

- Steps must implement `IImmutableContextBuildStep<TContext>`.  
- Builders must derive from `ImmutableContextBuilderBase<TContext, TStep>`.  
- Observability hooks can be overridden for custom telemetry.  
- This folder contains **only** the base builder — not concrete contexts or
  steps.

---
