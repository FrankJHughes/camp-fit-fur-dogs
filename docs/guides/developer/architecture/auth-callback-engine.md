# Auth Callback Engine

This guide documents the **current** authentication callback pipeline as implemented in:

- `AuthCallbackExecutor`
- `AuthCallbackService`
- `IAuthCallbackStep`
- `AuthCallbackContext`
- `AuthCallbackDiagnosticEvent`

This is the **pre‑refactor** architecture and reflects the exact behavior of the existing system.

The callback engine is a **deterministic, invariant‑checked pipeline** that orchestrates the OIDC authorization‑code callback flow.

It is a strong candidate to become a **Frank Engine**, because it is:

- deterministic  
- pure  
- invariant‑checked  
- step‑driven  
- cross‑cutting  
- reusable  
- governed  

This guide describes the engine as it exists today.

---

# 1. Purpose

The Auth Callback Engine orchestrates the **OIDC callback flow**:

1. Validate configuration  
2. Exchange authorization code  
3. Fetch userinfo  
4. Validate userinfo  
5. Resolve identity  
6. Audit login  
7. Issue session cookie  
8. Create session  
9. Build redirect  

Each responsibility is implemented as a **step** (`IAuthCallbackStep`), and the engine executes them in a **state‑driven order**.

The engine itself contains:

- no business logic  
- no identity logic  
- no persistence logic  
- no HTTP logic  

It only:

- selects steps  
- executes steps  
- enforces invariants  
- emits diagnostics  

---

# 2. Architecture Overview

````text
AuthCallbackService
    ↓
AuthCallbackExecutor
    ↓
[IAuthCallbackStep] × N
    ↓
AuthCallbackContext (immutable core + mutable fields)
````

The engine is composed of:

| Component | Responsibility |
|----------|----------------|
| `AuthCallbackService` | Entry point, creates initial context, validates final state |
| `AuthCallbackExecutor` | Pipeline engine: step selection, execution, invariants |
| `IAuthCallbackStep` | A single unit of callback behavior |
| `AuthCallbackContext` | Shared state passed between steps |
| `AuthCallbackDiagnosticEvent` | Structured tracing for observability |

---

# 3. AuthCallbackService

`AuthCallbackService` is the **public API** of the callback engine.

````csharp
public sealed class AuthCallbackService : IAuthCallbackService
{
    private readonly AuthCallbackExecutor _executor;
    private readonly IClock _clock;

    public async Task<AuthCallbackResult> HandleAsync(string code, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new AuthCallbackException(AuthCallbackError.MissingAuthorizationCode);

        var ctx = new AuthCallbackContext(code, Now: _clock.UtcNow);

        ctx = await _executor.ExecuteAsync(ctx, ct);

        ctx.RequireCustomerId();
        ctx.RequireSession();
        ctx.RequireSessionCookie();
        ctx.RequireRedirectUrl();

        return new AuthCallbackResult(
            CustomerId.From(ctx.CustomerId!.Value),
            ctx.SessionCookie!,
            ctx.RedirectUrl!
        );
    }
}
````

### Responsibilities

- Validate the incoming authorization code  
- Create the initial context  
- Execute the pipeline  
- Validate final state  
- Return a strongly typed result  

### Not responsible for

- HTTP  
- Cookies  
- Identity mapping  
- Session creation  
- Token exchange  

All of that happens inside steps.

---

# 4. AuthCallbackExecutor

This is the **pipeline engine**.

````csharp
public sealed class AuthCallbackExecutor
{
    private readonly IReadOnlyList<IAuthCallbackStep> _steps;
    private readonly Action<AuthCallbackDiagnosticEvent>? _trace;

    public async Task<AuthCallbackContext> ExecuteAsync(AuthCallbackContext ctx, CancellationToken ct)
    {
        var remaining = new HashSet<IAuthCallbackStep>(_steps);

        while (TrySelectNextStep(remaining, ctx, out var step))
        {
            var before = ctx;

            ctx = await ExecuteStep(
                step!,
                before,
                emitBefore: EmitStartEvent,
                emitAfter: EmitEndEvent,
                ct);

            AssertValidTransition(step!, before, ctx);

            remaining.Remove(step!);
        }

        return ctx;
    }
}
````

### Responsibilities

- Select the next executable step  
- Execute the step  
- Emit diagnostics  
- Enforce invariants  
- Prevent illegal state transitions  

### Not responsible for

- Business logic  
- Identity logic  
- Persistence  
- HTTP  
- Error shaping  

---

# 5. Step Selection

````csharp
private static bool TrySelectNextStep(
    HashSet<IAuthCallbackStep> remaining,
    AuthCallbackContext ctx,
    out IAuthCallbackStep? step)
{
    step = remaining.FirstOrDefault(s => s.CanExecute(ctx));
    return step is not null;
}
````

### Rules

