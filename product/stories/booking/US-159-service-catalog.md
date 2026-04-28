---
id: US-159
title: "Service Catalog"
epic: Booking
milestone: M3
status: backlog
domain: booking
vertical_slice: false
dependencies: []
---

# US-159: Service Catalog

## Intent

As a **business operator**, I need to define the services Camp Fit Fur Dogs
offers — daycare sessions, training classes, and grooming appointments — so
that owners can discover and book them.

## Value

The service catalog is the foundation of the booking domain. Every booking
references a service. Without a well-defined catalog, the system cannot
enforce capacity limits, pricing, prerequisites (e.g., vaccination
requirements), or scheduling rules. The catalog also drives the public-facing
service browsing experience.

## Acceptance Criteria

### Domain model
- [ ] `Service` entity with: Id, Name, Description, ServiceType (enum: Daycare, Training, Grooming), Duration, Capacity, Price, IsActive, Prerequisites
- [ ] `ServiceType` enum distinguishes booking patterns:
  - **Daycare** — half-day or full-day slots, high capacity, drop-in or recurring
  - **Training** — multi-session programs (e.g., 6-week course), limited capacity, enrollment-based
  - **Grooming** — individual appointments, 1:1 capacity, variable duration
- [ ] Services can be activated or deactivated without deletion
- [ ] Services can define prerequisites (e.g., "requires current rabies vaccination")
- [ ] Price is stored as a value object with amount and currency

### Data access
- [ ] `IServiceCatalogReader` returns active services with filtering by type
- [ ] `CreateServiceCommand` / `UpdateServiceCommand` manage the catalog
- [ ] Seed data includes at least one service per type for development and demo

### API
- [ ] `GET /api/services` returns active services (public, no auth required)
- [ ] `GET /api/services/{id}` returns service detail with prerequisites
- [ ] Admin endpoints for create/update are auth-protected (admin role — future)

## Emotional Guarantees

- **EG-01 No Surprises** — Service descriptions accurately represent what the owner is booking
- **EG-06 Explicit Risk** — Prerequisites are stated upfront, not discovered at booking time

## Legal Guarantees

- **LG-02 Transparent** — Pricing is displayed clearly before any booking commitment

## Notes

- No admin UI in this story — services are managed via API or seed data initially
- Admin portal (future story) will provide a UI for catalog management
- Consider: service categories or tags for filtering (e.g., "beginner," "advanced," "senior dogs")
- Consider: service images and marketing descriptions for the public catalog page
- **Demo:** Browse `/api/services` — see daycare, training, and grooming options with descriptions and pricing
