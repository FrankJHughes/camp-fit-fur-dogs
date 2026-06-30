# ADR‑0063 — Endpoint Registration Engine Architecture

## Status  
Accepted  
Supersedes: ADR‑0020 (Endpoint Auto‑Discovery)

## Context  
The original endpoint discovery model (ADR‑0020) relied on:

- suffix‑based scanning (`*Endpoint`)
- Scrutor‑based DI registration
- implicit discovery rules
- implicit mapping conventions
- implicit purity rules

As the platform evolved, several architectural changes made ADR‑0020 insufficient:

- Introduction of **Registration Engine** (ADR‑0061)
- Introduction of **StartupEngine** (ADR‑0062)
- Strengthened **vertical slice isolation** (ADR‑0052)
- Strengthened **reader/repository separation** (ADR‑0048)
- Strengthened **DTO architecture** (ADR‑0050)
- Strengthened **identity model** (ADR‑0059)
- Strengthened **error boundary architecture** (ADR‑0047)
- Strengthened **purity rules v3** (Architecture.Tests)
- Strengthened **endpoint guardrails** (Api.Tests/Guardrails)

Endpoints are now a **first‑class architectural concept**, not a suffix‑based convention.

A new **Endpoint Engine** was required.

---

## Decision  
We introduce a **Frank‑level Endpoint Engine** that governs:

- endpoint discovery  
- endpoint registration  
- endpoint purity  
- endpoint mapping  
- endpoint DI rules  
- endpoint test rules  

This engine replaces the old suffix‑based auto‑discovery model.

### 1. IEndpoint — canonical endpoint contract

````csharp
public interface IEndpoint
{
    void Map(IEndpointRouteBuilder builder);
}
````

Rules:

- Endpoints must implement `IEndpoint`.
- Endpoints must be **stateless**.
- Endpoints must not contain business logic.
- Endpoints must not reference domain entities.
- Endpoints must not bypass the dispatcher pipeline.
- Endpoints must not perform DI registration.

### 2. Endpoint Engine — discovery + registration

The Endpoint Engine:

- discovers endpoint types using **Registration Engine** rules  
- validates endpoint purity  
- registers endpoints deterministically  
- maps endpoints during StartupEngine.UseAll  

````csharp
public sealed class EndpointEngine
{
    private readonly IReadOnlyList<IEndpoint> _endpoints;

    public EndpointEngine(IReadOnlyList<IEndpoint> endpoints)
    {
        _endpoints = endpoints;
    }

    public void MapAll(IEndpointRouteBuilder builder)
    {
        foreach (var endpoint in _endpoints)
            endpoint.Map(builder);
    }
}
````

### 3. Discovery via Registration Engine

Endpoints are discovered using governed rules:

- include all types implementing `IEndpoint`
- exclude abstract types
- exclude generic types
- enforce purity rules
- enforce DI rules

No Scrutor.  
No suffix scanning.  
No implicit discovery.

### 4. StartupEngine Integration

StartupEngine wires endpoints during the **Use** phase:

````csharp
public sealed class ApiStartupModule : IStartupModule
{
    public void Add(WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointEngine();
    }

    public void Use(WebApplication app)
    {
        var engine = app.Services.GetRequiredService<EndpointEngine>();
        engine.MapAll(app);
    }
}
````

### 5. Purity Rules

Endpoints must:

- call the dispatcher pipeline  
- use DTOs (ADR‑0050)  
- not reference domain entities  
- not reference repositories  
- not reference readers directly  
- not reference infrastructure  
- not reference other endpoints  
- not contain business logic  
- not contain validation logic  
- not contain identity resolution logic  
- not contain environment logic  

Endpoints are **thin** and **pure**.

### 6. Guardrails

Api.Tests/Guardrails enforce:

- every `*Endpoint` implements `IEndpoint`
- no endpoint bypasses dispatcher pipeline
- no endpoint references domain entities
- no endpoint references repositories
- no endpoint references readers
- no endpoint references infrastructure
- no endpoint performs DI registration
- no endpoint contains business logic
- endpoint mapping smoke tests

Architecture.Tests enforce:

- endpoint purity via reflection  
- layering rules  
- DTO purity  
- dispatcher usage  

### 7. Test Harness Integration

Endpoint tests use:

- ApiFactory  
- ApiContext  
- full request → response flow  
- error boundary  
- identity model  
- observation model  

Endpoints are tested as **black‑box HTTP units**.

---

## Consequences  

### Positive  
- Deterministic endpoint discovery  
- Strong purity enforcement  
- Clean separation of concerns  
- Consistent endpoint mapping  
- No suffix‑based scanning  
- No Scrutor  
- No implicit DI registration  
- Clean integration with StartupEngine  
- Clean integration with Registration Engine  
- Strong guardrail coverage  
- Predictable endpoint behavior  
- Easier contributor onboarding  

### Negative  
- Contributors must follow endpoint purity rules  
- Endpoint creation requires implementing `IEndpoint`  
- Migration from ADR‑0020 required updating all endpoints  

### Neutral  
- Endpoint Engine does not dictate routing style  
- Endpoint Engine does not dictate DTO shape  
- Endpoint Engine does not dictate dispatcher behavior  

---

## Summary  
The Endpoint Engine replaces the old suffix‑based auto‑discovery model with a governed, deterministic, purity‑enforced architecture that:

- discovers endpoints via Registration Engine  
- maps endpoints via StartupEngine  
- enforces purity via guardrails  
- integrates with dispatcher pipeline  
- integrates with identity model  
- integrates with observation model  
- integrates with error boundary  

This ADR supersedes ADR‑0020 and formalizes the modern endpoint architecture used across CampFitFurDogs.

