# Folder Structure & Slice Anatomy  
*A developer‑facing guide to how the solution is structured and how vertical slices cut through it.*

This guide describes the high‑level folder structure of the Camp Fit Fur Dogs solution and the anatomy of a vertical slice.  
It ensures contributors understand:

- Where code belongs  
- Why it belongs there  
- How layers interact  
- How vertical slices span the system  

This is an **architecture guide**. It explains the model; governance documents define enforcement.

---

## 1. Top‑Level Solution Structure

```text
.devcontainer/
.github/
.vscode/
docs/
frontend/
hooks/
integration-tests/
portfolio/
product/
scripts/
src/
tests/
```

High‑level intent:

- `src/` — backend products (CampFitFurDogs, Frank)  
- `frontend/` — Next.js frontend  
- `tests/` — unit + architecture tests  
- `integration-tests/` — cross‑boundary integration tests  
- `docs/` — ADRs, governance, developer guides  
- `product/` — stories, milestones, guarantees  
- `scripts/` — automation and tooling  

---

## 2. Backend Product Structure

### 2.1 CampFitFurDogs Projects

```text
src/
  CampFitFurDogs.Api/
  CampFitFurDogs.Application/
  CampFitFurDogs.Domain/
  CampFitFurDogs.Infrastructure/
```

**CampFitFurDogs.Api**  
HTTP endpoints, routing, request/response DTOs, middleware wiring.

**CampFitFurDogs.Application**  
Use cases, handlers, validators, dispatchers, domain event dispatch.

**CampFitFurDogs.Domain**  
Entities, value objects, domain events, invariants.

**CampFitFurDogs.Infrastructure**  
Persistence, external systems, repositories, readers, EF Core configuration.

---

### 2.2 Frank (Reusable Product)

Frank is a **separate product**, not a layer.  
It provides reusable architectural primitives and eliminates boilerplate.

```text
src/
  Frank/
  Frank.Api/
  Frank.Infrastructure/
  Frank.Infrastructure.EntityFrameworkCore/
  Frank.Testing/
```

Key Frank areas:

```text
Frank/
  Abstractions/
  Authentication/
  AutoRegistration/
  DependencyInjection/
  Domain/
  Events/
  ImmutableContext/
  Modules/
  Settings/
```

```text
Frank.Api/
  ExceptionHandling/
  Hosting/
  SecurityHeaders/
  Startup/
```

```text
Frank.Infrastructure/
  Environment/
```

```text
Frank.Infrastructure.EntityFrameworkCore/
  Configurations/
```

```text
Frank.Testing/
  Contexts/
  Endpoints/
  Factories/
```

Frank provides:

- DI auto‑registration  
- Endpoint discovery  
- HostingEngine + StartupEngine  
- Validation pipeline  
- Domain event infrastructure  
- Authentication callback pipeline  
- Immutable context builder  
- Testing utilities  

CampFitFurDogs **consumes** Frank.

---

## 3. CampFitFurDogs.Api Structure

```text
CampFitFurDogs.Api/
  Horizontals/
  Verticals/
  Validation/
  Properties/
```

### 3.1 Horizontals

```text
Horizontals/
  Cors/
  ExceptionHandling/
    Handlers/
    Middleware/
  Hosting/
    Modules/
  Startup/
    Modules/
```

Horizontals = cross‑cutting concerns.

### 3.2 Verticals

```text
Verticals/
  Authentication/
    Callback/
    Login/
  Dogs/
```

Verticals = API slices.  
Endpoints map HTTP → Application commands/queries.

---

## 4. CampFitFurDogs.Application Structure

```text
CampFitFurDogs.Application/
  Abstractions/
  Authentication/
  Customers/
  Dogs/
  Errors/
  Exceptions/
  Settings/
```

### 4.1 Abstractions

```text
Abstractions/
  Audit/
  Authentication/
    Callback/
  Customers/
    CreateCustomer/
    FindCustomerByExternalId/
  Dogs/
    EditDogProfile/
    GetDogProfile/
    ListDogsByOwner/
    RegisterDog/
    RemoveDog/
```

Abstractions define:

- Commands  
- Queries  
- Results  
- Reader interfaces  

### 4.2 Feature Folders

```text
Authentication/
  Callback/
    Steps/

Customers/
  CreateCustomer/

Dogs/
  EditDogProfile/
  GetDogProfile/
  ListDogsByOwner/
  RegisterDog/
  RemoveDog/
```

Each use case folder contains:

- Handler  
- Validator  
- Domain event handlers (if any)  

---

## 5. CampFitFurDogs.Domain Structure

