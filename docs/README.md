# Documentation Hub

This directory is the architecture and engineering map for the repository. It connects the code under `src` to the design intent, subsystem boundaries, and operational guidance.

## Scope

- `src/CampFitFurDogs` — the product application: its API, application layer, domain model, and infrastructure.
- `src/Frank/Core` — the shared platform foundation for API hosting, commands, queries, domain primitives, persistence patterns, and infrastructure services.
- `src/Frank/Identity` — authentication, sessions, users, authorization, and identity integration.
- `src/Frank/Testing` — reusable test helpers and HTTP-focused integration infrastructure.

## How to read this folder

1. Start with `00-overview` for the high-level architecture and conventions.
2. Read `01-campfitfurdogs` for the product-specific domain and feature boundaries.
3. Review `02-frank-core` and `03-frank-identity` to understand the reusable platform modules.
4. Reference `04-testing`, `05-cross-cutting`, and `06-deployment` for verification and runtime concerns.
5. Use `07-adrs` to understand the rationale behind major decisions.

## Documentation principles

- Align documentation with the actual source tree.
- Describe responsibilities before implementation detail.
- Distinguish architecture concerns from operational concerns.
- Keep the content grounded in the real codebase rather than generic assumptions.
