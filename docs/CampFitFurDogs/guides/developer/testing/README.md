# Testing Guides

This folder contains all developer-facing documentation related to **testing strategy, patterns, and practices** across the CampFitFurDogs backend and frontend.

These guides explain *how to test*, not *what to test*.  
They complement the architectural guides by showing how tests map to the system’s layers.

---

# Contents

- **[Test Architecture](../test-architecture.md)**  
  How tests map to API, Application, Domain, Infrastructure, and Frontend layers.

- **[Integration Testing](../integration-testing.md)**  
  How to write integration tests using the test host, fake services, the dispatcher, and EF Core test seams.

- **[Authentication Testing](../authentication-testing.md)**  
  How to test the OIDC login flow, callback behavior, session cookie issuance, identity mapping, and authentication error boundaries.

- **[Frontend Testing](../frontend-testing.md)**  
  How to test React components, forms, hooks, and UI flows using Vitest and React Testing Library.

- **[Form Testing](../form-testing.md)**  
  How to test form validation, submission, error states, and state machine transitions.

- **[Infrastructure Testing](../infrastructure-testing.md)**  
  How to test repositories, readers, EF Core mappings, migrations, and hosting provider seams.

- **[Preview Environment Testing](../preview-testing.md)**  
  How to test PR Preview behavior, teardown/readiness probes, and environment‑specific hosting provider logic.

---

# Purpose

These guides help developers:

- Write tests at the correct layer  
- Avoid over-testing or under-testing  
- Use the test host and fakes correctly  
- Understand how validation boundaries affect test placement  
- Maintain consistent testing patterns across vertical slices  
- Test authentication flows and session behavior safely and deterministically  
- Test EF Core mappings and repository behavior without leaking Infrastructure details  
- Test frontend forms, queries, and components in isolation  
- Test PR Preview behavior using hosting provider seams  

---

# Testing Philosophy

CampFitFurDogs uses a **layered testing strategy**:

- **Unit tests** validate pure logic (Domain, Application validators, small helpers).  
- **Handler tests** validate Application orchestration without Infrastructure.  
- **Integration tests** validate Infrastructure, EF Core, and API endpoints.  
- **Frontend tests** validate UI behavior, form flows, and API client interactions.  
- **Preview tests** validate hosting provider behavior and environment configuration.

Tests must:

- Be deterministic  
- Avoid real network calls  
- Use fakes for external dependencies  
- Use the test host for API integration tests  
- Use Frank’s seams for hosting provider tests  
- Avoid mocking domain behavior  

---

# Test Host & Fakes

The test host provides:

- Automatic endpoint discovery  
- Automatic handler/validator registration  
- In‑memory EF Core database  
- Fake hosting provider  
- Fake environment  
- Fake artifact client  
- Fake PR parser  
- Fake configuration writer  

Fakes must be used instead of mocks whenever possible.

---

# Layer Boundaries & Test Placement

Tests must align with the system’s architectural boundaries:

- **Domain tests** validate invariants and domain events.  
- **Application tests** validate handlers, validators, and orchestration.  
- **Infrastructure tests** validate EF Core mappings, repositories, and readers.  
- **API tests** validate endpoint purity, DTO binding, and dispatcher behavior.  
- **Frontend tests** validate UI flows, form state machines, and query behavior.

Tests must not cross layers.

---

# Related Documentation

- [Validation Boundaries](ca://s?q=Show_validation_boundaries_doc)  
- [Dispatcher Pipeline](ca://s?q=Show_dispatcher_pipeline_guide)  
- [API Endpoint Purity](ca://s?q=Show_API_endpoint_purity_guide)  
- [Authentication Operations](ca://s?q=Show_authentication_operations_doc)  
- [Architecture Conventions](ca://s?q=Open_Architecture_Conventions)  
- [Code Conventions](ca://s?q=Open_Code_Conventions)  
- [Workflow Conventions](ca://s?q=Open_Workflow_Conventions)