```text
CampFitFurDogs.Domain/
  Authentication/
    Sessions/
      Errors/
  Customers/
    Errors/
  Dogs/
  Errors/
```

Domain contains:

- Entities  
- Value objects  
- Domain events  
- Invariants  

Domain has **no dependencies** on Application, Infrastructure, or Api.

---

## 6. CampFitFurDogs.Infrastructure Structure

```text
CampFitFurDogs.Infrastructure/
  Audit/
  Authentication/
    Sessions/
  Customers/
  Data/
  Dogs/
  Identity/
  Migrations/
  Time/
```

Infrastructure:

- Implements repositories  
- Implements readers  
- Integrates with EF Core, identity, time, environment  
- Contains no domain logic  

---

## 7. Tests

### 7.1 Unit + Architecture Tests

```text
tests/
  CampFitFurDogs.Api.Tests/
  CampFitFurDogs.Application.Tests/
  CampFitFurDogs.Architecture.Tests/
  CampFitFurDogs.Domain.Tests/
  CampFitFurDogs.Infrastructure.Tests/
  CampFitFurDogs.Integration.Tests/
  CampFitFurDogs.TestUtilities/
```

### 7.2 Frank Tests

```text
tests/
  Frank.Api.Tests/
  Frank.Infrastructure.Tests/
  Frank.Infrastructure.EntityFrameworkCore.Tests/
  Frank.Tests/
  Frank.TestUtilities/
```

### 7.3 Integration Tests

```text
integration-tests/
  CampFitFurDogs.Api.IntegrationTests/
  CampFitFurDogs.Infrastructure.IntegrationTests/
```

---

## 8. Frontend Structure

```text
frontend/src/
  api/
  app/
  components/
  hooks/
  lib/
  test/
```

### 8.1 Frontend Aggregates

```text
api/
  dogs/
  health/

components/
  dogs/

hooks/
  dogs/

lib/
  api/
  auth/
  components/
  dogs/
  forms/
  hooks/

app/
  (auth)/login/
  dogs/[id]/edit/
  dogs/register/success/
```

### 8.2 Frontend Test Structure

```text
frontend/src/test/
  api/
  app/
  components/
  helpers/
  hooks/
  lib/
  ui/
```

---

## 9. Vertical Slice Anatomy

Example slice: **Dogs**

### Backend

```text
Api/
  Verticals/Dogs/

Application/
  Abstractions/Dogs/
  Dogs/

Domain/
  Dogs/

Infrastructure/
  Dogs/
```

### Frontend

```text
frontend/src/
  api/dogs/
  components/dogs/
  hooks/dogs/
  lib/dogs/
  app/dogs/
```

### Backend Slice Diagram

```text
[Api] → [Application] → [Domain] → [Infrastructure]
```

### Frontend Slice Diagram

```text
[app] → [components] → [hooks] → [api] → [lib]
```

---

## 10. Layer Responsibilities

### Api
- HTTP endpoints  
- Request/response mapping  
- No business logic  
- Must use dispatchers  

### Application
- Use cases  
- Handlers + validators  
- Domain event dispatch  
- No HTTP or persistence logic  

### Domain
- Entities, value objects, domain events  
- Invariants  
- No dependencies on other layers  

### Infrastructure
- Repositories, readers  
- EF Core, identity, time  
- No domain logic  

### Frank
- Reusable product  
- Provides architectural primitives  
- Consumed by CampFitFurDogs  

---

## 11. Contributor Decision Tree

1. **Is it reusable across products?**  
   → Frank

2. **Is it a business rule or invariant?**  
   → Domain

3. **Is it a use case?**  
   → Application

4. **Is it persistence or external integration?**  
   → Infrastructure

5. **Is it HTTP routing or endpoint mapping?**  
   → Api

6. **Is it frontend routing?**  
   → `app/`

7. **Is it a UI component?**  
   → `components/`

8. **Is it a frontend API call?**  
   → `api/`

9. **Is it shared frontend logic?**  
   → `lib/` or `hooks/`

---

## 12. Anti‑Patterns

- Business logic in Api  
- Domain logic in Infrastructure  
- Using Frank types in Domain  
- Treating Frank as a layer  
- Mixing aggregates  
- Adding product‑specific logic to Frank  

---

## 13. Summary

- CampFitFurDogs is structured by **layers** and **vertical slices**.  
- Frank is a **separate product** providing reusable architectural primitives.  
- The frontend mirrors backend aggregates while staying idiomatic to Next.js.  
- Purity rules keep each layer focused and maintainable.  
- Tests mirror the product and layer structure.

This guide is the **map**; governance defines the **rules**.
