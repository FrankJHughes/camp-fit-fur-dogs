# Developer Guide

Welcome to Camp Fit Fur Dogs. This guide provides the high‑level orientation you need to work effectively in the codebase. It explains how to set up your environment, how the repository is organized, and where to find the detailed architectural rules that govern the system.

For all architecture, purity, and slice‑level rules, see the companion documents in:

```
docs/guides/developer/
```

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

## 2. First‑Time Setup

### 2.1 Docker Desktop (Windows)

Docker must be running for Dev Containers and Testcontainers.

1. Open Docker Desktop  
2. Settings → General  
3. Enable **Start Docker Desktop when you sign in**  
4. Verify:

```powershell
docker version
```

If the daemon isn’t ready, wait a few seconds and retry.

### 2.2 Git Identity

VS Code forwards your host Git identity into the Dev Container:

```powershell
git config --global user.name "Your Name"
git config --global user.email "you@example.com"
```

---

## 3. Repository Structure

The solution uses a clean, layered architecture with vertical slices:

```
src/
  CampFitFurDogs.Api/
  CampFitFurDogs.Application/
  CampFitFurDogs.Domain/
  CampFitFurDogs.Infrastructure/
  CampFitFurDogs.SharedKernel/

tests/
  CampFitFurDogs.Api.Tests/
  CampFitFurDogs.Application.Tests/
  CampFitFurDogs.Domain.Tests/
  CampFitFurDogs.Infrastructure.Tests/
```

For a full explanation of slice anatomy and layer responsibilities, see:

- **Folder Structure** — `developer/folder-structure.md`

---

## 4. Working in the Codebase

### 4.1 Vertical Slice Workflow

Each feature spans:

- Abstractions (commands, queries, results)
- Application (handlers, validators)
- Domain (entities, value objects, domain events)
- Infrastructure (repositories)
- API (endpoints)

See:

- **Abstractions Contract** — `developer/abstractions-contract.md`
- **Dispatcher Pipeline** — `developer/dispatcher-pipeline.md`
- **API Endpoint Purity** — `developer/api-endpoint-purity.md`
- **DI Conventions** — `developer/di-conventions.md`

### 4.2 Domain Modeling

Domain logic lives in the Domain layer:

- Entities  
- Value objects  
- Domain events  
- Invariants  

See:

- **Domain Events** — `developer/domain-events.md`
- **Shared Kernel** — `developer/shared-kernel.md`

### 4.3 Testing

Tests enforce both correctness and architectural purity.

See:

- **Test Architecture** — `developer/test-architecture.md`
- **Purity Rules** — `developer/purity-rules.md`

---

## 5. Development Workflow

### 5.1 Branching

Follow the project’s branching conventions:

- Create a feature branch per story  
- Never commit directly to `main`  
- Use GitHub Issues only when a story enters a sprint  

### 5.2 Running the App

Inside the Dev Container:

```powershell
make run
```

### 5.3 Running Tests

```powershell
make test
```

### 5.4 Creating a Pull Request

Use GitHub CLI:

```powershell
gh pr create --fill
```

Include:

- `Closes #<issue-number>`
- The 6‑item merge checklist from `.github/PULL_REQUEST_TEMPLATE.md`

---

## 6. Test‑Driven Development (TDD)

Camp Fit Fur Dogs is built using strict Test‑Driven Development.  
Every change — from a small refactor to a new vertical slice — follows the same discipline:

1. **Red** — write a failing test  
2. **Green** — write the minimum code to make it pass  
3. **Refactor** — improve the design while keeping tests green  

This applies across all layers:

- Domain (entities, value objects, invariants)
- Application (handlers, validators, dispatchers)
- API (endpoint tests)
- Infrastructure (repositories, persistence)
- Guardrails (architecture enforcement)

TDD is not optional.  
It is the foundation of the system’s correctness, design, and maintainability.

---

## 7. The Developer Loop

The developer loop is the daily workflow for contributing to the system.  
It ensures fast feedback, clean commits, and consistent architecture.

### 7.1 The Loop

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

### 7.2 Why This Loop Matters

- Keeps architecture consistent  
- Prevents regressions  
- Ensures every change is intentional  
- Makes the system self‑documenting  
- Supports long‑term maintainability  

The developer loop is the heartbeat of the project.

## 8. Git Hooks & Source Control Safety

Camp Fit Fur Dogs enforces a strict source‑control discipline to protect `main`, prevent accidental commits, and ensure every change is intentional and reviewable.

The repository includes Git hooks that run automatically inside the Dev Container. These hooks enforce safety rules before code ever reaches CI.

---

### 8.1 Pre‑Commit Safety

The pre‑commit hook performs:

- **Whitespace cleanup**
- **File formatting checks**
- **Forbidden file checks** (e.g., no stray `.cs` files in Abstractions)
- **Guardrail test stubs** (ensuring new slices follow conventions)
- **Prevention of large accidental commits**

If a hook fails, fix the issue before committing.

---

### 8.2 Pre‑Push Safety

The pre‑push hook ensures:

- All tests pass locally  
- No purity violations  
- No broken slices  
- No missing companion documentation  
- No untracked files that should be committed  

This prevents “push‑and‑pray” workflows.

---

### 8.3 Branch Protection

The project follows strict branch protection rules:

- **Never commit directly to `main`**
- **Every change must come through a PR**
- **Every PR must pass CI**
- **Every PR must include the merge checklist**
- **Every PR must update documentation when architecture changes**

These rules are enforced by GitHub and by local hooks.

---

### 8.4 Feature Branch Workflow

1. Create a branch for each story:

```
git checkout -b feature/us-###-short-description
```

2. Commit frequently, in small, meaningful units.
3. Push regularly to keep your branch backed up.
4. Open a PR early and iterate.

---

### 8.5 Preventing Accidental Commits

The repo includes safeguards to prevent:

- Committing generated files  
- Committing secrets  
- Committing large binaries  
- Committing directly to protected branches  
- Committing without tests  

If a hook blocks your commit, it’s doing its job.

---

### 8.6 Resetting Hooks (Dev Container)

If hooks ever get out of sync:

```powershell
.devcontainer/reset-intellisense.ps1
```

This resets:

- Git hooks  
- Editor config  
- Intellisense caches  

---

### 8.7 Philosophy

Git hooks are not an annoyance — they are a **safety net**.

They ensure:

- You never break `main`
- You never push untested code
- You never bypass architectural rules
- You never forget documentation updates
- You never commit something you didn’t mean to

They are part of the developer loop and part of the culture of the project.

## 9. Where to Go Next

The developer guide is intentionally high‑level.  
All architecture, purity, and contributor rules live in the companion documents:

```
docs/guides/developer/
```

Start with:

- **Folder Structure**  
- **Dispatcher Pipeline**  
- **API Endpoint Purity**  
- **Purity Rules**  

These documents define how to write code that fits the system.


