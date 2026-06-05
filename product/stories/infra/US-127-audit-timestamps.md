---
id: US-127
title: "Audit Timestamps"
epic: Infrastructure
milestone: M1+
status: backlog
domain: infra
vertical_slice: false
dependencies:
  - US-050
  - US-107
  - US-108
urgency: 
importance: 
covey_quadrant: 
emotional_guarantees:
legal_guarantees:
---

# US-127: Audit Timestamps

## Intent

As a **system operator**, I need every opted-in entity to automatically track when
it was created and last modified, so that I have a reliable audit trail without
requiring manual wiring in each slice.

## Value

Audit timestamps are foundational infrastructure. Debugging, compliance, data
recovery, and future reporting all depend on knowing when records changed.
Automating this at the persistence layer eliminates an entire class of
"forgot to set the timestamp" bugs and ensures consistency across all current
and future slices.

Implementing this as a Frank capability means any future consumer gets
audit timestamps for free — same extraction pattern established by US-108.

## Acceptance Criteria

### Frank (the capability)
- [ ] `IAuditable` interface in `Frank/Domain/` declares `CreatedAt` and `ModifiedAt` properties
- [ ] `AuditTimestampInterceptor` in `Frank/Persistence/` sets `CreatedAt` on `Added` entities and `ModifiedAt` on `Modified` entities via change tracker state
- [ ] Interceptor ignores entities that do not implement `IAuditable`
- [ ] Interceptor auto-registers via Frank DI conventions — consumers opt in by referencing Frank, not by manual wiring
- [ ] Timestamps use UTC
- [ ] Frank unit tests verify timestamps are set correctly for create and update operations
- [ ] Frank unit tests verify non-auditable entities are unaffected

### Camp Fit Fur Dogs (the consumer)
- [ ] All existing entities implement `IAuditable`
- [ ] EF migration adds `CreatedAt` and `ModifiedAt` columns with sensible defaults for existing rows
- [ ] Architecture test confirms all entities in the app implement `IAuditable`
- [ ] Architecture test confirms no command handler manually sets audit timestamp fields

## Emotional Guarantees

- **EG-01 No Surprises** — Timestamp columns appear consistently on every table; no entity is silently excluded
- **EG-02 No Blame** — If a record has an unexpected state, the audit trail helps diagnose what happened and when

## Design Seam: Soft-Delete

> `DeletedAt` is intentionally excluded from `IAuditable`. Soft-delete is a
> separate lifecycle concern handled by `ISoftDeletable` (US-125). Entities that
> need both implement both interfaces — the interceptors are independent and
> compose cleanly.

## Notes

- Dependencies are all shipped: US-050 (Unit of Work), US-107 (EF Entity Auto-Discovery), US-108 (Foundation Extraction)
- `IAuditable` is opt-in by interface, not mandatory on `Entity` base — keeps Frank flexible for diverse consumers
- App-level architecture tests enforce the "all entities must be auditable" policy
- **Demo:** Create a dog, update it — query the database and show `CreatedAt` and `ModifiedAt` populated automatically
