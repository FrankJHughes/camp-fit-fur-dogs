# Developer Experience — Product Brief

## Vision

A developer joining Camp Fit Fur Dogs should go from zero to a passing test
suite in under five minutes, with no tribal knowledge, no manual tool
installation, and no messages asking "how do I build this?"

## Principles

1. **Local development is the primary path.** Docker is a stated prerequisite.
   Infrastructure runs in containers via shared definitions (L1). The local
   bootstrap (L3) is the primary onboarding path.
2. **Containerized development is a supported alternative.** The dev container
   (L2) consumes the same shared infrastructure definitions as local
   bootstrap. Codespaces is a side effect, not a goal.
3. **Infrastructure is declarative.** All external dependencies (databases,
   caches, message brokers) are defined once in shared Compose fragments and
   consumed by every environment.
4. **Common tasks have common commands.** A task runner (L4) provides a
   uniform interface so build, test, migrate, and lint commands are identical
   across local, containerized, and CI environments.
5. **The editor works out of the box.** Shared editor settings, recommended
   extensions, and format-on-save rules are committed to the repository.

## Architecture — Diamond Model (ADR-003)

```
        L5  CI Pipeline (validates what L4 runs)
       / \
     L2   L3   Dev Container / Local Bootstrap (peer consumers)
       \ /
        L4  Task Runner (uniform commands)
        |
        L1  Shared Infra Definitions (Compose fragments)
```

L1 (infrastructure) and L4 (task runner) are the dual foundations.
L2 (dev container) and L3 (local bootstrap) are peer consumers that
share the same infrastructure and commands. L5 (CI) validates the
same workflows.

## Capability Map

| Capability                   | Story                                   |
| ---------------------------- | --------------------------------------- |
| Architecture Governance      | DX Architecture Decision                |
| Dependency Management        | Declarative Infrastructure Dependencies |
| Command Standardization      | Standardized Developer Commands         |
| Local Bootstrap              | One-Command Local Bootstrap             |
| Containerized Dev            | Containerized Development Environment   |
| Editor & Tooling Consistency | Consistent Editor Experience            |

## Epic

Developer Onboarding

## Success Criteria

- Time from repository access to passing tests: < 5 minutes
- Zero manual steps beyond running one command
- Build and test commands work identically across local, containerized, and CI
- A new contributor can submit a pull request without asking how to set up
