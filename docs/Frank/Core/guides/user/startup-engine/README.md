
# Frank — Guides — User — Startup Engine Guide  
*How to use the Startup Engine in Frank.*

The Startup Engine allows your application to start up in a **modular, predictable, and organized** way.  
Instead of putting all startup logic in one place, the Startup Engine lets you break it into small units called **startup modules**.

Each module can:

- register services during the *Add phase*  
- configure middleware and endpoints during the *Use phase*  
- run in a specific order  

This guide explains how to use the Startup Engine in your application.

---

## 1. What the Startup Engine Does

The Startup Engine:

- discovers all registered startup modules  
- sorts them by their declared order  
- runs all **Add()** methods before the app is built  
- runs all **Use()** methods after the app is built  

This gives you:

- clean separation of responsibilities  
- predictable startup behavior  
- easier maintenance  
- easier testing  
- easier onboarding for new developers  

---

## 2. How to Enable the Startup Engine

Add the engine to your service collection:

```csharp
services.AddStartupEngine();
```

This registers the engine as a singleton.

You must also register your startup modules — typically via DI:

```csharp
services.AddScoped<IStartupModule, MyModule>();
services.AddScoped<IStartupModule, LoggingModule>();
services.AddScoped<IStartupModule, EndpointModule>();
```

The Startup Engine does **not** auto‑discover modules.  
You explicitly register them like any other service.

---

## 3. How Startup Modules Work

A startup module looks like this:

```csharp
public sealed class MyModule : IStartupModule
{
    public void Add(WebApplicationBuilder builder)
    {
        // register services here
    }

    public void Use(WebApplication app)
    {
        // configure middleware here
    }
}
```

### Add Phase

Runs **before** the DI container is built.

Use this phase for:

- registering services  
- adding configuration  
- adding logging  
- adding options  
- adding authentication/authorization schemes  

**Important:**  
You cannot resolve services during this phase.  
Only registration is allowed.

### Use Phase

Runs **after** the DI container is built.

Use this phase for:

- adding middleware  
- mapping endpoints  
- resolving services  
- configuring runtime behavior  

---

## 4. Controlling Module Order

You can control the order modules run in by adding an attribute:

```csharp
[StartupModule(100)]
public sealed class MyModule : IStartupModule { ... }
```

Rules:

- lower numbers run first  
- modules without an attribute default to `1000`  
- ordering applies to both Add and Use phases  

This ensures deterministic startup behavior.

---

## 5. How to Run All Modules

In your Program.cs:

```csharp
var builder = WebApplication.CreateBuilder(args);

// Resolve the engine
var startup = builder.Services
    .BuildServiceProvider()
    .GetRequiredService<StartupEngine>();

// Run Add phase
startup.AddAll(builder);

// Build the app
var app = builder.Build();

// Run Use phase
startup.UseAll(app);

app.Run();
```

This ensures:

- all Add() methods run before the app is built  
- all Use() methods run after the app is built  

---

## 6. Best Practices

- Keep modules small and focused  
- Use ordering when modules depend on each other  
- Put service registration in Add()  
- Put middleware and endpoints in Use()  
- Avoid resolving services in Add()  
- Avoid global/static state  
- Keep module responsibilities clear and isolated  

---

## 7. Common Mistakes to Avoid

- **Resolving services in Add()**  
  The DI container does not exist yet.

- **Calling BuildServiceProvider() in Add()**  
  This creates a second container and breaks the app.

- **Adding middleware in Add()**  
  Middleware belongs in Use().

- **Assuming modules run in DI registration order**  
  They run in attribute order.

- **Putting runtime logic in Add()**  
  Add() is strictly for registration.

---

## 8. Summary

The Startup Engine gives you:

- modular startup  
- predictable ordering  
- clean separation between service registration and middleware configuration  
- a scalable way to organize application startup  

As a user:

- you register the engine  
- you register your modules  
- you optionally assign an order  
- the engine runs everything for you  

This makes your application startup easier to understand, easier to maintain, and easier to extend.

