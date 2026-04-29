# Developer Guide

Welcome to Camp Fit Fur Dogs. This guide gets you from zero to a running dev loop. For architecture, purity, and slice-level rules, see the companion documents linked in §4.

---

## 1. Prerequisites

Install the following tools:

| Tool | Version | Purpose |
|------|---------|---------|
| .NET SDK | 9.0+ | Build and run the API |
| Docker Desktop | Latest | Local container runtime |
| PowerShell | 7+ | Developer scripts |
| Git | 2.x | Source control |
| Node.js | 22 LTS+ | Frontend runtime |
| GitHub CLI (`gh`) | 2.x | Issue and PR management |

---

## 2. First-Time Setup

### 2.1 Docker Desktop (Windows)

Docker must be running for Dev Containers and Testcontainers.

1. Open Docker Desktop
2. Settings → General
3. Enable **Start Docker Desktop when you sign in**
4. Verify:

```powershell
docker version
```

If the daemon isn't ready, wait a few seconds and retry.

### 2.2 Git Identity

VS Code forwards your host Git identity into the Dev Container:

```powershell
git config --global user.name "Your Name"
git config --global user.email "you@example.com"
```

---

## 3. The Developer Loop

The developer loop is the daily workflow for contributing to the system.
It ensures fast feedback, clean commits, and consistent architecture.

### 3.1 The Loop

1. **Pick a story**
   - Move it into the sprint board
   - Create a feature branch
   - Never commit to `main`

2. **Write the first failing test**
   - Start at the highest layer affected
   - Let the test drive the design

3. **Make the test pass**
   - Write the smallest amount of code
   - Follow purity rules
   - Use the dispatcher pipeline

4. **Refactor**
   - Improve naming, structure, and slice boundaries
   - Ensure no purity violations
   - Keep commits small and meaningful

5. **Run the full test suite**
   - Guardrails must pass
   - No broken slices
   - No architectural regressions

6. **Update documentation**
   - ADRs for decisions
   - Developer guides for conventions
   - README indexes for discoverability

7. **Commit and push**
   - Use conventional commit messages
   - Keep commits atomic
   - Open a PR with the merge checklist

8. **Review and merge**
   - Ensure all checks pass
   - Ensure documentation is updated
   - Clean up the feature branch

### 3.2 Why This Loop Matters

- Keeps architecture consistent
- Prevents regressions
- Ensures every change is intentional
- Makes the system self-documenting
- Supports long-term maintainability

The developer loop is the heartbeat of the project.


---

## 4. Adding a New Feature Slice

The project follows a strict TDD sequence for every vertical slice:

- **Command slices** (write path): Domain → Application → Infrastructure → API
- **Query slices** (read path): Application → Infrastructure → API

Each step starts with a failing test. The walkthrough covers file naming, folder placement, and the conventions that make every slice purely additive (no shared files to modify).

See [Feature Slice Walkthrough](developer/feature-slice-walkthrough.md) for the complete step-by-step guide.
---

## 5. Quick Links

| Topic | Document |
|-------|----------|
| Repository structure | [Folder Structure](developer/folder-structure.md) |
| Vertical slice anatomy | [Abstractions Contract](developer/abstractions-contract.md) |
| Dispatcher pipeline | [Dispatcher Pipeline](developer/dispatcher-pipeline.md) |
| Endpoint conventions | [API Endpoint Purity](developer/api-endpoint-purity.md) |
| DI conventions | [DI Conventions](developer/di-conventions.md) |
| Domain events | [Domain Events](developer/domain-events.md) |
| Shared kernel | [Shared Kernel](developer/shared-kernel.md) |
| Test architecture | [Test Architecture](developer/test-architecture.md) |
| Purity rules | [Purity Rules](developer/purity-rules.md) |
| Frontend testing | [Frontend Testing](developer/frontend-testing.md) |
| Feature slice walkthrough | [Feature Slice Walkthrough](developer/feature-slice-walkthrough.md) |
| TDD discipline | [copilot-instructions.md](../../.github/copilot-instructions.md) §TDD |
| Source control & git hooks | [copilot-instructions.md](../../.github/copilot-instructions.md) §Source Control |
| Branching & PR workflow | [CONTRIBUTING.md](../../CONTRIBUTING.md) |
| Secrets Setup | [secrets-setup.md](developer/secrets-setup.md) |
| Integration Testing | [integration-testing.md](developer/integration-testing.md) |
