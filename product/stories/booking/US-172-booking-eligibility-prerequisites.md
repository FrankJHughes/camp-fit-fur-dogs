---
id: US-172
title: "Booking Eligibility & Prerequisites Management"
epic: Booking
milestone: M3
status: backlog
domain: booking
vertical_slice: true
dependencies:
  - US-159
  - US-162
  - US-167
  - US-168
  - US-169
  - US-170
---

# US-172: Booking Eligibility & Prerequisites Management

## Intent

As a **business operator**, I need to define and maintain the health and safety
prerequisites for each service so that only dogs who meet those requirements
can be booked — and as an **owner**, I need to understand clearly and kindly
what my dog needs before they can attend.

## Value

A dog daycare facility has a duty of care. Letting an unvaccinated dog into
group play, or a reactive dog into an open room without an assessment, puts
every animal and the business at risk. But the way this is communicated matters
enormously. A cold "Your dog is ineligible" destroys trust. A warm "Here's
what Bailey needs to get started — we're almost there!" builds it.

This story creates two capabilities:
1. **Management control** — operators configure what's required per service,
   and those rules evolve as the business learns
2. **Owner clarity** — a single, consolidated view of what the dog needs,
   what's done, and what's left, with warm guidance at every step

## Acceptance Criteria

### Prerequisite configuration (management side)

- [ ] Each service (US-159) has a configurable list of prerequisites
- [ ] Prerequisite types include:
  - **Vaccination** — specific vaccine required, with validity check (e.g., "Current Rabies")
  - **Health profile** — health profile must be completed (US-167)
  - **Emergency contact** — at least one authorized emergency contact (US-169)
  - **Behavior assessment** — behavior questionnaire completed (US-170)
  - **Custom** — free-text prerequisite with manual verification (e.g., "Spay/neuter required for dogs over 6 months")
- [ ] Each prerequisite has: type, description, required (boolean), and service applicability
- [ ] Management can add, update, or remove prerequisites per service via admin API
- [ ] Changes to prerequisites take effect immediately for new bookings; existing confirmed bookings are not retroactively invalidated
- [ ] Default prerequisites are seeded per service type:
  - **Daycare**: rabies, DHPP, bordetella, emergency contact, behavior assessment
  - **Training**: rabies, DHPP, emergency contact, behavior assessment
  - **Grooming**: rabies, emergency contact
- [ ] Admin UI for prerequisite management (future admin portal story provides the interface; this story provides the API and data model)

### Eligibility assessment (system side)

- [ ] `IEligibilityAssessor` service evaluates a dog's readiness for a specific service
- [ ] Assessment returns a structured result per prerequisite: Met, Not Met, Expiring Soon, or Not Applicable
- [ ] Overall eligibility is: Eligible (all required prerequisites met), Partially Ready (some met), or Not Yet Ready (critical requirements missing)
- [ ] Assessment is queryable per dog per service: `GET /api/dogs/{dogId}/eligibility/{serviceId}`
- [ ] Assessment is checked atomically during the booking flow (US-162) — a dog cannot be booked if overall status is Not Yet Ready
- [ ] Concurrency: prerequisite changes between eligibility check and booking confirmation are caught by a re-check at commit time

### Owner-facing eligibility experience (customer side)

- [ ] Dog profile includes a "Ready to Book" section showing eligibility status per service type
- [ ] Each service shows a checklist with clear, friendly status indicators:
  - ✅ Met — "Rabies vaccination — up to date"
  - ⚠️ Expiring — "Bordetella expires in 12 days — time to schedule a booster!"
  - ❌ Needed — "We'll need a behavior assessment before Bailey's first group play — it helps us find the perfect playmates!"
- [ ] Each unmet prerequisite includes a direct action link (e.g., "Add vaccination record," "Complete behavior questionnaire," "Add an emergency contact")
- [ ] When all prerequisites are met, a celebratory confirmation: "Bailey is all set for Daycare! 🎉 Ready to book?"
- [ ] The booking flow (US-162) shows the eligibility checklist inline if prerequisites are not met, rather than a generic error message
- [ ] Messaging never uses words like "ineligible," "denied," "rejected," or "failed"

### Approved language patterns

| ❌ Never say | ✅ Say instead |
|-------------|---------------|
| "Your dog is ineligible" | "Bailey needs a couple more things before daycare — almost there!" |
| "Booking denied" | "Let's get Bailey ready first — here's what's needed" |
| "Requirements not met" | "A few items to check off — we want to make sure Bailey has the best experience" |
| "Failed prerequisite check" | "Bailey's bordetella vaccination has expired — a quick booster and you're all set!" |
| "Access restricted" | "Once these are in place, Bailey can join the fun!" |

### Notifications

- [ ] When a prerequisite status changes (e.g., vaccination expires), the owner is notified via their preferred channel with warm, specific messaging
- [ ] When all prerequisites for a previously-blocked service become met, a proactive notification: "Great news — Bailey is now eligible for Daycare! Ready to book?"
- [ ] Expiring-soon notifications are sent at configurable intervals (e.g., 30 days and 7 days before expiration)

## Emotional Guarantees

- **EG-01 No Surprises** — The eligibility checklist is visible on the dog profile at all times, not just during booking
- **EG-02 No Blame** — Unmet prerequisites are framed as next steps, never as failures or faults
- **EG-03 Calm Protection** — The system ensures every dog in a group session meets the same safety standard
- **EG-04 Inclusive by Default** — Every dog can eventually participate — the path is clear and achievable
- **EG-05 Respect for Attention** — One consolidated checklist, not scattered requirements discovered one at a time

## Legal Guarantees

- **LG-01 Accessible** — Eligibility checklist and action links meet WCAG 2.1 AA
- **LG-02 Transparent** — All prerequisites are visible before any booking attempt, not discovered at checkout
- **LG-05 Secure** — Health records used for eligibility are access-controlled

## Design Seam: Prerequisite Ownership

> Prerequisites live on the Service aggregate (US-159) — they define what a
> service requires. Eligibility lives on the intersection of Dog and Service —
> it evaluates whether a specific dog meets a specific service's requirements.
> The `IEligibilityAssessor` is a domain service, not an entity, because it
> cross-cuts aggregates (Dog, Service, HealthProfile, VaccinationRecord,
> EmergencyContact, BehaviorAssessment).
>
> This separation means:
> - Changing a prerequisite on a service doesn't touch dog data
> - Updating a vaccination on a dog doesn't touch service data
> - Eligibility is computed at query time, not stored — always fresh

## Notes

- Depends on US-159 (Service Catalog), US-162 (Book a Service), US-167 (Health Profile), US-168 (Vaccination Tracking), US-169 (Emergency Contact), US-170 (Behavior Assessment)
- The approved language patterns table should be referenced during frontend copy review — consider extracting it into a tone-of-voice guide
- Custom prerequisites with manual verification require a staff workflow (future admin portal)
- Consider: prerequisite groups (e.g., "Group Play Requirements" vs. "Individual Service Requirements") for cleaner organization
- Consider: a "Getting Started" onboarding flow that walks new owners through all prerequisites before their first booking
- **Demo:** View Bailey's profile — see the "Ready to Book" checklist showing 3/5 prerequisites met for Daycare. Click "Add vaccination record" — add bordetella. Checklist updates to 4/5. Complete the behavior assessment. Checklist shows 5/5 with "Bailey is all set for Daycare! 🎉" — click through to book.
