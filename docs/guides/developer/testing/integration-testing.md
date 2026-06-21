# Integration Testing Guide  
Validated Infrastructure • Real Database • Real API • Real DI

Integration tests verify that the system behaves correctly under real conditions:
real API host, real middleware pipeline, real DI container, real EF Core migrations,
and a real PostgreSQL database (local Testcontainers or Neon branch in CI).

Integration tests form a critical part of the system’s operational safety net and
are required for all pull requests targeting `main`.

---

## Purpose of Integration Tests

Integration tests ensure that:

- the API behaves correctly end‑to‑end  
- EF Core migrations apply cleanly against a real PostgreSQL instance  
- repositories and readers interact with the database correctly  
- identity resolution and authentication flows behave as expected  
- vertical slices (customers, dogs, authentication‑dependent flows) work under real conditions  
- the DI container and middleware pipeline are wired correctly  

Integration tests complement, but do not replace:

- Test Architecture Guide  
- Authentication Testing Guide  
- Frontend Testing Guide  
- Guardrail tests  
- Unit tests  

Integration tests validate **system behavior**, not internal implementation details.

---

## Test Harness (Current Model)

Integration tests use the unified test harness:

- **ApiContext** — configures database, auth mode, and environment  
- **ApiFactory** — builds the real API host with real DI + real middleware  
- **ApiClientContext** — configures the test client (auth, headers, cookies)  
- **PostgresFixture** — provides a fresh PostgreSQL container per test class  

This harness ensures:

- no mocks  
- no in‑memory databases  
- no minimal DI containers  
- no bypassed middleware  
- no shortcuts  

Every integration test runs against the **real system**.

---

## Database Strategy

Integration tests always run against a **real PostgreSQL instance**:

- **Locally:** Testcontainers (`PostgresFixture`)  
- **CI:** Neon branch database (created per PR, destroyed after run)  

This ensures:

- migrations are validated  
- schema drift is detected early  
- repository and reader behavior is exercised  
- identity and authentication flows behave correctly  

In‑memory databases are prohibited.

---

## CI Workflow (GitHub Actions)

Every pull request targeting `main` triggers the Integration Tests workflow.

### 1. Create a Neon branch database
- named `pr-<number>`  
- auto‑expires after 14 days  
- connection string converted to Npgsql format  

### 2. Apply EF Core migrations
- uses the same migration pipeline as production  
- fails fast on schema errors  

### 3. Run integration tests
- executes the full integration test suite  
- uses the Neon branch connection string  
- validates API + DB + DI + middleware behavior  

### 4. Delete the Neon branch database
- ensures no leftover resources  
- keeps Neon clean and cost‑efficient  

This guarantees that every PR is validated against a **real database** before merge.

---

## Local Integration Testing

Run integration tests locally using the same script CI uses:

```powershell
.\scripts\integration\Run-IntegrationTests.ps1 -ConnectionString "<your connection string>"
```

Supported local workflows:

- local Postgres container  
- Neon preview branch (download `db-conn.txt` from CI)  
- personal Neon branch for debugging  

Local behavior matches CI behavior.

---

## Responsibilities of Integration Tests

Integration tests must validate:

### API Behavior
- correct status codes  
- correct payloads  
- correct authorization behavior  
- correct interaction with the database  

### Database Behavior
- migrations apply cleanly  
- repositories behave correctly  
- readers behave correctly  
- domain invariants are enforced at persistence boundaries  

### Identity + Authentication
- OIDC callback pipeline  
- external ID → internal ID mapping  
- session creation  
- cookie issuance  

### Vertical Slice Behavior
Each slice must be tested end‑to‑end:

- customer creation  
- dog creation + assignment  
- authentication‑dependent flows  
- any slice that touches persistence  

---

## What Integration Tests Must NOT Do

Integration tests must not:

- use guardrail patterns  
- use minimal DI containers  
- mock HttpContextAccessor  
- mock repositories or readers  
- use in‑memory databases  
- use `/__test__/sign-in` unless explicitly testing identity resolution  
- create Testcontainers manually (use `PostgresFixture`)  

Integration tests must use **real infrastructure**, not substitutes.

---

## Example Integration Test (Updated for PostgresFixture)

```csharp
using System.Net.Http.Json;
using CampFitFurDogs.TestUtilities.Contexts;
using CampFitFurDogs.TestUtilities.Factories;
using CampFitFurDogs.TestUtilities.Fixtures;
using FluentAssertions;

namespace CampFitFurDogs.IntegrationTests.Customers;

public class CreateCustomerTests : IClassFixture<PostgresFixture>
{
    private readonly ApiFactory _api;

    public CreateCustomerTests(PostgresFixture fixture)
    {
        var ctx = new ApiContext()
            .WithDatabase(true, fixture.Container)
            .WithCookieAuthOnly(false)
            .WithConfigOverride(cfg =>
                cfg.AddInMemoryCollection(
                    new Dictionary<string, string?>
                    {
                        ["Frontend:BaseUrl"] = "http://localhost:5173"
                    }
                )
            );

        _api = new ApiFactory(ctx);
    }

    [Fact]
    public async Task Should_Create_Customer()
    {
        var client = _api.CreateClient(new ApiClientContext());

        var response = await client.PostAsJsonAsync("/api/customers", new
        {
            firstName = "Frank",
            lastName = "Smith",
            email = "frank@example.com"
        });

        response.EnsureSuccessStatusCode();
    }
}
```

This is the canonical structure for all integration tests.

---

## Branch Protection Requirements

The `main` branch must enforce:

- pull requests  
- passing Integration Tests workflow  
- no force pushes  
- no direct commits  

This ensures:

- schema changes cannot break production  
- repository and reader behavior is always validated  
- vertical slices remain correct end‑to‑end  
- `main` remains stable and deployable  

Integration tests are a **merge gate**, not an optional check.

---

## Summary

Integration tests ensure that:

- EF Core migrations are valid  
- the API works against a real database  
- vertical slices behave correctly end‑to‑end  
- identity resolution works under real conditions  
- CI enforces correctness before merge  
- `main` remains stable, predictable, and deployable  

They are a foundational part of the system’s operational safety model.

---

## Related Documentation

- API Hosting Guide  
- Database Hosting Guide  
- Authentication Testing Guide  
- Test Architecture Guide  
- Preview Troubleshooting Guide  
