# Frank ExceptionHandling — User Guide

The ExceptionHandling capability provides a **safe, predictable, structured way** for your application to convert exceptions into consistent error responses.  
As a user of this capability, you do not implement the pipeline — you **use** it by:

- throwing meaningful exceptions  
- relying on the system to classify them  
- receiving consistent `ProblemDetails` responses  
- configuring how much detail is exposed  

This guide explains how to use the capability safely and effectively.

---

# 1. What This Capability Does for You

When an exception occurs anywhere in the API:

1. The system finds the correct exception handler.
2. The handler classifies the exception into an `IErrorCode`.
3. The handler produces a structured `ProblemDetails` object.
4. The API layer converts it into an HTTP response.
5. Options determine whether details and error codes are included.
6. Logging occurs if enabled.

You get:

- consistent error responses  
- safe detail handling  
- predictable behavior across all endpoints  
- no need to manually catch and format exceptions  

---

# 2. What You Need to Do

## 2.1 Throw meaningful exceptions

The system works best when you throw domain‑specific exceptions:

```csharp
throw new OwnerNotFoundException(ownerId);
```

Handlers will:

- detect the exception  
- classify it  
- produce the correct error code  
- generate a structured response  

## 2.2 Do not catch exceptions unless you must

Let the system handle them unless:

- you can recover  
- you need to add context  
- you need to convert to a domain exception  

Otherwise:

```csharp
// Let the pipeline handle it
```

## 2.3 Configure ExceptionHandlingOptions

You can control how much detail is exposed:

```csharp
services.Configure<ExceptionHandlingOptions>(o =>
{
    o.IncludeExceptionDetails = env.IsDevelopment();
    o.IncludeErrorCode = true;
    o.LogUnhandledExceptions = true;
});
```

### IncludeExceptionDetails
- true → message + stack trace included  
- false → safe, generic messages  

### IncludeErrorCode
- true → stable error code included  
- false → omitted  

### LogUnhandledExceptions
- true → logs exceptions  
- false → suppresses logging  

---

# 3. What You Should Expect in API Responses

When an exception occurs, you receive a structured `ProblemDetails` response:

```json
{
  "title": "Owner Not Found",
  "detail": "The owner with ID 123 was not found.",
  "status": 404,
  "type": "https://errors.frank.dev/owner-not-found",
  "errors": null
}
```

If `IncludeExceptionDetails = false`, sensitive information is removed.

If `IncludeErrorCode = true`, the error code is included (API layer adds it).

---

# 4. How to Add or Replace Exception Handlers

If you need custom behavior:

1. Implement `IExceptionHandler`
2. Add `[ExceptionHandler(order)]`
3. Register it (auto‑registration handles this)

Example:

```csharp
[ExceptionHandler(200)]
public sealed class CustomAuthExceptionHandler : IExceptionHandler
{
    public bool CanHandle(Exception ex) => ex is AuthException;

    public IErrorCode GetErrorCode(Exception ex) => AuthErrorCodes.InvalidCredentials;

    public ProblemDetails CreateProblemDetails(Exception ex)
        => new()
        {
            Title = "Authentication Failed",
            Detail = ex.Message,
            Status = 401,
            Type = "https://errors.frank.dev/auth-failed"
        };
}
```

Your handler will automatically participate in the pipeline.

---

# 5. How to Disable the Capability (Opt‑Out)

If your application uses capability governance, you can disable ExceptionHandling entirely:

- disable the capability in configuration  
- or disable specific handlers  
- or replace the registry with your own  

When disabled:

- handlers are not registered  
- registry is not created  
- middleware is not activated  

This is useful for:

- minimal APIs  
- internal services  
- custom error pipelines  

---

# 6. Best Practices

- Throw domain exceptions, not generic ones.
- Avoid catching exceptions unless you can recover.
- Never expose sensitive details in production.
- Always keep a fallback handler enabled.
- Use error codes consistently across your domain.
- Prefer `ProblemDetails` for all error responses.

---

# 7. Anti‑Patterns

Avoid:

- returning ad‑hoc error objects  
- throwing raw `Exception` for domain errors  
- leaking stack traces in production  
- writing your own error responses manually  
- relying on handler registration order instead of `[ExceptionHandler(order)]`  

---

# 8. Summary

As a user of the ExceptionHandling capability:

- You throw meaningful exceptions.  
- The system classifies them into error codes.  
- Handlers produce structured `ProblemDetails`.  
- Options control detail and logging.  
- The API layer returns consistent, safe error responses.  
- Capability governance lets you enable/disable the entire system.  

This unified User Guide gives you everything needed to use Frank’s exception handling safely and effectively.
