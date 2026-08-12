# Validation

The **Validation** folder contains API‑level helpers that streamline the use of
FluentValidation within endpoint handlers.  
Rather than manually invoking validators and checking results, this folder
provides a concise extension method that integrates cleanly with the API’s
exception‑handling pipeline.

This keeps endpoint code expressive, predictable, and free of repetitive
validation boilerplate.

---

## Files

```
Validation/
└── ValidationExtensions.cs
```

---

## ValidationExtensions.cs

Provides a fluent extension method for validating request DTOs using
FluentValidation.

### Responsibilities

- Execute `ValidateAndThrowAsync` on a given validator  
- Throw a `ValidationException` when validation fails  
- Return the original request object for fluent chaining  
- Integrate seamlessly with `ValidationExceptionHandler` in the API layer

### Usage

Inside an endpoint handler:

```csharp
await request.Validate(validator, ct);
```

This pattern:

1. Runs FluentValidation rules  
2. Throws a `ValidationException` on failure  
3. Allows the global exception handler to produce a structured `400 Bad Request`  
4. Returns the validated request for further processing

### Example

```csharp
public async Task<IResult> HandleAsync(MyRequest request, CancellationToken ct)
{
    await request.Validate(_validator, ct);

    // Request is now guaranteed valid
    return Results.Ok(await _service.DoSomethingAsync(request, ct));
}
```

---

## Design Principles

Validation helpers follow these principles:

- **Expressiveness** — endpoint code reads cleanly and declaratively  
- **Consistency** — all validation failures flow through the same exception handler  
- **Separation of concerns** — validators define rules; handlers consume validated data  
- **Predictability** — validation always throws, never returns partial results  
- **Minimalism** — only one extension method, no unnecessary abstractions  

---

## Summary

The Validation folder provides a simple but powerful API‑level helper:

- One extension method  
- Full FluentValidation integration  
- Automatic routing to structured error responses  
- Clean endpoint code with fluent request validation

This structure ensures validation is consistent, predictable, and easy to use
across the entire Camp Fit Fur Dogs API.