- Steps declare readiness via `CanExecute(ctx)`  
- Steps run **at most once**  
- Steps run **only when ready**  
- Order is **state‑driven**, not index‑driven  

This allows:

- conditional steps  
- optional steps  
- dynamic ordering  
- future extensibility  

---

# 6. Step Execution

````csharp
private static async Task<AuthCallbackContext> ExecuteStep(
    IAuthCallbackStep step,
    AuthCallbackContext before,
    Action<IAuthCallbackStep, AuthCallbackContext>? emitBefore,
    Action<IAuthCallbackStep, AuthCallbackContext, AuthCallbackContext, long>? emitAfter,
    CancellationToken ct)
{
    emitBefore?.Invoke(step, before);

    var sw = Stopwatch.StartNew();
    var after = await step.ExecuteAsync(before, ct);
    sw.Stop();

    emitAfter?.Invoke(step, before, after, sw.ElapsedMilliseconds);

    return after;
}
````

### Rules

- Steps receive the **entire context**  
- Steps return a **new context**  
- Steps may not mutate the old context  
- Steps may not return null  
- Steps may not clear previously set fields  

---

# 7. Invariant Enforcement

````csharp
private static void AssertValidTransition(
    IAuthCallbackStep step,
    AuthCallbackContext before,
    AuthCallbackContext? after)
{
    if (after is null)
        throw new InvalidOperationException(...);

    AssertImmutable(step, before, after);
    AssertNoClearing(step, before, after);
}
````

### Immutable Fields

````csharp
if (after.Code != before.Code) throw ...
if (after.Now != before.Now) throw ...
````

### No‑Clearing Rule

````csharp
EnsureNotCleared(before.Token, after.Token, step, "Token");
EnsureNotCleared(before.User, after.User, step, "User");
...
````

### Why this matters

- Prevents step bugs  
- Prevents accidental data loss  
- Ensures monotonic context growth  
- Guarantees deterministic pipeline behavior  

---

# 8. Diagnostics

Two events are emitted per step:

- **Start**  
- **End** (with duration)

````csharp
_trace?.Invoke(new AuthCallbackDiagnosticEvent(...));
````

This enables:

- observability  
- performance profiling  
- debugging  
- structured logging  

---

# 9. Step Contract

A step implements:

````csharp
public interface IAuthCallbackStep
{
    AuthCallbackStepMetadata Metadata { get; }
    bool CanExecute(AuthCallbackContext ctx);
    Task<AuthCallbackContext> ExecuteAsync(AuthCallbackContext ctx, CancellationToken ct);
}
````

### Rules

- `CanExecute` must be pure  
- `ExecuteAsync` must be deterministic  
- Steps must not throw for expected conditions  
- Steps must not mutate the input context  
- Steps must return a new context  

---

# 10. Context Contract

`AuthCallbackContext` contains:

- Immutable fields:  
  - `Code`  
  - `Now`  

- Mutable fields (monotonic):  
  - `Token`  
  - `User`  
  - `CustomerId`  
  - `TokenHash`  
  - `Session`  
  - `SessionCookie`  
  - `RedirectUrl`  

### Rules

- Fields may be set once  
- Fields may never be cleared  
- Fields may never be overwritten  
- Steps must not mutate existing values  

---

# 11. Error Model

The engine throws:

- `InvalidOperationException` for invariant violations  
- Step‑specific exceptions for business failures  
- `AuthCallbackException` for top‑level validation  

All errors flow through:

- Frank’s global error boundary  
- ProblemDetails shaping  

---

# 12. Extensibility Model

Steps can be:

- added  
- removed  
- reordered (via CanExecute logic)  
- replaced  

The engine does not assume:

- fixed ordering  
- fixed step count  
- fixed behavior  

This makes it suitable for:

- multi‑tenant identity providers  
- custom identity mapping  
- custom session logic  
- custom auditing  

---

# 13. Testing Strategy

### Unit Tests

- Step tests  
- Context transition tests  
- Invariant tests  

### Integration Tests

- Full callback flow  
- Error conditions  
- Missing fields  
- Invalid transitions  

### Diagnostic Tests

- Start/End events emitted  
- Duration captured  

---

# 14. Why This Should Become a Frank Engine

| Reason | Explanation |
|--------|-------------|
| Deterministic pipeline | Matches Frank Dispatcher |
| Invariant‑checked | Matches Frank Validation Pipeline |
| Cross‑cutting | Applies to all products using OIDC |
| Pure orchestration | No business logic |
| Extensible | Step‑based architecture |
| Observable | Structured diagnostics |
| Governed | Security + Identity + Session governance |

This is a **core engine**, not product logic.

---

# 15. Summary

The Auth Callback Engine is:

- deterministic  
- state‑driven  
- invariant‑checked  
- extensible  
- observable  
- pure  

It orchestrates the entire OIDC callback flow and is a perfect candidate for promotion into **Frank** as a first‑class engine.

