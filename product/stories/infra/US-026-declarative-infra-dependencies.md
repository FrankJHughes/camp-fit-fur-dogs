---
id: US-026
title: "Declarative Infra Dependencies"
epic: ""
milestone: ""
status: shipped
domain: infra
urgency: ""
importance: ""
covey_quadrant: ""
vertical_slice: false
emotional_guarantees: ""
legal_guarantees: ""
---
# Declarative Infrastructure Dependencies

## Intent

All infrastructure services required by the application — database, cache, and message
broker — are defined in a single, version-controlled declarative configuration file at
the repository root. Any developer or automation system can start, verify, and stop the
full infrastructure stack with a single command.

## Value

Infrastructure definitions are the foundation of the Diamond Model (ADR-003, Layer 1).
Every other developer-experience layer — local bootstrap (L3), containerized development
environment (L2), and CI — consumes this single specification. Defining infrastructure
declaratively eliminates "works on my machine" drift, ensures persistent data survives
container restarts, and provides health checks that distinguish "starting" from "ready."

## Acceptance Criteria

- [x] A declarative configuration file exists at the repository root defining all infrastructure services
- [x] Each service includes a health check so consumers can wait for readiness, not just container start
- [x] Persistent storage is configured via named volumes so data survives down / up cycles
- [x] Connection strings, ports, and credentials are documented via environment variables with sensible development defaults
- [x] The full stack starts and stops with a single command (docker compose up -d / docker compose down)
- [x] Definitions are reusable by Codespaces, DevContainers, and CI without modification

## Out of Scope

- Application container definitions (the API does not run inside the compose stack at L1)
- Prescribing specific database migrations or seed data
- Cloud or production infrastructure provisioning
- Code-level integration with infrastructure services (NuGet packages, DbContext registration)

