# Frank.Core.Infrastructure — Validation

The **validation subsystem** in Frank.Core provides a unified, composable, and testable approach for validating domain models, commands, and queries. Validation is explicit, declarative, and layered so that each part of the system validates only what it is responsible for—without duplication or hidden side effects.

This document maps the validation subsystem under:

```
docs/02-frank-core/infrastructure
```

back to its implementation in:

```
src/Frank/Core/Infrastructure/Validation
```

---

## Validation Principle

Validation in Frank.Core follows three core rules:

- **Domain objects validate invariants**  
  Domain models enforce correctness at construction time.

- **Commands validate input via `ICommandValidator`**  
  Application‑level inputs are validated before handlers run.

- **Queries validate input via `IQueryValidator`**  
  Query parameters are validated before execution.

Handlers do **not** duplicate validation logic. They assume validated input.

This keeps validation declarative, predictable, and easy to test.

---

## Domain Validation

Domain value objects enforce invariants at creation time. They never allow invalid state.

```csharp
public sealed record DogName
{
    public string Value { get; }

    private DogName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidOperationException("Name required");

        if (value.Length > 100)
            throw new InvalidOperationException("Name too long");

        Value = value;
    }

    public static Result<DogName> Create(string value) =>
        string.IsNullOrWhiteSpace(value)
            ? Result.Failure<DogName>("Name required")
            : value.Length > 100
                ? Result.Failure<DogName>("Name too long")
                : Result.Success(new DogName(value));
}
```

Key points:

- Domain invariants are enforced in constructors.
- The `Result<T>` pattern provides safe, exception‑free creation.
- Invalid domain objects are never instantiated.

---

## Command Validation

Commands use validators implementing `ICommandValidator<T>`.

```csharp
public sealed class RegisterDogCommandValidator : ICommandValidator<RegisterDogCommand>
{
    public async Task<ValidationResult> ValidateAsync(
        RegisterDogCommand command,
        CancellationToken ct)
    {
        var errors = new List<ValidationError>();

        if (string.IsNullOrWhiteSpace(command.Name))
            errors.Add(new ValidationError("Name required", "Name"));

        if (command.DateOfBirth > DateOnly.FromDateTime(DateTime.Now))
            errors.Add(new ValidationError("Invalid birth date", "DateOfBirth"));

        return errors.Any()
            ? ValidationResult.Failure(errors)
            : ValidationResult.Success();
    }
}
```

Validators are:

- automatically discovered via assembly scanning  
- registered during DI setup  
- executed before handlers run  

This ensures handlers always receive valid commands.

---

## Query Validation

Queries follow the same pattern as commands:

- `IQueryValidator<TQuery>`  
- declarative validation rules  
- executed before query handlers  

This prevents invalid query parameters from reaching application logic.

---

## Validation Failure Handling

Handlers run only after validation succeeds:

```csharp
var validationResult = await validator.ValidateAsync(command, ct);

if (!validationResult.IsValid)
    return Results.BadRequest(validationResult.Errors);
```

Validation responses:

- include all errors  
- include field names  
- are client‑friendly  
- support UI display and form binding  

This provides a consistent validation experience across the platform.

---

## How Validation Connects to the Broader Platform

Validation collaborates with:

- **Frank.Core.Domain**  
  Domain invariants ensure internal correctness.

- **Frank.Core.Application**  
  Validators ensure external input is safe and complete.

- **Frank.Core.Infrastructure**  
  Validation results integrate with exception handling and result patterns.

- **Frank.Core.Api**  
  Validation errors map cleanly to HTTP responses.

This layered approach keeps validation clean, predictable, and maintainable.

---

## Runtime Collaboration Points

Validation interacts with the runtime by:

- preventing invalid commands/queries from reaching handlers  
- ensuring domain objects are always valid  
- integrating with the Result pattern for safe object creation  
- producing structured validation errors for clients  

It is a core part of the vertical slice lifecycle.

---

## Composition Flow (API → Application → Domain)

```
API Request
    ↓
Command/Query Validator
    ↓
Application Handler
    ↓
Domain Object Creation (Result<T>)
    ↓
Persistence / Response
```

Validation ensures correctness at every stage.

---

## Notes

Keep this document grounded in the actual Frank.Core.Infrastructure validation implementation.  
Whenever validation patterns, result types, or handler conventions evolve, update this section to reflect the current platform architecture.
