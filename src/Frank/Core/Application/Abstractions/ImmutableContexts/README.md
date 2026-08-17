# ImmutableContexts

The **ImmutableContexts** folder contains abstractions and supporting types used to construct deterministic, read‑only context objects from incoming requests. Immutable contexts provide a stable snapshot of validated, normalized, and enriched data that downstream components can rely on without mutation.

This subsystem ensures that context construction is predictable, composable, observable, and free of side effects.

---

## Purpose

Immutable contexts exist to:

- **normalize and validate input** before it reaches business logic  
- **produce deterministic, read‑only state** that cannot be mutated  
- **support composable build pipelines** through ordered build steps  
- **enable deep diagnostics** via before/after snapshots  
- **improve testability** by isolating context construction logic  
- **enforce immutability** across the entire context lifecycle  

This pattern is ideal for pipelines where correctness, reproducibility, and traceability matter.

---

## Components

### ImmutableContextBase
The root type for all immutable context objects.

```csharp
public abstract record ImmutableContextBase;
```

Represents the final, read‑only state produced by the builder.

---

### ImmutableContextBuilderRequestBase
The base type for all requests that initiate context building.

```csharp
public abstract record ImmutableContextBuilderRequestBase;
```

Requests contain raw input that must be validated and transformed.

---

### ImmutableContextBuilderResultBase
The base type for results returned by the builder.

```csharp
public abstract record ImmutableContextBuilderResultBase;
```

Results typically wrap the final context and any additional metadata.

---

### IImmutableContextBuilder
The main entry point for constructing immutable contexts.

```csharp
public interface IImmutableContextBuilder<TRequest, TContext, TResult>
{
    Task<TResult> BuildAsync(TRequest request, CancellationToken ct);
}
```

Builders orchestrate validation, step execution, diagnostics, and final result creation.

---

### IImmutableContextBuildStep
Represents a single transformation step in the build pipeline.

```csharp
public interface IImmutableContextBuildStep<TContext>
{
    IImmutableContextBuildStepMetadata Metadata { get; }
    bool CanExecute(TContext context);
    Task<TContext> ExecuteAsync(TContext context, CancellationToken ct);
}
```

Each step is responsible for producing a new immutable context instance.

---

### IImmutableContextBuildStepMetadata
Describes a build step.

```csharp
public interface IImmutableContextBuildStepMetadata
{
    string Id { get; }
    string DisplayName { get; }
}
```

Used for ordering, diagnostics, and introspection.

---

### ImmutableContextBuildStepMetadata
Concrete metadata implementation.

```csharp
public sealed class ImmutableContextBuildStepMetadata : IImmutableContextBuildStepMetadata
{
    public string Id { get; }
    public string DisplayName { get; }
}
```

---

### ImmutableContextBuilderDiagnosticEvent
Captures before/after snapshots and timing information for each step.

```csharp
public sealed record ImmutableContextBuilderDiagnosticEvent(
    string StepId,
    string StepName,
    string Phase,
    long? DurationMs,
    ImmutableContextBase Before,
    ImmutableContextBase After);
```

Provides deep visibility into the pipeline’s behavior.

---

## Design Principles

- **Immutability**  
  Every transformation produces a new context instance.

- **Determinism**  
  Given the same request, the pipeline produces the same result.

- **Composability**  
  Build steps are modular, ordered, and independently testable.

- **Observability**  
  Diagnostic events capture step‑level transformations and timing.

- **Separation of concerns**  
  Context building is isolated from business logic and transport layers.

- **Extensibility**  
  New steps can be added without modifying existing ones.

---

## How Immutable Contexts Fit Into the Application

Immutable contexts are typically used in:

- command and query dispatch pipelines  
- validation and normalization layers  
- domain orchestration  
- request preprocessing  
- feature modules that require deterministic input state  

They ensure that downstream components receive a stable, validated, and enriched snapshot of the request, free from mutation and side effects.

---
