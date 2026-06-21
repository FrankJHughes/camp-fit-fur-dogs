# Infrastructure Architecture  
*How CampFitFurDogs integrates persistence, external systems, hosting providers, and environment seams using Frank.*

The Infrastructure layer is responsible for **integration**, not business logic.  
It connects the application to the outside world through **repositories**, **readers**, **hosting providers**, **environment seams**, and **external service adapters**.

This guide explains how Infrastructure is structured, how it interacts with Application and Domain, and how Frank enforces purity and safety.

For enforceable rules, see:  
`docs/governance/technical/architecture-governance.md`

---

# 1. Purpose of the Infrastructure Layer

Infrastructure provides:

- Persistence (EF Core)  
- External service integrations  
- Hosting provider implementations  
- Environment abstraction  
- Time abstraction  
- Email delivery (future)  
- Outbox/eventual consistency (future)  

Infrastructure **implements** abstractions defined in Application.  
It does **not** define business rules.

Infrastructure is the **outermost layer** of the system.

---

# 2. Responsibilities of Infrastructure

Infrastructure must:

- Implement repositories  
- Implement readers  
- Implement external service clients  
- Implement hosting providers  
- Implement environment access  
- Implement time providers  
- Configure EF Core  
- Apply migrations (via CI/CD or hosting provider)  
- Integrate with Frank’s DI auto‑registration  
- Integrate with Frank’s hosting provider pipeline  

Infrastructure must **not**:

- Contain business logic  
- Construct domain entities incorrectly  
- Expose infrastructure types to Application or Domain  
- Access environment variables directly  
- Perform HTTP/JSON/ZIP operations directly (hosting providers)  
- Bypass Frank’s guardrails  

---

# 3. Infrastructure Folder Structure

Infrastructure follows a **slice‑aligned** structure:

```
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

Each folder corresponds to a vertical slice or cross‑cutting concern.

## **3.1 Audit**
Implements `IAuditLogger`.

## **3.2 Authentication/Sessions**
Implements session persistence and lookup.

## **3.3 Customers**
Implements customer readers and repositories.

## **3.4 Dogs**
Implements dog readers and repositories.

## **3.5 Identity**
Implements identity mapping and external identity lookups (if needed).

## **3.6 Time**
Implements `ITimeProvider`.

## **3.7 Data**
Contains EF Core DbContext and configuration.

## **3.8 Migrations**
Contains EF Core migrations.

---

# 4. EF Core Integration

EF Core is used for persistence.  
Infrastructure owns:

- DbContext  
- Entity configurations  
- Migrations  
- Database providers  
- Connection strings  
- Transaction boundaries  

Frank provides:

- EF Core configuration scanning  
- Guardrails for missing configurations  
- Hosting provider integration for connection strings  

## **4.1 DbContext**

The DbContext must:

- Contain only persistence logic  
- Not contain domain logic  
- Not contain business rules  
- Not contain validation  
- Not contain mapping logic outside EF Core configuration  

## **4.2 Entity Configuration**

Configuration classes live in:

```
Frank.Infrastructure.EntityFrameworkCore/Configurations/
```

Frank scans these automatically.

## **4.3 Migrations**

Migrations are:

- Generated locally  
- Applied via CI/CD or hosting provider  
- Never edited manually  
- Never applied directly in production by developers  

---

# 5. Repositories & Readers

Application defines abstractions:

```
I<Aggregate>Repository
I<Aggregate>Reader
```

Infrastructure implements them.

## **5.1 Repositories**
Repositories:

- Persist aggregates  
- Load aggregates  
- Use EF Core  
- Respect aggregate boundaries  
- Never return EF entities directly  
- Never expose DbContext  

Repositories must:

- Rehydrate aggregates correctly  
- Persist domain events (future outbox)  
- Respect invariants enforced by Domain  

## **5.2 Readers**
Readers:

- Return DTOs  
- Use LINQ projections  
- Never return domain entities  
- Never mutate state  
- Never perform business logic  

Readers are optimized for queries.

---

# 6. Hosting Providers

Hosting providers are implemented in Infrastructure but orchestrated by Frank.

Hosting providers:

- Run before DI is built  
- Validate environment configuration  
- Provide dynamic configuration (PR previews)  
- Provide connection strings  
- Provide frontend URLs  
- Provide hosting metadata  

Hosting providers must:

- Use Frank’s abstractions  
- Never read environment variables directly  
- Never perform HTTP/JSON/ZIP directly  
- Fail fast on misconfiguration  

Hosting providers include:

- Render API hosting provider  
- Render PR preview provider  
- Vercel frontend provider (future)  

---

# 7. Environment Abstraction

Infrastructure must use:

```
IEnvironment
```

provided by Frank.

This ensures:

- Safe environment variable access  
- Deterministic behavior  
- Testability  
- No direct `Environment.GetEnvironmentVariable` calls  

Environment access is:

- Validated at startup  
- Logged via hosting provider  
- Required for hosting provider selection  

---

# 8. Time Abstraction

Infrastructure implements:

```
ITimeProvider
```

This ensures:

- Deterministic tests  
- No direct `DateTime.UtcNow` usage  
- Consistent time behavior across environments  

---

# 9. External Service Integrations

External services (future):

- Email delivery  
- SMS delivery  
- Payment providers  
- Notification providers  

Integrations must:

- Use Application abstractions  
- Be implemented in Infrastructure  
- Be testable via fakes  
- Not leak external types upward  

---

# 10. Infrastructure Testing

Infrastructure is tested through:

## **10.1 Integration Tests**
Validate:

- EF Core configuration  
- Migrations  
- Repository behavior  
- Reader behavior  
- Hosting provider behavior  
- Environment seams  

## **10.2 Test Harness**
Frank provides:

- Fake environment  
- Fake hosting provider  
- Fake time provider  
- Fake external services  

Infrastructure tests must not:

- Hit real external services  
- Hit real databases (except ephemeral test DBs)  

---

# 11. Common Failure Modes

Infrastructure failures typically appear as:

- Incorrect EF Core configuration  
- Missing migrations  
- Incorrect aggregate rehydration  
- Incorrect projection logic  
- Hosting provider misconfiguration  
- Environment variable drift  
- Leaking infrastructure types into Application  

Frank’s guardrails prevent most of these.

---

# 12. Summary

Infrastructure is the integration layer of the system.  
It implements persistence, hosting, environment seams, and external service adapters — but never business logic.

Frank provides:

- EF Core scanning  
- Hosting provider pipeline  
- Environment abstraction  
- DI auto‑registration  
- Guardrails  

CampFitFurDogs Infrastructure provides:

- Repositories  
- Readers  
- Hosting provider implementations  
- Time provider  
- Audit logging  
- Session persistence  

Infrastructure must remain:

- Pure  
- Deterministic  
- Testable  
- Slice‑aligned  
- Free of business logic  

For enforceable rules, see:

```
docs/governance/technical/architecture-governance.md
```
