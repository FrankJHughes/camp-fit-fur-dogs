# Changelog — Frank

All notable changes to the **Frank** shared kernel are documented here.
Format follows [Keep a Changelog](https://keepachangelog.com/en/1.1.0/).
Frank uses [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

> **Frank** provides reusable .NET building blocks — domain primitives, CQRS
> abstractions, dispatchers, DI conventions, and persistence infrastructure —
> extracted from [Camp Fit Fur Dogs](https://github.com/FrankJHughes/camp-fit-fur-dogs)
> and designed for any consumer that follows DDD and clean architecture.

---

## [Unreleased]

### Planned
- `IAuditable` interface and `AuditTimestampInterceptor` (US-127)
- `ISoftDeletable` interface, `SoftDeleteInterceptor`, and global query filter convention (US-125)
- `ICommandPipelineBehavior` / `IQueryPipelineBehavior` pipeline behavior abstractions (US-124)

---

## [0.2.0] — 2026-04-18 — Sprint 5

### Added — Foundation Extraction (US-108, PR #164)

The defining release for Frank. Components that had been growing in the Camp Fit
Fur Dogs application project across Sprints 1–4 were extracted into the
SharedKernel as a standalone, reusable library.

**CQRS Abstractions** (`Frank/Abstractions/`)
- `ICommand` / `ICommand<TResult>` — command marker interfaces
- `IQuery<TResult>` — query marker interface
- `ICommandHandler<TCommand>` / `ICommandHandler<TCommand, TResult>` — command handler contracts
- `IQueryHandler<TQuery, TResult>` — query handler contract

**Dispatch** (`Frank/`)
- `CommandDispatcher` — resolves and invokes command handlers via DI

**Dependency Injection** (`Frank/DependencyInjection/`)
- Convention-based auto-registration of handlers, repositories, and readers
- Assembly scanning via `AssemblyMarker`

**Domain Events** (`Frank/Events/`)
- Domain event raise/collect infrastructure for aggregate-level eventing

### Changed
- All CQRS abstractions relocated from `CampFitFurDogs.Application` into `SharedKernel`
- DI auto-registration conventions relocated from `CampFitFurDogs.Infrastructure` into `SharedKernel`
- Camp Fit Fur Dogs now references `SharedKernel` for all CQRS and DI contracts

### Context — Other Sprint 5 contributions consumed by Frank

These shipped alongside the extraction and established patterns that Frank's
consumers depend on:

| Story | Title | Contribution |
|-------|-------|-------------|
| US-050 | Unit of Work | Persistence transaction pattern (app-level, references Frank entities) |
| US-107 | EF Entity Auto-Discovery | `ApplyConfigurationsFromAssembly`, `Set<T>()` convention (app-level) |
| US-106 | Add-Only Slice Architecture | `IEndpoint` auto-discovery, Reader isolation pattern (app-level) |
| US-104 | Architecture Test Consolidation | Pure-reflection guardrails verifying Frank's contracts |

---

## [0.1.0] — 2026-04-03 — Sprint 1

### Added — Frank Is Born (US-017, PR #18)

The SharedKernel project was created with foundational DDD building blocks.
These base types define the domain modeling vocabulary for any consumer.

**Domain Primitives** (`Frank/Domain/`)
- `Entity` — base type with typed identity
- `AggregateRoot` — aggregate base with domain event collection
- `ValueObject` — structural equality base
- `IDomainEvent` — domain event marker interface
- `IRepository<T>` — repository contract

**Tests**
- `SharedKernel.Tests` project scaffold with unit tests for base types

### Context — Sprint 1 consumers

| Story | Title | Contribution |
|-------|-------|-------------|
| US-018 | Domain Model Core Aggregates | First consumer — Dog and Guardian aggregates inherit Frank base types |
| US-019 | API DDD Layer Wiring | Wired Frank into the application DI and persistence layers |
| US-016 | ADR Foundation | Established ADR practice; ADR-0002 (DDD Layered Architecture) governs Frank's layer |

---

## Pre-Frank Lineage — Sprints 2–4

Components that grew in the Camp Fit Fur Dogs application project and were
later extracted into Frank (v0.2.0). Documented here for traceability.

### Sprint 4 — 2026-04-18

| Story | What was created | Later became |
|-------|-----------------|-------------|
| US-051 | CQRS Command/Query Pipelines (ADR-0011) | `Frank/Abstractions/` contracts |
| US-079 | Convention-Based Auto-Registration via Scrutor | `Frank/DependencyInjection/` conventions |

### Sprint 3 — 2026-04-11

| Story | What was created | Later became |
|-------|-----------------|-------------|
| US-027 | `ICommand`, `ICommandHandler`, `ICommandDispatcher` | `Frank/Abstractions/` command contracts |
| US-028 | First command handler (Register Dog) | Validated the dispatch pattern Frank would formalize |
| US-029 | `IQuery<TResponse>`, `IQueryHandler`, `IQueryDispatcher` | `Frank/Abstractions/` query contracts |

### Sprint 2 — 2026-04-08

No Frank changes. Sprint focused on developer experience, documentation, and
governance (Dev Container, Makefile, CONTRIBUTING.md, merge protection).

### Sprint 0 — 2026-03-27

| Story | What was created | Later became |
|-------|-----------------|-------------|
| US-014 | .NET Solution Skeleton | `SharedKernel.csproj` project file and test project scaffold |
| US-015 | CI Baseline Build and Test | CI pipeline that builds and tests Frank alongside the app |

---

## Sprint 6 — No Frank Changes

Sprint 6 (Unreleased) shipped app-level features (Edit Dog, Remove Dog, List
Dogs, Not Found components). No changes to Frank's public API surface. All new
features consumed Frank's existing abstractions without modification —
validating the library's stability.

---

[Unreleased]: https://github.com/FrankJHughes/camp-fit-fur-dogs/compare/v0.2.0...HEAD
[0.2.0]: https://github.com/FrankJHughes/camp-fit-fur-dogs/compare/v0.1.0...v0.2.0
[0.1.0]: https://github.com/FrankJHughes/camp-fit-fur-dogs/releases/tag/v0.1.0
