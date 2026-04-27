---
id: US-160
title: "Schedule & Availability"
epic: Booking
milestone: M3
status: backlog
domain: booking
vertical_slice: false
dependencies:
  - US-159
---

# US-160: Schedule & Availability

## Intent

As a **business operator**, I need to define when each service is available —
including days, time slots, and capacity — so that owners can only book times
that the business can actually serve.

## Value

Availability management prevents overbooking and ensures operational capacity
matches demand. A daycare session with 20 dog capacity and a grooming slot
with 1 dog capacity have fundamentally different scheduling needs. The
schedule system must be flexible enough to handle recurring weekly patterns,
holiday closures, and one-off capacity adjustments.

## Acceptance Criteria

### Domain model
- [ ] `Schedule` entity with: Id, ServiceId, DayOfWeek, StartTime, EndTime, Capacity, IsRecurring, EffectiveFrom, EffectiveTo
- [ ] Recurring schedules define weekly patterns (e.g., "Daycare available Mon-Fri 7AM-6PM, capacity 20")
- [ ] One-off schedule overrides for holidays, special events, or temporary closures
- [ ] `ScheduleSlot` value object represents a specific bookable instance (date + time + remaining capacity)
- [ ] Capacity is tracked per slot — decremented on booking, incremented on cancellation

### Availability queries
- [ ] `IAvailabilityReader` returns available slots for a given service and date range
- [ ] Available slots include: date, start time, end time, total capacity, remaining capacity
- [ ] Fully booked slots are excluded from availability results (or marked as full)
- [ ] Past slots are excluded from availability results

### API
- [ ] `GET /api/services/{id}/availability?from={date}&to={date}` returns available slots
- [ ] Response includes remaining capacity per slot
- [ ] Admin endpoints for schedule management are auth-protected

## Emotional Guarantees

- **EG-01 No Surprises** — If a slot shows as available, it is bookable
- **EG-03 Calm Protection** — Overbooking is prevented by the system, not by manual tracking

## Legal Guarantees

- **LG-01 Accessible** — Date and time pickers are keyboard navigable and screen reader compatible

## Notes

- Schedule management is via API or seed data initially — admin UI is a future story
- Seed data: Mon-Fri daycare 7AM-6PM (20 dogs), Sat morning training 9AM-11AM (8 dogs), Tue/Thu grooming 10AM-4PM (1 dog per hour)
- Consider: timezone handling — all times stored in UTC, displayed in business local time
- Consider: buffer time between grooming appointments for cleanup
- **Demo:** Query availability for daycare next week — see slots with remaining capacity
