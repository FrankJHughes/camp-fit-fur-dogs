# Frank — Guides — User  
Authoritative handbook for developers building applications with Frank

Welcome to the **Frank User Guide** — the handbook for developers, engineers, and teams who *use* the Frank Framework to build applications.  
This guide explains how to work with Frank’s capabilities, how to structure applications using Frank’s conventions, and how to leverage Frank’s deterministic, governed architecture in your own products.

If you are building or maintaining the Frank Framework itself, see the **Frank Developer Guide**.  
If you are testing Frank or using Frank.Testing, see the **Frank Tester Guide**.

This guide is for **users of Frank**, not contributors to Frank.

---

# 1. What Is Frank?

Frank is a **deterministic, capability‑oriented application framework** designed to provide:

- predictable startup  
- governed hosting  
- strict purity  
- deterministic configuration  
- automatic DI registration  
- structured validation  
- consistent error handling  
- secure defaults  
- structured, correlated observability (NEW)  
- a unified testing harness  

Frank is designed for teams who want:

- strong architectural guardrails  
- predictable behavior across environments  
- a clear separation of capabilities  
- a governed, opinionated foundation  
- a framework that enforces correctness  

As a Frank user, you consume these capabilities to build your product.

---

# 2. How to Use This Guide

This guide provides:

- an overview of Frank’s capabilities  
- how to use each capability in your application  
- how to structure your project using Frank conventions  
- how to configure hosting, environment, and DI  
- how to work with Frank’s validation and pipeline systems  
- how to use Frank.Testing for integration tests  
- how to use Frank’s observability primitives (NEW)  

Capability‑specific user guides live in:

```
docs/frank/guides/user/<capability>/README.md
```

Examples:

- `testing/`  
- `authentication/`  
- `hosting/`  
- `environment/`  
- `configuration/`  
- `observability/` (NEW)  

---

# 3. Frank’s Core Concepts (User‑Facing)

Frank provides several core concepts that you will use in every application.

## 3.1 Capabilities

Frank is organized into **capabilities**, each representing a cohesive subsystem:

- Hosting  
- Startup  
- Environment  
- Configuration  
- Dependency Injection  
- Validation  
- Dispatching  
- Security Headers  
- Error Boundaries  
- Observability (NEW)  
- Testing  

Each capability has:

- a purpose  
- a boundary  
- a user guide  
- a developer guide (internal)  

As a user, you interact with the **public surface** of each capability.

---

## 3.2 Deterministic Startup

Frank applications start the same way every time:

- hosting provider selection  
- environment resolution  
- configuration layering  
- DI auto‑registration  
- startup module execution  
- observability context creation (NEW)  

You do not write your own startup pipeline — Frank handles it.

---

## 3.3 Hosting Providers

Frank supports multiple hosting providers (local, preview, production).  
Your application does not choose the provider — Frank does.

You configure hosting via:

- environment abstraction  
- configuration  
- hosting metadata  

---

## 3.4 Environment Abstraction

Frank replaces environment variables with a pure interface:

```csharp
IEnvironment env
```

This ensures:

- deterministic behavior  
- testability  
- no ambient state  

You never access `Environment.GetEnvironmentVariable`.

---

## 3.5 Configuration

Frank provides deterministic configuration layering:

1. defaults  
2. environment  
3. hosting provider  
4. secrets  
5. overrides  

You consume configuration via:

```csharp
IConfiguration config
```

---

## 3.6 Dependency Injection

Frank auto‑registers services using attributes:

```csharp
[RegisterSingleton]
public class MyService : IMyService { }
```

Manual registration is still available:

```csharp
services.AddSingleton<IMyService, MyService>();
```

---

## 3.7 Validation

Frank automatically discovers validators and enforces validation rules before handlers run.

You write validators using:

```csharp
public class CreateCustomerValidator : AbstractValidator<CreateCustomer> { }
```

---

## 3.8 Dispatcher Pipeline

Frank provides a deterministic request pipeline:

- validation  
- authorization  
- handler execution  
- result mapping  
- error boundaries  
- observability events (NEW)  

