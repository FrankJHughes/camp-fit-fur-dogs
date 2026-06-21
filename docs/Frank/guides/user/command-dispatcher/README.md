# Command Dispatcher — User Guide

The Command Dispatcher capability provides a **simple, consistent way to execute commands** in your application.  
It handles validation, handler resolution, and execution automatically, so you can focus on writing clean, intention‑revealing commands and handlers.

This guide explains how to **use** the Command Dispatcher in your application code.

---

## 1. What the Command Dispatcher Does

When you send a command through the dispatcher:

1. All validators for that command run automatically  
2. If validation succeeds, the dispatcher resolves the correct handler  
3. The handler executes the command  
4. If the command returns a value, the dispatcher returns it  
5. If validation fails, a `ValidationException` is thrown  

You do **not** need to manually:

- run validators  
- resolve handlers  
- wire up DI  
- handle validation errors  

The dispatcher does all of this for you.

---

## 2. When You Use This Capability

You use the Command Dispatcher whenever you want to:

- perform an action  
- change system state  
- trigger a workflow  
- run business logic that is not a query  

Examples:

- Create a customer  
- Register a dog  
- Send a welcome email  
- Update a reservation  
- Cancel a booking  

Commands represent **intent**, not data structures.

---

## 3. How to Dispatch a Command

Inject the dispatcher:

````csharp
public class CustomerController
{
    private readonly ICommandDispatcher _dispatcher;

    public CustomerController(ICommandDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }
}
````

### Commands with a return value

````csharp
var result = await _dispatcher.DispatchAsync(
    new CreateCustomerCommand(...),
    ct);
````

### Commands without a return value

````csharp
await _dispatcher.DispatchAsync(
    new SendWelcomeEmailCommand(...),
    ct);
````

That’s it — the dispatcher handles everything else.

---

## 4. How to Define a Command

### Commands with no return value

````csharp
public sealed record SendWelcomeEmailCommand(Guid CustomerId) : ICommand;
````

### Commands with a return value

````csharp
public sealed record CreateCustomerCommand(string Name, string Email)
    : ICommand<Guid>;
````

Commands should be:

- immutable  
- intention‑revealing  
- free of behavior  
- simple data carriers  

---

## 5. How to Implement a Handler

### Handler for a command with no return value

````csharp
public sealed class SendWelcomeEmailHandler
    : ICommandHandler<SendWelcomeEmailCommand>
{
    public Task HandleAsync(SendWelcomeEmailCommand command, CancellationToken ct)
    {
        // perform action
        return Task.CompletedTask;
    }
}
````

### Handler for a command with a return value

````csharp
public sealed class CreateCustomerHandler
    : ICommandHandler<CreateCustomerCommand, Guid>
{
    public async Task<Guid> HandleAsync(CreateCustomerCommand command, CancellationToken ct)
    {
        // create customer
        return customerId;
    }
}
````

Handlers are automatically registered via `AutoRegister`.

---

## 6. How Validation Works

If you add a FluentValidation validator for a command:

````csharp
public sealed class CreateCustomerValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerValidator()
    {
        RuleFor(x => x.Email).EmailAddress();
        RuleFor(x => x.Name).NotEmpty();
    }
}
````

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
- no handler is registered for a command  
- the handler throws an exception  
- the handler returns the wrong type  
- DI cannot resolve the handler  

Your application code should treat these as normal operational errors.

---

## 9. Summary

The Command Dispatcher gives you:

- a unified way to execute commands  
- automatic validation  
- automatic handler resolution  
- clean separation of intent and behavior  
- predictable, testable command execution  

As a user of this capability:

- you define commands  
- you implement handlers  
- you optionally add validators  
- you call the dispatcher  

Everything else is handled for you.
