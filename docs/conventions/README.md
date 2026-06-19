# Conventions Index

This index provides a unified map of all conventions across Frank, CampFitFurDogs,
testing, and development workflow.  
Conventions define **how the system is implemented** and serve as the authoritative
rules for code, architecture, testing, and workflow.

Governance defines **what must be true**.  
Conventions define **how it must be done**.

---

# Frank Conventions

Frank provides platform‑level primitives, hosting, OIDC protocol support, DI,
test seams, and cross‑cutting infrastructure.

## Code Conventions

- [DI & Registration](../Frank/conventions/code/di-and-registration-conventions.md)
- [CQRS](../Frank/conventions/code/cqrs-conventions.md)
- [Immutable Context Builder](../Frank/conventions/code/immutable-context-builder-conventions.md)
- [Endpoint Conventions](../Frank/conventions/code/endpoint-conventions.md)
- [Hosting Provider Conventions](../Frank/conventions/code/hosting-provider-conventions.md)
- [Security Header Conventions](../Frank/conventions/code/security-header-conventions.md)
- [Environment Seam Conventions](../Frank/conventions/code/environment-seam-conventions.md)
- [OIDC Protocol Code Conventions](../Frank/conventions/code/oidc-protocol-code-conventions.md)
- [Test Seam Conventions](../Frank/conventions/code/test-seam-conventions.md)

## Architecture Conventions

- [OIDC Protocol Architecture](../Frank/conventions/platform-architecture/oidc-protocol-architecture.md)

---

# CampFitFurDogs Conventions

CampFitFurDogs implements product‑level behavior, domain logic, application
orchestration, persistence, and UI.

## Code Conventions

- [Backend Layering](../CampFitFurDogs/conventions/code/backend-layering-conventions.md)
- [Domain Conventions](../CampFitFurDogs/conventions/code/domain-conventions.md)
- [Application Conventions](../CampFitFurDogs/conventions/code/application-conventions.md)
- [Infrastructure Conventions](../CampFitFurDogs/conventions/code/infrastructure-conventions.md)
- [API Endpoint Conventions](../CampFitFurDogs/conventions/code/api-endpoint-conventions.md)
- [Authentication Callback Conventions](../CampFitFurDogs/conventions/code/authentication-callback-conventions.md)
- [Session Cookie Conventions](../CampFitFurDogs/conventions/code/session-cookie-conventions.md)
- [EF Core Conventions](../CampFitFurDogs/conventions/code/ef-core-conventions.md)
- [Frontend Conventions](../CampFitFurDogs/conventions/code/frontend-conventions.md)

---

# Test Conventions

Test conventions define how to use Frank’s test seams, the ApiFactory harness,
fake builders, and deterministic test data.

- [Testing Conventions](../CampFitFurDogs/conventions/test/testing-conventions.md)
- [Test Harness Conventions](../CampFitFurDogs/conventions/test/test-harness-conventions.md)
- [Fake Builder Conventions](../CampFitFurDogs/conventions/test/fake-builder-conventions.md)
- [HttpClient Test Conventions](../CampFitFurDogs/conventions/test/httpclient-test-conventions.md)
- [Integration Test Conventions](../CampFitFurDogs/conventions/test/integration-test-conventions.md)
- [Unit Test Conventions](../CampFitFurDogs/conventions/test/unit-test-conventions.md)
- [Frontend Test Conventions](../CampFitFurDogs/conventions/test/frontend-test-conventions.md)
- [Test Data Conventions](../CampFitFurDogs/conventions/test/test-data-conventions.md)
- [Test Fixture Conventions](../CampFitFurDogs/conventions/test/test-fixture-conventions.md)

---

# Development Workflow Conventions

Workflow conventions define how stories, tasks, branches, commits, PRs, CI/CD,
and releases operate.

- [Branching Conventions](../CampFitFurDogs/conventions/development-workflow/branching-conventions.md)
- [Commit Message Conventions](../CampFitFurDogs/conventions/development-workflow/commit-message-conventions.md)
- [Pull Request Conventions](../CampFitFurDogs/conventions/development-workflow/pull-request-conventions.md)
- [Issue Lifecycle Conventions](../CampFitFurDogs/conventions/development-workflow/issue-lifecycle-conventions.md)
- [Task Lifecycle Conventions](../CampFitFurDogs/conventions/development-workflow/task-lifecycle-conventions.md)
- [Story Lifecycle Conventions](../CampFitFurDogs/conventions/development-workflow/story-lifecycle-conventions.md)
- [CI/CD Conventions](../CampFitFurDogs/conventions/development-workflow/ci-cd-conventions.md)
- [Local Development Conventions](../CampFitFurDogs/conventions/development-workflow/local-development-conventions.md)
- [Preview Environment Conventions](../CampFitFurDogs/conventions/development-workflow/preview-environment-conventions.md)
- [Release Conventions](../CampFitFurDogs/conventions/development-workflow/release-conventions.md)

---

# Purpose of Conventions

Conventions ensure:

- predictable architecture  
- consistent code quality  
- deterministic CI/CD behavior  
- safe and maintainable documentation  
- clear onboarding for new contributors  
- reduced cognitive load  
- alignment with Frank’s platform guarantees  
- preview‑safe behavior across all environments  

Conventions are **source‑of‑truth documents**.  
Guides and runbooks must conform to them.

---

# Updating Conventions

Changes to conventions must:

1. be proposed and discussed in a PR  
2. reference the relevant ADR (if architectural)  
3. follow the Universal Patch Rule  
4. update all affected convention files  
5. update any impacted guides or runbooks  
6. ensure guardrail tests remain aligned  

Conventions are reviewed at each sprint close.
