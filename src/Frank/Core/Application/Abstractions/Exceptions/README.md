# Exceptions

The **Exceptions** folder contains abstractions that define how the application classifies, maps, and responds to exceptions in a consistent, structured, and predictable way. This layer provides the foundation for a centralized exception‑handling pipeline that converts thrown exceptions into stable error codes and standardized `ProblemDetails` responses.

This folder includes interfaces and attributes used to identify exception handlers, determine handler ordering, and produce machine‑readable error output.

---

## Purpose

The exception‑handling subsystem ensures that all exceptions are:

- **classified** — mapped to stable, application‑defined error codes  
- **structured** — represented using a consistent `ProblemDetails` format  
- **deterministic** — processed in a predictable order  
- **extensible** — new handlers can be added without modifying existing ones  
- **decoupled** — business logic does not need to know how errors are surfaced  

This design provides a clean separation between exception throwing and exception presentation.

---

## Components

### ExceptionHandlerAttribute
Marks a class as an exception handler and defines its execution order.

```csharp
[AttributeUsage(AttributeTargets.Class)]
public sealed class ExceptionHandlerAttribute : Attribute
{
    public int Order { get; }
    public ExceptionHandlerAttribute(int order) => Order = order;
}
```

Handlers with lower `Order` values run earlier in the pipeline.

---

### IExceptionHandler
Defines how an exception is evaluated, classified, and transformed into a `ProblemDetails` response.

```csharp
public interface IExceptionHandler
{
    bool CanHandle(Exception exception);
    IErrorCode GetErrorCode(Exception exception);
    ProblemDetails CreateProblemDetails(Exception exception);
}
```

Each handler:

- decides whether it can handle the exception  
- maps the exception to an `IErrorCode`  
- produces a structured `ProblemDetails` response  

Handlers are automatically registered as **singletons**.

---

### IErrorCode
Represents a stable, application‑defined identifier for an error.

```csharp
public interface IErrorCode
{
    string Code { get; }
    string? Description => null;
}
```

Error codes:

- are deterministic  
- are safe to log and expose  
- help clients classify failures  
- provide a durable mapping between exceptions and responses  

---

### ProblemDetails
A structured, machine‑readable error response model.

```csharp
public class ProblemDetails
{
    public string Title { get; set; } = default!;
    public string Detail { get; set; } = default!;
    public int? Status { get; set; }
    public string Type { get; set; } = default!;
    public Dictionary<string, string[]>? Errors { get; set; }
}
```

This model aligns with the shape of RFC 7807 and is used by handlers to produce consistent API error responses.

---

## Design Principles

- **Centralized handling**  
  All exceptions flow through a unified pipeline.

- **Ordered processing**  
  Handlers execute in a deterministic sequence using `ExceptionHandlerAttribute`.

- **Stable classification**  
  Exceptions map to durable error codes via `IErrorCode`.

- **Structured output**  
  `ProblemDetails` ensures consistent error responses across the application.

- **Extensibility**  
  New handlers can be added without modifying existing ones.

- **Separation of concerns**  
  Business logic throws exceptions; handlers decide how to represent them.

---

## How Exceptions Fit Into the Application

The exception subsystem integrates with:

- endpoint pipelines  
- command and query dispatchers  
- domain event dispatchers  
- middleware layers  
- logging and telemetry systems  

This ensures that all exceptions — whether domain, application, or infrastructure — are surfaced in a predictable, structured, and client‑friendly manner.

---
