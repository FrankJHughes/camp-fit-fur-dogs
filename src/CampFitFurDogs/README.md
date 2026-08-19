# CampFitFurDogs.Host

The **CampFitFurDogs.Host** project is the composition root of the Camp Fit Fur Dogs platform.  
It contains the application’s entry point (`Program.cs`) and is responsible for configuring the web host, applying hosting‑environment logic, registering platform services, and activating all API endpoints.

This project orchestrates the entire application; it does not define domain, application, or API behavior.

---

## Purpose

The Host project provides:

- The executable entry point (`Program.cs`)  
- Environment‑specific hosting adaptation  
- WebApplicationBuilder configuration  
- Registration of platform‑level services  
- Activation of all API endpoints  
- Middleware pipeline configuration  
- The final call to `app.Run()`  

All API behavior is delegated to `CampFitFurDogs.Api`.  
All business logic is delegated to `CampFitFurDogs.Application` and `CampFitFurDogs.Domain`.

---

## Project Structure

```
CampFitFurDogs.Host
├── Program.cs
├── appsettings.json
├── appsettings.Development.json
├── appsettings.Testing.json
└── Properties
    └── launchSettings.json
```

---

## Responsibilities

### **Startup Orchestration**
The Host project adapts the application to its environment:

```csharp
await Hosting.AdaptToHostingEnvironment(builder);
```

This applies hosting modules defined in `CampFitFurDogs.Api.HostingModules`.

### **Platform Registration**
The Host composes all platform layers:

```csharp
services
    .AddCampFitFurDogsApiPlatform(configuration)
    .AddFrankCoreApiPlatform(configuration)
    .AddFrankIdentityApiPlatform(configuration);
```

### **Endpoint Activation**
Endpoints are discovered and registered:

```csharp
services
    .AddCampFitFurDogsApiEndpoints()
    .AddFrankIdentityApiEndpoints();
```

### **Middleware Pipeline**
The Host applies platform‑level middleware:

```csharp
app
    .UseFrankCoreApiPlatform()
    .UseFrankIdentityApiPlatform();
```

### **API Routing**
All endpoints are grouped under `/api`:

```csharp
app.MapRegisteredApiEndpoints("/api");
```

---

## Design Principles

- **Single responsibility** — the Host project only orchestrates startup  
- **Separation of concerns** — hosting logic is isolated from API logic  
- **Composability** — platform layers are composed, not hard‑coded  
- **Environment awareness** — hosting modules adapt configuration dynamically  
- **Predictability** — startup is centralized and consistent

---

## What Does *Not* Belong Here

Do **not** add:

- Endpoints  
- DTOs  
- Validators  
- Exception handlers  
- Domain logic  
- Application logic  
- Infrastructure logic  
- Hosting modules (they live in the Api assembly)

---

## Summary

The **CampFitFurDogs.Host** project is the executable entry point of the platform.  
It configures the host, applies environment‑specific behavior, registers platform services, activates endpoints, and runs the application.

It is the top of the dependency chain:

**Host → Api → Application → Domain → Infrastructure**
