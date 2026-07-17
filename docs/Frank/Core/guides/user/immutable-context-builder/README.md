# Frank ImmutableContextBuilder — User Guide

The `ImmutableContextBuilderBase<TContext, TStep>` capability is how Frank runs a **multi‑step, rule‑driven pipeline** that transforms an immutable context through a sequence of conditional build steps.

As a user of this capability, you do not write the pipeline engine itself — you **use** it by:

- defining build steps  
- defining the immutable context  
- defining the request/result types  
- invoking a concrete builder that orchestrates the pipeline  

This guide explains how to use the capability correctly and safely.

---

# 1. What This Capability Does

`ImmutableContextBuilderBase`:

- takes an initial immutable context  
- evaluates which steps can run  
- executes each eligible step once  
- validates each transition  
- **emits diagnostic events for observability**  
- returns the final immutable context  

It guarantees:

- **deterministic execution**  
- **strict immutability**  
- **safe, validated transitions**  
- **traceable step execution** via diagnostics  

Use this when you need a **predictable, multi‑stage processing pipeline**.

---

# 2. How You Use It

You interact with this capability by providing:

---

## 2.1 A context type

````csharp
public sealed record MyContext : ImmutableContextBase;
````

---

## 2.2 One or more build steps

Each step decides:

- whether it can run (`CanExecute`)
- how it transforms the context (`ExecuteAsync`)
- how it identifies itself (`Metadata`)

Example:

````csharp
public sealed class ValidateRequestStep : IImmutableContextBuildStep<MyContext>
{
    public IImmutableContextBuildStepMetadata Metadata { get; }
        = new ImmutableContextBuildStepMetadata("validate", "Validate Request");

    public bool CanExecute(MyContext ctx) => true;

    public Task<MyContext> ExecuteAsync(MyContext ctx, CancellationToken ct)
    {
        // return a NEW context instance
        return Task.FromResult(ctx with { /* updated fields */ });
    }
}
````

---

## 2.3 A concrete builder

You typically do not inherit from `ImmutableContextBuilderBase` yourself —  
you use a builder that *already* inherits from it.

Example pattern:

````csharp
var result = await myBuilder.BuildAsync(request, ct);
````

---

# 3. What Happens When You Call BuildAsync

When you call:

````csharp
await builder.BuildAsync(request, ct);
````

The builder:

1. Creates the initial context  
2. Runs `ProcessAsync`  
3. Selects the next step whose `CanExecute` returns true  
4. **Emits a Start diagnostic event**  
5. Executes the step  
6. **Emits an End diagnostic event with duration**  
7. Validates the transition  
8. Removes the step from the remaining set  
9. Repeats until no steps can execute  
10. Wraps the final context into a result object  

You receive a **result** that contains the final context.

---

# 4. What You Should Expect as a User

### 4.1 Steps run only when eligible  
If `CanExecute` returns false, the step is skipped.

### 4.2 Steps run at most once  
No step ever runs twice.

### 4.3 The pipeline stops naturally  
Execution ends when no remaining step can execute.

### 4.4 Context is always immutable  
Every step returns a *new* context instance.

### 4.5 Diagnostics are emitted  
You can subscribe to diagnostic events to trace:

- which steps ran  
- how long they took  
- what the before/after contexts were  
- the exact transition path  

This is the core of **Frank’s observability model**.

---

# 5. What You Should Not Do

Users should **never**:

- mutate the context directly  
- reuse a builder instance across multiple builds  
- rely on step execution order beyond the defined list  
- assume all steps will run  
- assume steps run unconditionally  
- bypass the builder and run steps manually  

The builder is the **only** supported way to run the pipeline.

---

# 6. When to Use This Capability

Use `ImmutableContextBuilderBase` when you need:

- a multi‑stage processing pipeline  
- conditional execution of steps  
- strict immutability  
- deterministic behavior  
- **step‑level observability**  
- invariant enforcement  

Common examples:

- authentication/session pipelines  
- request validation pipelines  
- multi‑phase processing flows  
- state‑transition workflows  

---

# 7. Benefits to You as a User

You get:

- **predictability** — same inputs → same outputs  
- **safety** — invalid transitions fail early  
- **clarity** — each step is isolated and self‑describing  
- **observability** — diagnostics show exactly what happened  
- **immutability** — no accidental state mutation  
- **composability** — steps can be added or removed cleanly  

---

# 8. Summary

As a user:

- You define steps and context types  
- You call a concrete builder  
- The builder executes steps deterministically  
- Each step transforms the context immutably  
- **Diagnostics give you full visibility**  
- Transition validation keeps pipelines safe  

`ImmutableContextBuilderBase` is the foundation for all multi‑step, rule‑driven pipelines in Frank.
