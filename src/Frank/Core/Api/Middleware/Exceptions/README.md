# Exceptions Middleware

The **Exceptions Middleware** subsystem provides centralized, structured exception
handling for the Frank.Core API.  
It ensures that all unhandled exceptions thrown by vertical slices or framework
components are consistently unwrapped, resolved, and transformed into
`ProblemDetails` responses.

This folder contains the middleware responsible for intercepting exceptions and
the extension method used to register it in the ASP.NET Core pipeline.

---

## Files

```
Exceptions/
├── ExceptionHandlingMiddleware.cs
└── ApplicationBuilderExtensions.cs
```

---

## ExceptionHandlingMiddleware

`ExceptionHandlingMiddleware` is the core of the exception-handling pipeline.  
It intercepts unhandled exceptions, unwraps common wrapper exceptions, resolves
the correct handler from the `ExceptionHandlerRegistry`, and writes a structured
`ProblemDetails` response.

### Responsibilities

- Executes the next middleware and captures unhandled exceptions.
- Unwraps common wrapper exceptions:
  - `TargetInvocationException`
  - `InvalidOperationException`
  - `AggregateException`
- Resolves the correct `IExceptionHandler` via `ExceptionHandlerRegistry`.
- Produces a `ProblemDetails` instance using the resolved handler.
- Writes the response with the correct HTTP status code.

### Why this matters

This middleware ensures:

- consistent error responses across all vertical slices  
- predictable HTTP status codes  
- structured diagnostic output  
- safe unwrapping of framework-level wrapper exceptions  
- separation of concerns between exception handling and business logic  

### Typical behavior

For a thrown exception:

```
{
  "type": "https://example.com/errors/not-found",
  "title": "Resource Not Found",
  "status": 404,
  "detail": "The requested dog could not be found."
}
```

The exact shape depends on the registered exception handler.

---

## ApplicationBuilderExtensions

`ApplicationBuilderExtensions` provides a single extension method for adding the
exception-handling middleware to the ASP.NET Core pipeline.

### Responsibilities

- Registers `ExceptionHandlingMiddleware` via `UseMiddleware`.
- Provides a clean, discoverable API for enabling centralized exception handling.

### Usage

```csharp
app.UseFrankCoreApiMiddlewareExceptions();
```

This should be placed **early in the pipeline**, typically:

1. After routing  
2. Before endpoint execution  

This ensures all slice-level exceptions are captured.

---

## How Exceptions Middleware Fits Into the Architecture

Exception handling is part of the API’s cross-cutting infrastructure.  
It integrates tightly with:

- `ExceptionHandlerRegistry`  
- slice-specific exception handlers  
- infrastructure-level exception abstractions  
- observability and diagnostics subsystems  

The middleware ensures that:

- slices remain free of boilerplate try/catch logic  
- exception handling is centralized and consistent  
- ProblemDetails responses follow a unified format  
- wrapper exceptions are safely unwrapped before resolution  

---

## Typical Flow

1. **Request enters pipeline**  
2. **Middleware executes next component**  
3. **Exception is thrown**  
4. **Middleware unwraps wrapper exceptions**  
5. **Registry resolves correct handler**  
6. **Handler creates ProblemDetails**  
7. **Middleware writes JSON response**  
8. **Client receives structured error**

---

## Design Principles

- **Centralized handling**  
  All exceptions flow through one middleware.

- **Deterministic resolution**  
  Registry selects the correct handler based on exception type.

- **Structured output**  
  Responses use `ProblemDetails` for consistency.

- **Safe unwrapping**  
  Common wrapper exceptions are removed to expose the root cause.

- **Slice-friendly**  
  Slices do not need to implement their own exception pipelines.

---

## Notes

- Middleware requires a configured `ExceptionHandlerRegistry`.
- Handlers should be deterministic and side-effect free.
- Logging is typically performed inside handlers or observers.
- This subsystem is safe for all environments, including production.

---
