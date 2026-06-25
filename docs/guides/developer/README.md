# Camp Fit Fur Dogs — Guides — Developer
Product‑specific development handbook

Welcome to the **Camp Fit Fur Dogs Developer Guide** — the handbook for developers building and maintaining the Camp Fit Fur Dogs product **using the Frank Framework**.

This guide documents **product‑specific architecture, conventions, and workflows**.  
Frank’s capabilities (DI, hosting, startup, validation, observability, testing harness, etc.) are documented separately in the Frank guides.

---

# 1. Purpose of This Guide

This guide provides:

- Product‑specific architecture  
- Product slice structure  
- Product domain model overview  
- Product conventions  
- Product‑specific DI/config rules  
- Product‑specific observability events  
- How to use Frank capabilities inside Camp Fit Fur Dogs  
- Developer workflow for adding new slices/features  

This guide does **not** restate Frank’s capabilities or internal architecture.

---

# 2. Product Architecture

Camp Fit Fur Dogs follows a **clean, vertical‑slice architecture** built on Frank.

```
src/
  CampFitFurDogs.Api/
  CampFitFurDogs.Application/
  CampFitFurDogs.Domain/
  CampFitFurDogs.Infrastructure/
```

## 2.1 Bounded Contexts

Camp Fit Fur Dogs is organized into the following bounded contexts:

- Authentication  
- Owners  
- Bookings  
- Dogs  
- Staff  
- Operations  

Each context contains:

- Domain entities  
- Application commands/queries  
- Validators  
- Repositories  
- API endpoints  

## 2.2 Aggregates

Key aggregates include:

- Owner  
- Dog  
- Booking  
- StaffMember  

Aggregates enforce product‑specific invariants.

## 2.3 Domain Model Rules

- All domain invariants live in the Domain layer  
- All mutations occur through aggregate root methods  
- Domain events are allowed but remain product‑specific  
- No Frank types appear in the Domain layer  

---

# 3. Product Slice Structure

Each feature slice follows this structure:

```
<Feature>/
  Domain/
    Entities/
    Events/
    ValueObjects/
    Rules/
  Application/
    Commands/
    Queries/
    Validators/
    Abstractions/
  Infrastructure/
    Repositories/
    Mappers/
  Api/
    Endpoints/
    Requests/
    Responses/
```

Slices must be vertical, isolated, self‑contained, and testable.  
Slices must not depend on each other unless explicitly required by the domain.

---

# 4. Product Conventions

## 4.1 Naming Conventions

- Commands end with `Command`  
- Queries end with `Query`  
- Validators end with `Validator`  
- Repositories end with `Repository`  
- API request DTOs end with `Request`  
- API response DTOs end with `Response`  

## 4.2 DTO Conventions

- DTOs never contain identity fields like `OwnerId` (resolved via `ICurrentUser`)  
- DTOs never contain domain invariants  
- DTOs never contain domain events  

## 4.3 Endpoint Conventions

- Endpoints live in `Api/<Feature>/`  
- Endpoints use Frank’s dispatcher  
- Endpoints never call handlers directly  
- Endpoints never return domain entities  
- Endpoints must follow the **API Endpoint Purity Guide**  

## 4.4 Validation Conventions

- Syntactic validation occurs in API  
- Business validation occurs in Application  
- Invariants live in Domain  
- No validation in Infrastructure  
- No validation in endpoints beyond shape validation  

## 4.5 Mapping Conventions

- Request DTO → Command/Query  
- Result → Response DTO  
- No domain entities in responses  
- No domain entities in API layer  
- No business logic in mapping  

---

# 5. Using Frank in Camp Fit Fur Dogs

Frank provides the framework; Camp Fit Fur Dogs provides the product logic.

## 5.1 Dependency Injection

- Use Frank’s `[AutoRegister]` attributes  
- Product‑specific services may be manually registered in `Api/Program.cs`  
- No DI logic in slices  

## 5.2 Configuration

Camp Fit Fur Dogs uses product‑specific configuration keys:

```
CFFD__Database__ConnectionString
CFFD__Auth__Authority
CFFD__Auth__ClientId
CFFD__Auth__Audience
```

All configuration is consumed through Frank’s environment abstractions.

## 5.3 Hosting

Camp Fit Fur Dogs uses Frank’s hosting provider selection.  
Product‑specific hosting metadata lives in:

```
Api/Hosting/
```

## 5.4 Observability (Product‑Specific)

Frank provides observability primitives; Camp Fit Fur Dogs defines product‑specific events.

Examples:

- `cffd.bookings.create.started`  
- `cffd.bookings.create.completed`  
- `cffd.owners.register.started`  
- `cffd.owners.register.failed`  

Rules:

- No secrets, tokens, or PII in event payloads  
- Use Frank’s `ITraceEvents` and `IMetrics`  
- Never create correlation IDs manually  

---

# 6. Developer Workflow

## 6.1 Adding a New Feature Slice

1. Create slice folder under each layer  
2. Define domain entities and invariants  
3. Add commands/queries  
4. Add validators  
5. Add repository interfaces  
6. Implement repositories in Infrastructure  
7. Add API endpoints  
8. Add product‑specific observability events  
9. Add tests  
10. Update documentation if needed  

## 6.2 Adding a New Endpoint

1. Create request/response DTOs  
2. Create command/query  
3. Create handler  
4. Create validator  
5. Add endpoint using Frank’s dispatcher  
6. Emit product‑specific observability events  
7. Add tests  
8. Follow the **API Endpoint Purity Guide**  

## 6.3 Adding a New Domain Rule

1. Add rule to Domain  
2. Add tests  
3. Update validators if needed  
4. Update handlers if needed  

---

# 7. What Camp Fit Fur Dogs Developers Should Not Do

- Do not bypass Frank’s environment abstraction  
- Do not bypass DI auto‑registration  
- Do not bypass the dispatcher pipeline  
- Do not write custom startup logic  
- Do not write custom hosting providers  
- Do not access environment variables directly  
- Do not use static state  
- Do not modify Frank internals  
- Do not create correlation IDs manually  
- Do not emit ad‑hoc logs or vendor‑specific metrics  

---

# 8. Summary

The Camp Fit Fur Dogs Developer Guide explains:

- How to build product features  
- How to structure slices  
- How to use Frank capabilities correctly  
- How to follow product‑specific conventions  
- How to emit product‑specific observability events  
- How to maintain product architecture  

Frank provides the deterministic foundation.  
Camp Fit Fur Dogs provides the product logic built on top of it.
