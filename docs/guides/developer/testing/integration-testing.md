# Integration Testing Guide

Integration tests validate **real API + real database behavior** using a temporary Neon branch or a local Postgres container.  
They ensure that every pull request targeting `main` is fully exercised end‑to‑end before merge.

Integration tests are part of the system’s **operational safety net**, complementing unit tests, guardrails, and preview‑environment tests.

This guide has been fully aligned with the **new test harness**, **new DI patterns**, **new identity model**, and **recent governance + conventions updates**.

---

# What Integration Tests Cover

Integration tests validate:

- API + database behavior  
- EF Core migrations against a real PostgreSQL instance  
- End‑to‑end flows for key vertical slices (customers, dogs, authentication‑dependent flows)  
- Repository + reader behavior  
- Real DI container wiring  
- Real middleware pipeline  
- Identity resolution using the real OIDC callback pipeline (when applicable)  

Integration tests do **not** replace:

- [Test Architecture](ca://s?q=Show_test_architecture_guide)  
- [Authentication Testing](ca://s?q=Show_authentication_testing_guide)  
- [Frontend Testing](ca://s?q=Show_frontend_testing_guide)

Integration tests focus on **backend correctness under real conditions**, not guardrail enforcement or unit‑level behavior.

---

# Test Harness (Updated)

Integration tests now use:

- **ApiContext** — configures database, auth mode, and environment  
- **ApiFactory** — builds the real API host  
- **ApiClientContext** — configures the test client  
- **PostgreSqlContainer** (local) or **Neon branch** (CI)  

Legacy base classes (e.g., `ApiWithPostgresTestBase`) are no longer used.

Integration tests must:

- Use **real DI**  
- Use **real middleware**  
- Use **real repositories/readers**  
- Use **real EF Core migrations**  
- Use **real Postgres** (local or Neon)  

---

# CI Behavior (GitHub Actions)

On every pull request targeting `main`, the CI workflow performs the following sequence:

## 1. Create a Neon branch database
- Named `pr-<number>`  
- Expires automatically after 14 days  
- Created via Neon API  
- Connection string converted from PostgreSQL URI → Npgsql format  

## 2. Apply EF Core migrations
- Uses the same migration pipeline as production  
- Ensures schema correctness before tests run  
- Fails fast if migrations are invalid  

## 3. Run the integration test suite
- Executes `CampFitFurDogs.IntegrationTests`  
- Uses the Neon branch connection string  
- Validates repository, reader, and API behavior  
- Exercises real DI container + middleware  
- Uses the **new ApiContext/ApiFactory harness**  

## 4. Delete the Neon branch database
- Ensures no leftover resources  
- Keeps Neon clean and cost‑efficient  
- Enforced by CI teardown step  

This ensures every PR is validated against a **real database**, not mocks or in‑memory substitutes.

---

# Local Integration Testing

You can run integration tests locally using the same script CI uses:

```
.\scripts\integration\Run-IntegrationTests.ps1 -ConnectionString "<your connection string>"
```

## Local workflow options

- Use a **local Postgres container**  
- Use a **Neon preview branch** (download `db-conn.txt` from CI)  
- Use a **personal Neon branch** for debugging  

Local integration tests should behave identically to CI.

---

# Integration Test Responsibilities (Updated)

Integration tests must validate:

### API Behavior
- Endpoints return correct status codes  
- Endpoints return correct payloads  
- Endpoints enforce authorization correctly  
- Endpoints interact with the database correctly  

### Database Behavior
- EF Core migrations apply cleanly  
- Repositories behave correctly  
- Readers behave correctly  
- Domain invariants are enforced at persistence boundaries  

### Identity + Authentication (when applicable)
- OIDC callback pipeline works end‑to‑end  
- External ID → internal ID mapping works  
- Sessions are created correctly  
- Cookies are issued correctly  

### Vertical Slice Behavior
Each slice must be tested end‑to‑end:

- Customer creation  
- Dog creation + assignment  
- Authentication‑dependent flows  
- Any slice that touches persistence  

---

# What Integration Tests Should NOT Do (Aligned With Governance)

Integration tests must **not**:

- Use guardrail patterns  
- Use minimal DI containers  
- Mock HttpContextAccessor  
- Mock repositories or readers  
- Use `/__test__/sign-in` unless explicitly testing identity resolution  
- Use in‑memory databases  
- Use Testcontainers for guardrails  

Integration tests must use **real infrastructure**, not substitutes.

---

# Example Integration Test Structure (Updated)

```csharp
public class CreateCustomerTests : IAsyncLifetime
{
    private PostgreSqlContainer _postgres = default!;
    private ApiFactory _api = default!;

    public async Task InitializeAsync()
    {
        _postgres = new PostgreSqlBuilder("postgres:16-alpine").Build();
        await _postgres.StartAsync();

        var ctx = new ApiContext()
            .WithDatabase(true, _postgres)
            .WithCookieAuthOnly(false);

        _api = new ApiFactory(ctx);
    }

    public async Task DisposeAsync()
    {
        if (_postgres is not null)
            await _postgres.DisposeAsync();
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

This reflects the **new test harness** and **new DI patterns**.

---

# Branch Protection

The `main` branch must require:

- Pull requests  
- The **Integration Tests** workflow to pass  
- No force pushes  
- No direct commits  

This ensures:

- Schema changes cannot break production  
- Repository and reader behavior is always validated  
- Vertical slices remain end‑to‑end correct  

Integration tests are a **merge gate**, not an optional check.

---

# Summary

Integration tests ensure:

- EF Core migrations are valid  
- The API works against a real database  
- Vertical slices behave correctly end‑to‑end  
- Identity resolution works under real conditions  
- CI enforces correctness before merge  
- `main` remains stable and deployable  

They are a critical part of the system’s operational safety model.

---

# Related Documentation

- [API Hosting](ca://s?q=Show_api_hosting_guide)  
- [Database Hosting](ca://s?q=Show_db_hosting_guide)  
- [Authentication Testing](ca://s?q=Show_authentication_testing_guide)  
- [Test Architecture](ca://s?q=Show_test_architecture_guide)  
- [Preview Troubleshooting](ca://s?q=Show_preview_troubleshooting_guide)