You write handlers, not controllers.

---

## 3.9 Security Headers

Frank automatically applies secure HTTP headers to all responses.  
No configuration required.

---

## 3.10 Error Boundaries

Frank ensures consistent error handling:

- validation errors  
- domain errors  
- infrastructure errors  
- unhandled exceptions  

You do not write your own exception middleware.

---

## 3.11 Observability (NEW)

Frank provides structured, correlated observability:

- `IObservabilityContext` — immutable correlation context  
- `ITraceEvents` — structured event emission  
- `IMetrics` — deterministic metrics  
- automatic correlation propagation  
- deterministic event/metric naming conventions  

As a user, you:

- emit events at handler boundaries  
- emit metrics for long‑running operations  
- never create correlation IDs manually  
- never log secrets, tokens, or PII  
- never use vendor‑specific logging or metrics  

---

# 4. Building Applications With Frank

## 4.1 Project Structure

A typical Frank application follows this structure:

```
src/
  <Product>.Api/
  <Product>.Application/
  <Product>.Domain/
  <Product>.Infrastructure/
tests/
  <Product>.Tests/
docs/
  <product>/guides/
```

---

## 4.2 Writing Handlers

```csharp
public class CreateCustomerHandler : IRequestHandler<CreateCustomer, CustomerDto>
{
    public Task<CustomerDto> Handle(CreateCustomer request, CancellationToken ct)
    {
        ...
    }
}
```

Frank handles:

- routing  
- validation  
- error handling  
- result mapping  
- observability context propagation (NEW)  

You focus on business logic.

---

## 4.3 Working With DI

```csharp
[RegisterScoped]
public class CustomerService : ICustomerService { }
```

Manual registration:

```csharp
services.AddSingleton<IMyService, MyService>();
```

---

## 4.4 Working With Configuration

```csharp
var value = config["MySection:MyKey"];
```

Frank ensures deterministic layering.

---

## 4.5 Working With Environment

```csharp
env.Get("MY_SETTING");
```

Frank ensures:

- no direct environment variable access  
- deterministic environment resolution  
- testability  

---

## 4.6 Working With Observability (NEW)

```csharp
events.Info("orders.create.started", new { OrderId = id });
metrics.Increment("orders.create.count");
```

You **never**:

- create correlation IDs manually  
- log secrets, tokens, or PII  
- use Stopwatch or real‑time timers  
- use vendor‑specific logging/metrics APIs  

---

# 5. Testing With Frank

Frank.Testing provides:

- deterministic test hosting  
- environment mutation  
- configuration mutation  
- DI mutation  
- hosting provider mutation  
- startup module mutation  
- observability test sinks (NEW)  

```csharp
var factory = new MutatedWebApplicationFactory();
var client = factory.CreateClient();
```

---

# 6. Using Frank Capabilities

Each capability has its own user guide:

```
docs/frank/guides/user/<capability>/README.md
```

Examples:

- `testing/`  
- `hosting/`  
- `environment/`  
- `configuration/`  
- `validation/`  
- `dispatching/`  
- `observability/`  

---

# 7. What Frank Users Should *Not* Do

You should **not**:

- bypass the environment abstraction  
- bypass DI auto‑registration  
- bypass the dispatcher pipeline  
- write your own startup logic  
- write your own hosting provider  
- write your own exception middleware  
- access environment variables directly  
- rely on static state  
- modify Frank internals  
- create correlation IDs manually (NEW)  
- use ad‑hoc logging or vendor‑specific metrics (NEW)  

These break determinism and guardrails.

---

# 8. Summary

The Frank User Guide is your handbook for:

- building applications with Frank  
- using Frank’s capabilities  
- writing handlers, validators, and services  
- working with hosting, environment, and configuration  
- writing deterministic tests using Frank.Testing  
- using Frank’s observability primitives correctly (NEW)  
- following Frank’s conventions and guardrails  

Frank provides a governed, deterministic, observable foundation so you can focus on building your product — not building a framework.
