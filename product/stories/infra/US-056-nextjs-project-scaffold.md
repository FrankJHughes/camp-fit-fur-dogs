---
id: US-056
title: "Nextjs Project Scaffold"
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
# US-056 — Next.js Project Scaffold

> Repurposed from "Next.js Project Bootstrap." Original concept absorbed; scope updated for Sprint 4.

## User Story

As a **contributor**, I want an initialized Next.js + TypeScript project with folder structure and dev server proxying to the .NET API, so that frontend development can begin.

## Context

With the frontend technology decision formalized in ADR-0012 (US-055), the project needs a working Next.js scaffold that follows the documented conventions. The dev server must proxy API calls to the .NET backend so the frontend and backend can run together during development. This scaffold becomes the foundation for all frontend stories.

## Scope

- Initialized Next.js + TypeScript project in the repo.
- Dev server proxy configuration targeting the .NET API.
- CI pipeline updated to build the frontend.
- Landing page that calls the health endpoint as a smoke test.

## Acceptance Criteria

- [x] Next.js project initialized with TypeScript in the repo.
- [x] Folder structure follows conventions documented in US-055 ADR.
- [x] Dev server proxies API calls to the .NET backend.
- [x] `npm run dev` starts the frontend and shows a "Camp Fit Fur Dogs" landing page.
- [x] Landing page calls the health endpoint and displays the result.
- [x] CI pipeline updated to build the frontend project.
- [x] Frontend project README documents folder structure, dev server setup, and proxy configuration.

## Dependencies

- US-055 (ADR-0012: Frontend Technology).

## Open Questions

- None.

