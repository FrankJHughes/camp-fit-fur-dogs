# Query Dispatcher — User Guide

The Query Dispatcher capability provides a **simple, consistent way to execute read‑only queries** in your application.  
It handles validation, handler resolution, and execution automatically, so you can focus on writing clear, intention‑revealing queries and handlers.

This guide explains how to **use** the Query Dispatcher in your application code.

---

## 1. What the Query Dispatcher Does

When you send a query through the dispatcher:

1. All validators for that query run automatically  
2. If validation succeeds, the dispatcher resolves the correct handler  
3. The handler executes the query  
4. The dispatcher returns the handler’s result  
5. If validation fails, a `ValidationException` is thrown  

You do **not** need to manually:

- run validators  
- resolve handlers  
- wire up DI  
- handle validation errors  

The dispatcher does all of this for you.

---

## 2. When You Use This Capability

Use the Query Dispatcher whenever you want to:

- fetch data  
- look up information  
- check system state  
- retrieve a projection or DTO  

Queries represent **read‑only operations** and must not modify state.

Examples:

- Get a customer by ID  
- List all dogs for an owner  
- Retrieve reservation details  
- Fetch dashboard metrics  

---

## 3. How to Dispatch a Query

Inject the dispatcher:

```csharp
public class CustomerController
{
    private readonly IQueryDispatcher _dispatcher;

    public CustomerController(IQueryDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }
}
```

### Dispatching a query

```csharp
var result = await _dispatcher.DispatchAsync(
    new GetCustomerByIdQuery(customerId),
    ct);
```

The dispatcher:

- runs validators  
- resolves the correct handler  
- executes the handler  
- returns the result  

---

## 4. How to Define a Query

Queries always return a value.

```csharp
public sealed record GetCustomerByIdQuery(Guid CustomerId)
    : IQuery<CustomerDto>;
```

Queries should be:

- immutable  
- intention‑revealing  
- simple data carriers  

---

## 5. How to Implement a Handler

```csharp
public sealed class GetCustomerByIdHandler
    : IQueryHandler<GetCustomerByIdQuery, CustomerDto>
{
    public async Task<CustomerDto> HandleAsync(GetCustomerByIdQuery query, CancellationToken ct)
    {
        // fetch data
        return dto;
    }
}
```

Handlers are automatically registered via `AutoRegister`.

---

## 6. How Validation Works

If you add a FluentValidation validator for a query:

```csharp
public sealed class GetCustomerByIdValidator : AbstractValidator<GetCustomerByIdQuery>
{
    public GetCustomerByIdValidator()
    {
        RuleFor(x => x.CustomerId).NotEmpty();
    }
}
```

The dispatcher will:

- find the validator  
- run it automatically  
- throw a `ValidationException` if it fails  
- skip the handler if validation fails  

You do **not** need to call validators manually.

---

## 7. What You Don’t Need to Worry About

The dispatcher handles:

- validator discovery  
- validation execution  
- handler resolution  
- dynamic dispatch  
- DI lifetime rules  
- error propagation  

You do **not** need to:

- manually resolve handlers  
- manually run validators  
- write boilerplate try/catch logic  
- manage multiple handlers  

The dispatcher enforces a clean, predictable workflow.

---

## 8. Common Failure Cases

The dispatcher will throw if:

- validation fails (`ValidationException`)  
- no handler is registered for a query  
- the handler throws an exception  
- the handler returns the wrong type  
- DI cannot resolve the handler  

Your application code should treat these as normal operational errors.

---

## 9. Summary

The Query Dispatcher gives you:

- a unified way to execute queries  
- automatic validation  
- automatic handler resolution  
- clean separation of intent and behavior  
- predictable, testable query execution  

As a user of this capability:

- you define queries  
- you implement handlers  
- you optionally add validators  
- you call the dispatcher  

Everything else is handled for you.
