---
id: US-125
title: "Soft-Delete & Restore Dog"
epic: Customer
milestone: M1+
status: backlog
domain: customer
vertical_slice: true
dependencies:
  - US-032
  - US-050
  - US-107
  - US-108
---

# US-125: Soft-Delete & Restore Dog

## Intent

As an **owner**, I should be able to restore a dog I previously removed so that
accidental removals don't cause permanent data loss.

## Value

Hard delete (US-032) is the simplest correct implementation today, but real users
make mistakes. Soft-delete adds a safety net — removed dogs are hidden but
recoverable within a grace period, reducing support burden and owner anxiety.

The soft-delete mechanism itself is implemented as a SharedKernel capability
(`ISoftDeletable` + interceptor + global query filter), following the same
extraction pattern as US-108. Any future SharedKernel consumer gets soft-delete
for free. The restore UX, grace period policy, and purge strategy remain
app-specific to Camp Fit Fur Dogs.

## Acceptance Criteria

### SharedKernel (the capability)
- [ ] `ISoftDeletable` interface in `SharedKernel/Domain/` declares a nullable `DeletedAt` property
- [ ] `SoftDeleteInterceptor` in `SharedKernel/Persistence/` intercepts `Deleted` entity state on `ISoftDeletable` entities and converts it to `Modified` with `DeletedAt` set to UTC now
- [ ] Interceptor ignores entities that do not implement `ISoftDeletable`
- [ ] Global query filter convention automatically applies `WHERE DeletedAt IS NULL` to all `ISoftDeletable` entities
- [ ] Query filter can be bypassed explicitly (e.g., `IgnoreQueryFilters()`) for admin or restore scenarios
- [ ] Interceptor and query filter auto-register via SharedKernel DI conventions
- [ ] SharedKernel unit tests verify delete interception, query filtering, and filter bypass
- [ ] SharedKernel unit tests verify non-soft-deletable entities are unaffected

### Camp Fit Fur Dogs (the consumer)
- [ ] `Dog` aggregate implements `ISoftDeletable`
- [ ] EF migration adds `DeletedAt` column to the Dogs table (nullable, default null)
- [ ] Removing a dog (US-032) now sets `DeletedAt` instead of physically deleting the row
- [ ] Soft-deleted dogs no longer appear in any owner-facing views (dog list, profiles)
- [ ] Owner can view a list of recently removed dogs within a configurable grace period
- [ ] Owner can restore a soft-deleted dog, returning it to full visibility (clears `DeletedAt`)
- [ ] After the grace period expires, soft-deleted dogs are permanently purged (or remain archived per policy decision)
- [ ] Existing US-032 Remove Dog flow is unchanged from the owner's perspective — only the persistence mechanism changes

## Emotional Guarantees

- **EG-01 No Surprises** — Restoration is discoverable, not hidden
- **EG-03 Calm Protection** — Mistakes are recoverable; anxiety is reduced
- **EG-06 Explicit Risk** — Grace period and permanence are stated clearly

## Design Seam: Audit Timestamps

> `ISoftDeletable` is independent of `IAuditable` (US-127). An entity can be
> soft-deletable without being auditable and vice versa. If `Dog` implements both
> (recommended), it gets `CreatedAt`, `ModifiedAt` from `IAuditable` and `DeletedAt`
> from `ISoftDeletable` — two interceptors, composing independently.

## Notes

- Dependencies are all shipped: US-032 (Remove Dog), US-050 (Unit of Work), US-107 (EF Entity Auto-Discovery), US-108 (Foundation Extraction)
- The old `IsRemoved` flag is replaced by `DeletedAt IS NOT NULL` — single source of truth
- Consider: should the grace period be configurable per-owner or system-wide?
- Consider: purge strategy — background job vs. on-read cleanup
- **Demo:** Remove a dog, confirm it vanishes from the list, navigate to "Recently Removed," restore it — dog reappears
