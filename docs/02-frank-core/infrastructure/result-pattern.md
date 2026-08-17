# Frank.Core.Infrastructure — Result Pattern

The **Result pattern** provides explicit, predictable success/failure handling without relying on exceptions for domain‑level control flow. It keeps domain logic pure, application handlers clean, and error handling fully testable. Instead of throwing exceptions for validation or business‑rule failures, operations return a `Result<T>` that clearly indicates whether the operation succeeded.

This document maps the Result subsystem under:

```
docs/02-frank-core/infrastructure
```

back to its implementation in:

```
src/Frank/Core/Infrastructure/Results
```

---

## Purpose

The Result pattern exists to:

- eliminate exception‑driven domain logic  
- make failures explicit and predictable  
- simplify handler control flow  
- improve testability of error conditions  
- unify success/failure semantics across vertical slices  

It is a foundational pattern for domain correctness and handler clarity.

---

## Result<T> Type

A `Result<T>` represents either a successful value or a failure:

```csharp
public abstract record Result<T>
{
    public sealed record Success(T Value) : Result<T>;
    public sealed record Failure(string Error) : Result<T>;
}
```

Key characteristics:

- **Success** wraps a value  
- **Failure** wraps an error message  
- no exceptions are thrown for domain validation  
- consumers must handle both cases explicitly  

---

## Usage in Domain Logic

Domain operations return `Result<T>` instead of throwing:

```csharp
public static Result<DogName> Create(string value)
{
    if (string.IsNullOrWhiteSpace(value))
        return Result.Failure<DogName>("Name required");

    if (value.Length > 100)
        return Result.Failure<DogName>("Name too long");

    return Result.Success(new DogName(value));
}
```

This keeps domain invariants explicit and testable.

---

## Usage in Handlers

Handlers consume results explicitly:

```csharp
var nameResult = DogName.Create(command.Name);

if (nameResult is Result<DogName>.Failure f)
    return Results.BadRequest(new { error = f.Error });

var name = (nameResult as Result<DogName>.Success)!.Value;
```

This prevents exceptions from leaking into API responses and keeps handler logic clear.

---

## Chaining Operations

Results can be chained using helper methods such as `Bind` and `Map`:

```csharp
var dog = Dog.Create(ownerId, name, breed, dateOfBirth, sex)
    .Bind(d => _dogWriter.AddAsync(d, ct))
    .Bind(d => _unitOfWork.CommitAsync().Map(_ => d));
```

Chaining benefits:

- eliminates nested `if` statements  
- keeps handler logic linear and readable  
- propagates failures automatically  

---

## Exception Handling at Boundaries

Exceptions are converted to `Result<T>` at application boundaries:

```csharp
try
{
    return await handler.HandleAsync(command, ct);
}
catch (Exception ex)
{
    Logger.LogError(ex, "Handler error");
    return Result.Failure("Internal error");
}
```

This ensures:

- domain logic uses results, not exceptions  
- infrastructure captures unexpected failures  
- API receives structured error responses  

---

## How the Result Pattern Connects to the Broader Platform

The Result pattern collaborates with:

- **Frank.Core.Domain**  
  Domain invariants return failures instead of throwing.

- **Frank.Core.Application**  
  Handlers consume results and map them to HTTP responses.

- **Frank.Core.Infrastructure**  
  Exception handling converts unexpected errors into `Result<T>`.

- **Frank.Core.Api**  
  Middleware and endpoints translate results into API responses.

This creates a consistent error‑handling model across all vertical slices.

---

## Runtime Collaboration Points

The Result pattern interacts with the runtime by:

- preventing exception‑driven control flow  
- enabling deterministic error handling  
- simplifying handler logic  
- improving testability of failure conditions  
- integrating cleanly with exception middleware  

It is a core building block for predictable application behavior.

---

## Composition Flow (API → Application → Domain → Persistence)

```
API Request
    ↓
Application Handler
    ↓
Domain Operation (returns Result<T>)
    ↓
Handler Maps Result<T> → HTTP Response
    ↓
Unit of Work Commit (if successful)
```

Failures are explicit, structured, and never thrown as exceptions.

---

## Notes

Keep this document grounded in the actual Frank.Core.Infrastructure Result implementation.  
Whenever result‑chaining helpers, mapping conventions, or handler patterns evolve, update this section to reflect the current platform architecture.
