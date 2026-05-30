# Integration Testing Guide

Integration tests validate **real API + real database behavior** using a temporary Neon branch.  
They ensure that every pull request targeting `main` is fully exercised end‑to‑end before merge.

Integration tests are part of the system’s **operational safety net**, complementing unit tests, guardrails, and preview‑environment tests.

---

# What Integration Tests Cover

Integration tests validate:

- API + database behavior  
- EF Core migrations against a real PostgreSQL instance  
- End‑to‑end flows for key vertical slices (customers, dogs, authentication‑dependent flows)  
- Repository + reader behavior  
- Real DI container wiring  
- Real middleware pipeline  

They do **not** replace:

- [Test Architecture](ca://s?q=Show_test_architecture_guide)  
- [Authentication Testing](ca://s?q=Show_authentication_testing_guide)  
- [Frontend Testing](ca://s?q=Show_frontend_testing_guide)

Integration tests focus on **backend correctness under real conditions**.

---

# CI Behavior (GitHub Actions)

On every pull request targeting `main`, the CI workflow performs the following sequence:

1. **Create a Neon branch database**  
   - Named `pr-<number>`  
   - Expires automatically after 14 days  

2. **Apply EF Core migrations**  
   - Uses the same migration pipeline as production  
   - Ensures schema correctness before tests run  

3. **Run the integration test suite**  
   - Executes `CampFitFurDogs.IntegrationTests`  
   - Uses the Neon branch connection string  
   - Validates repository, reader, and API behavior  

4. **Delete the Neon branch database**  
   - Ensures no leftover resources  
   - Keeps Neon clean and cost‑efficient  

This ensures every PR is validated against a **real database**, not mocks or in‑memory substitutes.

---

# Local Integration Testing

You can run integration tests locally using the same script CI uses:

```
.\scripts\integration\Run-IntegrationTests.ps1 -ConnectionString "<your connection string>"
```

### Local workflow options

- Use a **local Postgres container**  
- Use a **Neon preview branch** (download `db-conn.txt` from CI)  
- Use a **personal Neon branch** for debugging  

Local integration tests should behave identically to CI.

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
