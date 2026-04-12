# ADR-0012: Frontend Technology — React with Next.js

| Field     | Value                              |
|-----------|------------------------------------|
| Status    | Accepted                           |
| Date      | 2026-04-11                         |
| Deciders  | Frank Hughes                       |

## Context

Camp Fit Fur Dogs has a .NET backend using DDD layered architecture
(ADR-0002) with CQRS command/query pipelines. Sprint 4 introduces the
first frontend vertical slice — the Register Dog Page (US-084). Before
building UI, the team needs to commit to a frontend framework so that
all subsequent stories share a common technology, folder structure, and
testing approach.

The chosen framework must:

- Support TypeScript for compile-time safety across the stack.
- Integrate cleanly with the .NET API via dev-server proxying.
- Provide a mature testing story compatible with TDD (red-green-refactor).
- Have a large ecosystem for component libraries, tooling, and hiring.
- Support server-side rendering and static generation for future needs.

## Decision

We will use **React** with **Next.js** and **TypeScript** as the
frontend technology stack.

### Alternatives Considered

| Alternative         | Strengths                                        | Why Not                                                       |
|---------------------|--------------------------------------------------|---------------------------------------------------------------|
| **Blazor (WASM)**   | Single language (.NET/C#); shared domain models  | Smaller ecosystem; less mature component testing; larger WASM payload; limited community resources for rapid problem-solving |
| **Blazor (Server)** | Single language; no WASM payload                 | Requires persistent SignalR connection; latency on every interaction; poor offline story |
| **Angular**         | Full framework; strong TypeScript integration    | Heavier; steeper learning curve; more opinionated than needed for this project's scope |
| **Vue + Nuxt**      | Lighter than Angular; good DX                    | Smaller ecosystem than React; fewer testing patterns documented; less hiring reach |

### Why React + Next.js

- **Ecosystem size** — React has the largest component library and
  tooling ecosystem. Problems encountered during development are likely
  already solved and documented.
- **Testing maturity** — Vitest + React Testing Library provide fast,
  reliable component testing that supports TDD at the UI layer. This is
  critical for the project's strict red-green-refactor discipline.
- **Next.js conventions** — File-based routing, API route proxying, and
  built-in dev server simplify the integration with the .NET backend.
  The dev server proxy eliminates CORS configuration during development.
- **TypeScript first-class** — Next.js scaffolds with TypeScript by
  default. The typed API client layer (US-102) benefits from end-to-end
  type safety.
- **Future flexibility** — Server-side rendering (SSR) and static site
  generation (SSG) are available if the product evolves toward public
  pages, SEO, or performance-sensitive views.

## Consequences

### Positive

- Large talent pool and community support for onboarding contributors.
- Vitest + React Testing Library enable TDD at the component layer from
  day one (US-103).
- Next.js dev server proxy provides seamless local development against
  the .NET API without CORS configuration.
- File-based routing reduces boilerplate and enforces naming conventions.
- TypeScript catches integration errors at compile time, especially in
  the typed API client (US-102).

### Negative

- Two language ecosystems (.NET + Node.js) increase build complexity.
  CI must build, test, and lint both stacks.
- Contributors need proficiency in both C# and TypeScript. The skill
  overlap is common but not universal.
- Node.js tooling (npm, Vitest, ESLint) adds a second dependency tree
  to manage and keep secure.

### Neutral

- Frontend folder conventions, API client patterns, and testing
  patterns must be established as part of the scaffold (US-056) and
  testing setup (US-103). These are Sprint 4 deliverables and become
  the reference for all future UI work.
- The decision does not constrain the backend in any way. The .NET API
  remains framework-agnostic — any frontend could consume it.
- Deployment model (static export vs. Node.js server vs. containerized)
  is a separate decision deferred until the product approaches
  production readiness.
