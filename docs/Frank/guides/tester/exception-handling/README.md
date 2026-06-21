# Frank ExceptionHandling — Tester Guide

This guide describes how to test the full ExceptionHandling capability in Frank, including:

- **Core Abstractions** (`IExceptionHandler`, `IErrorCode`, `ProblemDetails`)
- **API Runtime** (`ExceptionHandlingRegistry`, `ExceptionHandlingOptions`)
- **Capability Governance** (activation/opt‑out behavior)

Testers validate correctness, determinism, safety, and compliance with invariants across all layers.

---

# 1. What Testers Validate

Testers must ensure:

- **Correct handler resolution**  
  Registry selects the correct handler based on `CanHandle`.

- **Correct ordering**  
  Handlers are sorted by `ExceptionHandlerAttribute.Order`.

- **Deterministic behavior**  
  Same exception → same handler → same `ProblemDetails`.

- **Correct error code mapping**  
  `GetErrorCode` returns stable, domain‑meaningful codes.

- **Correct ProblemDetails shaping**  
  Output is safe, structured, and consistent.

- **Correct option behavior**  
  - `IncludeExceptionDetails` respected  
  - `IncludeErrorCode` respected  
  - `LogUnhandledExceptions` respected  

- **Capability governance**  
  - Capability can be disabled  
  - Handlers can be disabled  
  - Fallback handler always exists  

- **No side effects**  
  Handlers must be pure and safe.

---

# 2. Required Test Types

## 2.1 Handler Ordering Tests

Validate:

- Handlers are sorted by `Order` ascending.
- Missing attribute defaults to `Order = 1000`.
- Specific handlers run before generic ones.

**Patterns:**

- Register handlers with known orders → assert registry order.
- Register handlers without attributes → assert they appear last.

---

## 2.2 Handler Resolution Tests

Validate:

- The first handler whose `CanHandle` returns true is selected.
- No other handlers are evaluated after the match.
- Multiple matching handlers → lowest‑order handler wins.

**Patterns:**

- Provide a custom exception → assert correct handler.
- Provide an exception handled by multiple handlers → assert ordering.

---

## 2.3 Fallback Handler Tests

Validate:

- A catch‑all handler exists.
- Registry never throws due to missing handlers.

**Patterns:**

- Throw an unknown exception → assert fallback handler is selected.

---

## 2.4 Error Code Mapping Tests

Validate:

- `GetErrorCode` returns a stable, deterministic code.
- Codes do not change across runs.
- Codes match domain expectations.

**Patterns:**

- Throw known exceptions → assert expected error code.
- Throw unknown exceptions → assert fallback error code.

---

## 2.5 ProblemDetails Shaping Tests

Validate:

- Title, Detail, Status, Type are correct.
- No sensitive details appear when disabled.
- Validation errors appear in `Errors` when applicable.

**Patterns:**

- Simulate exceptions → snapshot test ProblemDetails.
- Toggle options → assert differences.

---

## 2.6 ExceptionHandlingOptions Tests

### IncludeExceptionDetails
- true → message + stack trace included  
- false → generic detail only  

### IncludeErrorCode
- true → error code included  
- false → omitted  

### LogUnhandledExceptions
- true → logs emitted  
- false → no logs  

**Patterns:**

- Toggle each option → assert response/logging behavior.

---

## 2.7 Middleware Behavior Tests

Validate:

- Exceptions thrown in the pipeline are caught.
- Correct handler is invoked.
- Correct HTTP status code is produced.
- Correct ProblemDetails is written to the response.

**Patterns:**

- Throw exceptions inside test endpoints → assert HTTP output.

---

## 2.8 Negative Tests

Validate:

- `CanHandle` never throws.
- `CreateProblemDetails` never returns null.
- `GetErrorCode` never returns null.
- Handlers do not mutate shared state.
- Registry resolution is stable under stress.

**Patterns:**

- Force invalid inputs → assert safe failure.
- Run handlers concurrently → assert immutability.

---

# 3. Capability Governance Tests

## 3.1 Capability Opt‑Out

Validate:

- Disabling the capability prevents handler registration.
- Registry is not created when capability is disabled.
- Middleware is not activated.

## 3.2 Handler‑Level Opt‑Out

Validate:

- Disabled handlers are not registered.
- Registry does not consider disabled handlers.
- Fallback handler still exists.

## 3.3 Capability Dependencies

Validate:

- Dependent capabilities behave correctly when ExceptionHandling is disabled.
- System fails loudly if a required capability is missing.

---

# 4. Test Isolation Requirements

Testers must ensure:

- Each test uses a fresh registry instance.
- Handlers are not reused across tests unless intended.
- No test relies on environment variables or system time.
- No test uses static/global state.
- No test leaks exception details unintentionally.

This ensures deterministic, isolated behavior.

---

# 5. Recommended Testing Patterns

### 5.1 Fake Handlers
Use fake handlers with predictable `CanHandle` behavior.

### 5.2 Snapshot Testing
Validate `ProblemDetails` output shape.

### 5.3 Log Capture
Validate logging behavior when `LogUnhandledExceptions` is enabled.

### 5.4 Order Verification
Assert handler ordering explicitly.

### 5.5 Stress Testing
Run resolution under concurrency to ensure immutability and determinism.

---

# 6. Anti‑Patterns (Tests Must Reject)

Tests must fail if they detect:

- Incorrect handler ordering.
- Incorrect handler resolution.
- Missing fallback handler.
- Leaked exception details when disabled.
- Missing error code when enabled.
- Missing logs when logging is enabled.
- Handlers throwing unexpectedly.
- `CanHandle` performing expensive or stateful operations.
- Registry resolution depending on registration order instead of `Order`.

---

# 7. Summary

Testers ensure that the ExceptionHandling capability:

- resolves handlers deterministically  
- respects ordering  
- produces correct error codes  
- shapes safe, consistent ProblemDetails  
- respects all ExceptionHandlingOptions  
- integrates correctly with API middleware  
- behaves correctly under capability governance  
- never leaks sensitive information  
- never leaves exceptions unhandled  

This unified Tester Guide covers everything needed to validate Frank’s exception handling system end‑to‑end.
