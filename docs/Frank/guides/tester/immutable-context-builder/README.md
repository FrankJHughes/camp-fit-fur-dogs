# Frank ImmutableContextBuilder — Tester Guide

This guide explains how to test the `ImmutableContextBuilderBase<TContext, TStep>` capability.  
This capability is a **deterministic, rule‑driven, multi‑step pipeline executor** that transforms an immutable context through a sequence of conditional build steps.

Testers are responsible for validating determinism, immutability, step selection logic, transition invariants, and diagnostic correctness.

---

# 1. What Testers Validate

Testers must ensure:

- **Deterministic execution**  
  Same initial context + same steps → same final context.

- **Correct step selection**  
  Steps execute only when `CanExecute` returns true.

- **Single execution per step**  
  No step may run more than once.

- **Valid transitions**  
  `AssertValidTransition` enforces invariants and rejects illegal state changes.

- **Immutability**  
  Steps must return a *new* `TContext` instance; previous contexts must remain unchanged.

- **No side effects**  
  Steps must not depend on environment variables, system time, randomness, or global state.

- **Correct diagnostics**  
  `EmitStartEvent` and `EmitEndEvent` must fire with correct metadata and durations.

---

# 2. Required Test Types

## 2.1 Determinism Tests

Validate that:

- Running the same pipeline twice yields identical `TContext` results.
- Step ordering is stable.
- No nondeterministic values appear unless explicitly injected.

**Patterns:**

- Build twice → deep equality.  
- Snapshot test the final context.  
- Ensure no timestamps, GUIDs, or environment values appear unless provided.

---

## 2.2 Step Selection Tests

Validate:

- Steps execute only when `CanExecute(ctx)` is true.  
- Steps that cannot execute are skipped.  
- Execution stops when no remaining step can execute.  
- Steps execute in the order they appear in the `_steps` list.

**Patterns:**

- Provide contexts that enable/disable specific steps.  
- Assert that only the expected steps run.  
- Assert that no step runs twice.

---

## 2.3 Transition Validation Tests

`AssertValidTransition` must enforce invariants.

Testers must ensure:

- Illegal transitions fail.  
- Required fields are present after certain steps.  
- Immutable fields remain unchanged.  
- Steps cannot regress state.

**Patterns:**

- Provide a step that attempts an illegal change → expect exception.  
- Provide a step that omits required fields → expect exception.

---

## 2.4 Immutability Tests

Validate:

- `before` and `after` contexts are different instances.  
- `before` remains unchanged after step execution.  
- Steps do not mutate shared objects.

**Patterns:**

- Capture `before` → run step → assert `before` unchanged.  
- Assert `ReferenceEquals(before, after)` is false.

---

## 2.5 Diagnostic Tests

Validate:

- `EmitStartEvent` fires before execution.  
- `EmitEndEvent` fires after execution.  
- Duration is measured correctly.  
- Metadata (StepId, StepName, Phase) is correct.  
- Before/After snapshots match the actual contexts.

**Patterns:**

- Capture emitted events → assert ordering and correctness.  
- Assert duration is non‑negative and reasonable.

---

## 2.6 Error & Guardrail Tests

Testers must ensure:

- Steps throwing exceptions propagate correctly.  
- Invalid transitions fail loudly.  
- Null contexts or null steps are rejected.  
- Builders do not allow re‑entrancy or reuse during execution.

**Patterns:**

- Step throws → pipeline throws.  
- Step returns null → expect failure.  
- Reuse builder concurrently → expect failure.

---

# 3. Test Isolation Requirements

Testers must ensure:

- No test shares a builder instance.  
- No test shares a context instance unless explicitly intended.  
- No test relies on environment variables or system time.  
- No test uses static/global state.

This ensures tests validate **pure, deterministic behavior**.

---

# 4. Recommended Testing Patterns

## 4.1 Deep Equality Testing
Validate that two runs produce identical contexts.

## 4.2 Snapshot Testing
Validate the shape and content of the final context.

## 4.3 Step Execution Tracing
Capture emitted diagnostic events and assert:

- correct ordering  
- correct metadata  
- correct before/after references  

## 4.4 Negative Testing
Ensure invalid transitions and illegal states fail loudly.

---

# 5. Anti‑Patterns (Tests Must Reject)

Tests must fail if they detect:

- Mutation of `TContext`.  
- Steps running more than once.  
- Steps executing when `CanExecute` is false.  
- Nondeterministic behavior.  
- Hidden side effects.  
- Global/static state usage.  
- Missing or incorrect diagnostic events.  
- Weak or missing transition validation.

---

# 6. Summary

Testers ensure that `ImmutableContextBuilderBase`:

- executes steps deterministically  
- enforces correct step selection  
- validates transitions rigorously  
- preserves immutability  
- emits accurate diagnostics  
- rejects invalid or nondeterministic behavior  

This guarantees that all Frank pipelines built on this capability behave safely, predictably, and transparently.
