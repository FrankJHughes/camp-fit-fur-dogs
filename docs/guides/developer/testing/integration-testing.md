# Integration Testing Guide  
**Validated Infrastructure • Real Database • Real API • Real DI**

Integration tests verify that the system behaves correctly under **real conditions**: real API host, real middleware pipeline, real DI container, real EF Core migrations, and a real PostgreSQL database (local Testcontainers or Neon branch in CI).  
They form a critical part of the system’s operational safety net and are required for all pull requests targeting `main`.

---

# Purpose of Integration Tests

Integration tests ensure that:

- The API behaves correctly end‑to‑end  
- EF Core migrations apply cleanly against a real PostgreSQL instance  
- Repositories and readers interact with the database correctly  
- Identity resolution and authentication flows behave as expected  
- Vertical slices (customers, dogs, authentication‑dependent flows) work under real conditions  
- The DI container and middleware pipeline are wired correctly  

Integration tests complement, but do not replace:

- [Test Architecture](ca://s?q=Show_test_architecture_guide)  
- [Authentication Testing](ca://s?q=Show_authentication_testing_guide)  
- [Frontend Testing](ca://s?q=Show_frontend_testing_guide)  
- Guardrail tests  
- Unit tests  

Integration tests validate **system behavior**, not internal implementation details.

---

# Test Harness (Current Model)

Integration tests use the unified test harness:

- **ApiContext** — configures database, auth mode, and environment  
- **ApiFactory** — builds the real API host with real DI + real middleware  
- **ApiClientContext** — configures the test client (auth, headers, cookies)  
- **PostgresFixture** — provides a fresh PostgreSQL container per test class  

This harness ensures:

- No mocks  
- No in‑memory databases  
- No minimal DI containers  
- No bypassed middleware  
- No shortcuts  

Every integration test runs against the **real system**.

---

# Database Strategy

Integration tests always run against a **real PostgreSQL instance**:

- **Locally:** Testcontainers (`PostgresFixture`)  
- **CI:** Neon branch database (created per PR, destroyed after run)  

This ensures:

- Migrations are validated  
- Schema drift is detected early  
- Repository and reader behavior is exercised  
- Identity and authorization flows behave correctly  

In‑memory databases are prohibited.

---

# CI Workflow (GitHub Actions)

Every pull request targeting `main` triggers the Integration Tests workflow.

## 1. Create a Neon branch database
- Named `pr-<number>`  
- Auto‑expires after 14 days  
- Connection string converted to Npgsql format  

## 2. Apply EF Core migrations
- Uses the same migration pipeline as production  
- Fails fast on schema errors  

## 3. Run integration tests
- Executes the full integration test suite  
- Uses the Neon branch connection string  
- Validates API + DB + DI + middleware behavior  

## 4. Delete the Neon branch database
- Ensures no leftover resources  
- Keeps Neon clean and cost‑efficient  

This guarantees that every PR is validated against a **real database** before merge.

---

# Local Integration Testing

Run integration tests locally using the same script CI uses:

```powershell
.\scripts\integration\Run-IntegrationTests.ps1 -ConnectionString "<your connection string>"
```

Supported local workflows:

- Local Postgres container  
- Neon preview branch (download `db-conn.txt` from CI)  
- Personal Neon branch for debugging  

Local behavior matches CI behavior.

---

# Responsibilities of Integration Tests

Integration tests must validate:

### API Behavior
- Correct status codes  
- Correct payloads  
- Correct authorization behavior  
- Correct interaction with the database  

### Database Behavior
- Migrations apply cleanly  
- Repositories behave correctly  
- Readers behave correctly  
- Domain invariants are enforced at persistence boundaries  

### Identity + Authentication
- OIDC callback pipeline  
- External ID → internal ID mapping  
- Session creation  
- Cookie issuance  

### Vertical Slice Behavior
Each slice must be tested end‑to‑end:

- Customer creation  
- Dog creation + assignment  
- Authentication‑dependent flows  
- Any slice that touches persistence  

---

# What Integration Tests Must NOT Do

Integration tests must **not**:

- Use guardrail patterns  
- Use minimal DI containers  
- Mock HttpContextAccessor  
- Mock repositories or readers  
- Use in‑memory databases  
- Use `/__test__/sign-in` unless explicitly testing identity resolution  
- Create Testcontainers manually (use `PostgresFixture`)  

Integration tests must use **real infrastructure**, not substitutes.

---

# Example Integration Test (Updated for PostgresFixture)

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
;

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

# Branch Protection Requirements

The `main` branch must enforce:

- Pull requests  
- Passing Integration Tests workflow  
- No force pushes  
- No direct commits  

This ensures:

- Schema changes cannot break production  
- Repository and reader behavior is always validated  
- Vertical slices remain correct end‑to‑end  
- `main` remains stable and deployable  

Integration tests are a **merge gate**, not an optional check.

---

# Summary

Integration tests ensure that:

- EF Core migrations are valid  
- The API works against a real database  
- Vertical slices behave correctly end‑to‑end  
- Identity resolution works under real conditions  
- CI enforces correctness before merge  
- `main` remains stable, predictable, and deployable  

They are a foundational part of the system’s operational safety model.

---

# Related Documentation

- [API Hosting](ca://s?q=Show_api_hosting_guide)  
- [Database Hosting](ca://s?q=Show_db_hosting_guide)  
- [Authentication Testing](ca://s?q=Show_authentication_testing_guide)  
- [Test Architecture](ca://s?q=Show_test_architecture_guide)  
- [Preview Troubleshooting](ca://s?q=Show_preview_troubleshooting_guide)
