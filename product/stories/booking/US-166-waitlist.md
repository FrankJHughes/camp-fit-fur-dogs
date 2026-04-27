---
id: US-166
title: "Waitlist"
epic: Booking
milestone: M3+
status: backlog
domain: booking
vertical_slice: true
dependencies:
  - US-162
  - US-164
---

# US-166: Waitlist

## Intent

As an **owner**, I want to join a waitlist when my preferred slot is full so
that I am automatically notified if a spot opens up.

## Value

Waitlists convert lost demand into future bookings. Without a waitlist, a
fully booked slot is a dead end — the owner leaves and may not come back.
With a waitlist, a cancellation (US-164) automatically triggers outreach to
the next interested owner, maximizing capacity utilization.

## Acceptance Criteria

- [ ] When a slot is fully booked, the "Book Now" button is replaced with "Join Waitlist"
- [ ] Joining the waitlist records: CustomerId, DogId, ServiceId, SlotDate, JoinedAt, Position
- [ ] Waitlist is ordered by join time (FIFO)
- [ ] Owner can view their waitlist entries from the bookings page
- [ ] Owner can leave the waitlist at any time
- [ ] When a booking is cancelled (US-164) and a waitlist exists for that slot, the first waitlisted owner is notified via their preferred channel
- [ ] Notification includes: "A spot opened up! Book now before it's taken" with a direct booking link
- [ ] Waitlist offer expires after a configurable window (e.g., 4 hours) — if not claimed, the next waitlisted owner is notified
- [ ] Expired waitlist offers are logged and the slot remains open for general booking

## Emotional Guarantees

- **EG-01 No Surprises** — Owners know their waitlist position and will be notified if a spot opens
- **EG-03 Calm Protection** — The time-limited offer prevents indefinite holds while giving the owner a fair window

## Legal Guarantees

- **LG-03 Consensual** — Waitlist notifications use the owner's consented channels

## Notes

- Depends on US-162 (Booking), US-164 (Cancel Booking)
- FIFO ordering is fairest but consider: priority for frequent customers (future enhancement)
- Consider: auto-book vs. notify-and-wait (auto-book removes choice; notify-and-wait is more respectful)
- **Demo:** Try to book a full slot — see "Join Waitlist," join — another owner cancels — receive notification with a booking link — click and book
