# Test Harness Overview

A unified guide to the **Camp Fit Fur Dogs test harness** across all layers:

- API  
- Application  
- Infrastructure  
- Frontend  
- Guardrails  

This document explains **how the harness works**, **how to use it**, and **how to extend it**.  
It complements (but does not replace) the topic‑specific guides:

- `test-architecture.md`  
- `integration-testing.md`  
- `authentication-testing.md`  
- `frontend-testing.md`  
- `form-testing.md`  

---

# 1. Purpose of the Test Harness

The test harness provides:

- A **deterministic**, **slice‑aligned**, **layer‑pure** testing environment  
- A consistent way to test:
  - API endpoints  
  - Application handlers  
  - Infrastructure repositories/readers  
  - Frontend components/pages  
  - Guardrail rules  
- A unified set of helpers for:
  - Authentication  
  - Cookies  
  - Database setup  
  - DI overrides  
  - Environment seams  
  - ProblemDetails assertions  

The harness ensures that every test:

- Runs in isolation  
- Uses the correct layer  
- Uses the correct dependencies  
- Respects purity rules  
- Matches the vertical slice architecture  

---

# 2. Test Layering Model

````text
API Tests
    ↓ (dispatchers)
Application Tests
    ↓ (repositories/readers)
Infrastructure Tests
    ↓ (database)
Frontend Tests
    ↓ (API clients)
Guardrail Tests
````

Each layer has its own harness:

| Layer | Harness | Purpose |
|-------|---------|---------|
| API | ApiTestFactory | Full end‑to‑end HTTP tests |
| Application | Handler test harness | Pure handler + validator tests |
| Infrastructure | Testcontainers + PostgresFixture | Real DB integration |
| Frontend | Vitest + RTL + FormCommand harness | Component/page tests |
| Guardrails | Architecture tests | Enforce purity + boundaries |

---

# 3. API Test Harness

API tests use **ApiTestFactory**, which provides:

- A full ASP.NET Core test host  
- Real routing  
- Real middleware  
- Real DI  
- Real JSON serialization  
- Real ProblemDetails mapping  
- Real cookies  
- Real session cookie issuance (after US‑111)  

## 3.1 Creating a test client

````csharp
var app = new ApiTestFactory();
var client = app.CreateClient();
````

## 3.2 Faking authentication

````csharp
client = client.WithAuthenticatedUser(ownerId);
````

This injects:

- A fake `ClaimsPrincipal`  
- A valid `ICurrentUser`  
- A valid authentication scheme  

## 3.3 Overriding DI

````csharp
app.Override<ICurrentTime, FakeClock>();
````

## 3.4 Testing cookies

````csharp
var cookie = client.GetSetCookie("session");
````

## 3.5 Testing ProblemDetails

````csharp
var problem = await response.ReadProblemDetailsAsync();
problem.Title.Should().Be("Unauthorized");
````

## 3.6 Testing endpoints with and without auth

- Without auth → expect 401  
- With auth → expect success  

---

# 4. Application Test Harness

Application tests run **pure**, without:

- HTTP  
- EF Core  
- Infrastructure  
- Cookies  
- Middleware  

## 4.1 Mocking ICurrentUser

````csharp
_currentUser.Id.Returns(ownerId);
````

## 4.2 Mocking repositories/readers

````csharp
_repository.SaveAsync(...).Returns(...);
````

## 4.3 Testing handlers

````csharp
var result = await handler.Handle(command, ct);
````

## 4.4 Testing validators

````csharp
var errors = validator.Validate(command);
````

## 4.5 Testing domain events

````csharp
dispatcher.Received().PublishAsync(Arg.Any<DogRegisteredDomainEvent>());
````

---

# 5. Infrastructure Test Harness

Infrastructure tests use:

- **Testcontainers**  
- **PostgresFixture**  
- **Real EF Core**  
- **Real migrations**  

## 5.1 Using the fixture

````csharp
public class DogRepositoryTests : IClassFixture<PostgresFixture>
{
    private readonly AppDbContext _db;
}
````

## 5.2 Testing repositories

````csharp
await _db.Dogs.AddAsync(dog);
await _db.SaveChangesAsync();
````

## 5.3 Testing readers

````csharp
var result = await reader.GetDogProfile(id, ct);
````

