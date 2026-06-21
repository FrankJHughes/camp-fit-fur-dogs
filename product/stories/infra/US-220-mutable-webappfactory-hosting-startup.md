---
id: US-220
title: "Test Harness: MutableWebApplicationFactory Uses Hosting & Startup Engines"
epic: Infrastructure
milestone: M1+
status: backlog
domain: infra
vertical_slice: false
dependencies:
  - US-176
  - US-181
  - US-185
---

# US‑220 — Test Harness: MutableWebApplicationFactory Uses Hosting & Startup Engines

## Intent

As an **admin**, I must ensure the test harness uses the same HostingEngine and StartupEngine as production so that all integration tests run against a faithful, deterministic replica of the real application.

## Value

The test harness must behave like the real host.  
If tests bypass HostingEngine or StartupEngine, they run with:

- incomplete hosting configuration  
- incorrect environment flags  
- missing startup modules  
- a DI container that does not match production  

This creates drift, hides bugs, and invalidates test results.

By running both engines inside the MutableWebApplicationFactory, the test host becomes:

- realistic  
- deterministic  
- fully configured  
- override‑friendly  

This ensures that integration tests reflect real system behavior while still allowing test‑specific overrides.

---

## Acceptance Criteria

### HostingEngine Integration
- [ ] The MutableWebApplicationFactory invokes `HostingEngine.ApplyHostingEnvironmentConfigurationAsync(builder)` before DI is built  
- [ ] All hosting modules (Local, Render, etc.) run exactly as they do in production  
- [ ] Hosting configuration (URLs, environment flags, secrets) is applied identically to production  
- [ ] Tests can override hosting configuration *after* HostingEngine runs  

### StartupEngine Integration
- [ ] The MutableWebApplicationFactory invokes the StartupEngine to register all application modules  
- [ ] All startup modules run in the same order as production  
- [ ] The DI container shape matches production exactly  
- [ ] Tests can override services *after* StartupEngine runs  

### Test Mutability
- [ ] Tests can replace services (e.g., clock, email sender, outbox, identity resolver)  
- [ ] Tests can override configuration values  
- [ ] Tests can inject fakes and spies without breaking HostingEngine or StartupEngine  
- [ ] Overrides always occur *after* both engines have completed  

### Determinism & Fidelity
- [ ] The test host behaves identically to the real host unless explicitly overridden  
- [ ] No test can accidentally bypass hosting or startup configuration  
- [ ] Integration tests run through the full authentication/session pipeline  
- [ ] Integration tests run through the full dispatcher pipeline  
- [ ] Integration tests run through the full domain event pipeline  

### Safety & Purity
- [ ] HostingEngine and StartupEngine remain unchanged — the test harness only *consumes* them  
- [ ] No test‑specific logic leaks into production modules  
- [ ] No environment detection logic leaks into tests  
- [ ] No DI scanning or registration occurs outside Frank’s DI engine  

---

## Emotional Guarantees

- **EG‑01 No Surprises** — Tests behave like production; no hidden differences.  
- **EG‑03 Calm Protection** — Developers trust that tests reflect real behavior.  
- **EG‑05 Clear Boundaries** — Hosting and startup logic remain pure and isolated.  

---

## Notes

- This story completes the alignment between the test harness and the Frank hosting/startup architecture.  
- Overrides must always run *after* both engines to preserve fidelity.  
- This story enables future stories involving environment‑specific behavior, startup modules, and hosting modules to be tested without drift.  
- **Demo:** Run an integration test that exercises authentication → session creation → domain events. Confirm the test host behaves identically to production, with only the overridden services differing.
