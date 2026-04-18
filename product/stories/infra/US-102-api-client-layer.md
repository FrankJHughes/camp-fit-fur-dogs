# US-102 — API Client Layer

## User Story

As a **frontend developer**, I want a typed fetch wrapper with error handling and response parsing, so that UI components can call API endpoints with type safety.

## Context

The proving slice (US-084) needs to call the RegisterDog API endpoint. Rather than using raw `fetch` that would be immediately replaced, this story establishes the typed API client pattern that all future UI slices will use. The mocking patterns established here become the standard for all frontend TDD.

## Scope

- Typed API client module (e.g., `lib/api/client.ts`).
- Generic request/response handling with TypeScript types.
- Error handling for network, HTTP, and validation errors.
- Usage guide documenting how to add a new typed endpoint call.

## Acceptance Criteria

- [x] Typed API client module created with generic request/response handling.
- [x] TypeScript types enforce compile-time safety on API calls.
- [x] Network errors produce meaningful error objects.
- [x] HTTP errors (4xx/5xx) produce typed error objects.
- [x] Validation errors produce field-level error objects.
- [x] Base URL configuration supports dev proxy and production.
- [x] API client usage guide documents how to add a new typed endpoint call.
- [x] All tests pass in CI.

## Dependencies

- US-056 (Next.js Project Scaffold).

## Open Questions

- None.
