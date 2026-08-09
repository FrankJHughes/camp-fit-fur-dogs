# Hosting

The **Hosting** folder contains abstractions that define how the application adapts to the environment in which it is hosted. Hosting modules allow the application to modify configuration, enable or disable features, and perform environment‑specific initialization before the API is fully built.

This layer provides a structured, extensible mechanism for customizing startup behavior across different deployment environments (local development, staging, production, containers, cloud platforms, etc.).

---

## Purpose

Hosting modules enable the application to:

- **adapt to its environment** — load environment‑specific configuration or behavior  
- **modify configuration early** — before the host is built  
- **enable/disable features** — based on environment, flags, or external conditions  
- **bootstrap infrastructure** — perform startup tasks required by the hosting platform  
- **remain modular** — each module encapsulates a single hosting concern  

This keeps environment‑specific logic out of the core application and ensures clean separation between hosting concerns and business logic.

---

## Components

### HostingModuleAttribute
Marks a class as a hosting module and defines its execution order.

```csharp
[AttributeUsage(AttributeTargets.Class)]
public sealed class HostingModuleAttribute : Attribute
{
    public int Order { get; }
    public HostingModuleAttribute(int order) => Order = order;
}
```

Modules with lower `Order` values run earlier in the hosting pipeline.

---

### IHostingModule
Defines how a hosting module participates in startup.

```csharp
public interface IHostingModule
{
    bool IsActive(WebApplicationBuilder builder);
    Task<IDictionary<string, string?>> GetConfigurationOverridesAsync(WebApplicationBuilder builder);
}
```

Each module:

- decides whether it should be active  
- provides configuration overrides  
- adapts the application to the hosting environment  

Modules can inspect:

- environment variables  
- configuration sources  
- hosting environment name  
- deployment platform  
- feature flags  

---

## Design Principles

- **Environment‑driven behavior**  
  Hosting modules adapt the application based on where it runs.

- **Early configuration overrides**  
  Modules can modify configuration before the host is built.

- **Ordered execution**  
  Modules run in a deterministic sequence using `HostingModuleAttribute`.

- **Modularity**  
  Each module encapsulates a single hosting concern.

- **Separation of concerns**  
  Hosting logic stays out of business logic and application services.

- **Extensibility**  
  New hosting modules can be added without modifying existing ones.

---

## How Hosting Fits Into the Application

Hosting modules typically handle:

- environment‑specific configuration  
- cloud platform integration  
- container‑specific behavior  
- local development overrides  
- feature toggles  
- infrastructure bootstrapping  
- secrets loading  
- environment validation  

They run **before** the application host is built, ensuring that all configuration and environment‑specific behavior is applied consistently and predictably.

---