## 5.4 Testing EF Core configurations

- Ensure required fields  
- Ensure relationships  
- Ensure indexes  

---

# 6. Frontend Test Harness

Frontend tests use:

- **Vitest**  
- **React Testing Library**  
- **FormCommand test harness**  
- **Mocked API clients**  
- **Mocked navigation**  

## 6.1 Component tests

````ts
render(<RegisterDogForm />);
````

## 6.2 Page tests

````ts
render(<Page params={{ id: "123" }} />);
````

## 6.3 Mocking API clients

````ts
vi.mock("@/api/registerDog", () => ({
  registerDog: vi.fn().mockResolvedValue(successResult)
}));
````

## 6.4 Testing FormCommand flows

````ts
await user.click(screen.getByRole("button", { name: "Submit" }));
````

## 6.5 Testing useCommand/useApiQuery

````ts
expect(result.current.state.status).toBe("success");
````

---

# 7. Guardrail Test Harness

Guardrail tests enforce:

- Architecture boundaries  
- Layer purity  
- Reader isolation (ADR‑0021)  
- Endpoint purity  
- Forbidden references  
- Naming conventions  
- DI correctness  

## 7.1 Architecture tests

````csharp
Types.InAssembly(Application)
    .Should().NotReference(Infra);
````

## 7.2 Reader isolation

````csharp
QueryHandlers.Should().NotUseRepositories();
````

## 7.3 Endpoint purity

````csharp
Endpoints.Should().NotReferenceHandlers();
````

## 7.4 DI guardrails

````csharp
AllServices.Should().BeResolvable();
````

---

# 8. Authentication Helpers

The test harness provides helpers for:

- Faking authentication  
- Injecting claims  
- Injecting session cookies  
- Testing callback pipeline  
- Testing session creation  

## 8.1 Fake authenticated user

````csharp
client.WithAuthenticatedUser(ownerId);
````

## 8.2 Fake session cookie

````csharp
client.WithSessionCookie("abc123");
````

## 8.3 Testing callback pipeline

````csharp
var result = await client.GetAsync("/api/auth/callback?code=123");
````

---

# 9. Database Helpers

- Automatic database creation  
- Automatic migration application  
- Automatic cleanup  
- Per‑test isolation  

## 9.1 Resetting database

````csharp
await fixture.ResetAsync();
````

---

# 10. Environment Seams

The harness provides seams for:

- Time (`IClock`)  
- Randomness  
- Configuration  
- External services  

## 10.1 Overriding time

````csharp
app.Override<IClock>(new FakeClock(DateTime.Parse("2024-01-01")));
````

---

# 11. Mocking Conventions

- Use NSubstitute for Application tests  
- Use real DB for Infrastructure tests  
- Use vi.mock for frontend tests  
- Never mock EF Core  
- Never mock HttpContext in API tests  
- Never mock FormCommand  

---

# 12. Slice TDD Workflow (Unified)

````text
Application → Infrastructure → API → Frontend
````

Each slice follows:

1. RED — write failing test  
2. GREEN — implement code  
3. REFACTOR — clean up  

This applies to:

- Commands  
- Queries  
- Aggregates  
- Callback pipeline  
- Frontend forms  
- Frontend pages  

---

# 13. Troubleshooting

## 13.1 API tests failing with 401  
→ Missing `WithAuthenticatedUser`.

## 13.2 Repository tests failing  
→ Migrations not applied.

## 13.3 Frontend tests failing  
→ API client not mocked.

## 13.4 Guardrail tests failing  
→ Layer violation.

## 13.5 Callback pipeline failing  
→ Step cleared a field (invariant violation).

---

# 14. Reference Examples

See:

- `integration-testing.md`  
- `authentication-testing.md`  
- `frontend-testing.md`  
- `form-testing.md`  
- `test-architecture.md`  

Each contains slice‑specific examples.

---

# Summary

The test harness provides:

- A unified, deterministic testing environment  
- Layer‑pure testing at every level  
- Strong guardrails  
- Full slice TDD support  
- Authentication, cookie, and session helpers  
- Testcontainers for real DB integration  
- Frontend FormCommand + state machine testing  
- Architecture enforcement  

This document is the **entry point** for all testing in Camp Fit Fur Dogs.

