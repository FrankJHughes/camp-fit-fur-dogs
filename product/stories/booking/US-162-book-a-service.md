---
id: US-162
title: "Book a Service"
epic: Booking
milestone: M3
status: backlog
domain: booking
vertical_slice: true
dependencies:
  - US-027
  - US-018
  - US-159
  - US-160
  - US-161
---

# US-162: Book a Service

## Intent

As an **owner**, I want to book a daycare session, training class, or grooming
appointment for my dog so that my dog can attend the service.

## Value

This is the core revenue-generating action of the business. Every booking
represents a commitment from an owner and a capacity allocation from the
business. The booking flow must be simple, fast, and confidence-inspiring —
any friction here directly impacts revenue.

## Acceptance Criteria

### Booking flow
- [ ] Owner must be authenticated to book (redirect to login if not)
- [ ] Owner selects a service and available time slot (pre-filled if coming from US-161)
- [ ] Owner selects which dog the booking is for (from their registered dogs)
- [ ] System validates prerequisites: dog has required vaccinations (US-168), dog is not already booked for that slot
- [ ] If prerequisites are not met, a clear message explains what is needed with links to resolve (e.g., "Update vaccination records")
- [ ] Booking summary is displayed before confirmation: service name, date, time, dog name, price
- [ ] Owner confirms the booking with a single action
- [ ] Booking is created atomically — slot capacity is decremented in the same transaction

### Domain model
- [ ] `Booking` aggregate with: Id, CustomerId, DogId, ServiceId, ScheduleSlotDate, StartTime, EndTime, Status (Confirmed, Cancelled, Completed, NoShow), BookedAt, CancelledAt
- [ ] `BookingCreated` domain event is raised on successful booking
- [ ] Booking enforces invariants: no double-booking the same dog for overlapping times, slot capacity not exceeded
- [ ] Concurrency handling: if two owners book the last slot simultaneously, one succeeds and one receives a "slot no longer available" message

### Post-booking
- [ ] Confirmation page displays booking details with a "View My Bookings" link
- [ ] `BookingCreated` domain event triggers a confirmation notification via the outbox (US-143)
- [ ] Confirmation notification respects the owner's channel preference (email, SMS, or both)

## Emotional Guarantees

- **EG-01 No Surprises** — The booking summary matches exactly what was selected
- **EG-02 No Blame** — Prerequisite failures explain what's needed, not what's wrong
- **EG-03 Calm Protection** — Double-booking and overbooking are impossible

## Legal Guarantees

- **LG-01 Accessible** — Booking flow is fully keyboard navigable
- **LG-02 Transparent** — Price is confirmed before the booking is placed

## Notes

- Depends on US-027 (Customer), US-018 (Dog), US-159 (Catalog), US-160 (Schedule), US-161 (Browse)
- Payment is NOT part of this story — bookings are confirmed without payment initially. Payment integration (future story) adds a payment step before confirmation.
- Concurrency: use optimistic concurrency on the slot capacity or a database-level constraint
- Consider: booking for multiple dogs in one flow (owner with 3 dogs)
- **Demo:** Browse services, select Saturday daycare, pick your dog, confirm — see the confirmation page and receive an email/SMS
