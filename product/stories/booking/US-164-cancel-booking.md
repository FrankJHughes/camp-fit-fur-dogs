---
id: US-164
title: "Cancel Booking"
epic: Booking
milestone: M3
status: backlog
domain: booking
vertical_slice: true
dependencies:
  - US-162
  - US-163
---

# US-164: Cancel Booking

## Intent

As an **owner**, I want to cancel a booking I no longer need so that the slot
is freed for other dogs and I am not charged for a service I won't use.

## Value

Life happens — plans change, dogs get sick, schedules shift. A frictionless
cancellation flow respects the owner's time and reduces no-shows by
encouraging proactive cancellation over ghosting. Freed capacity benefits the
business (the slot can be rebooked or offered to the waitlist).

## Acceptance Criteria

### Cancellation flow
- [ ] Owner can cancel from the "View My Bookings" page (US-163)
- [ ] Cancellation confirmation dialog explains the cancellation policy before proceeding
- [ ] Cancellation within the free window (e.g., 24+ hours before) is free — no charge
- [ ] Late cancellation (e.g., less than 24 hours) displays a clear warning about potential charges
- [ ] Cancellation policy (free window, late fee) is configurable per service
- [ ] Cancelled bookings update status to `Cancelled` and restore slot capacity atomically
- [ ] `BookingCancelled` domain event is raised

### Post-cancellation
- [ ] Confirmation message displays on the bookings page
- [ ] Cancellation notification sent via outbox (email/SMS per owner preference)
- [ ] If a waitlist exists for the freed slot, the next waitlisted owner is notified (US-166)

### Restrictions
- [ ] Past bookings cannot be cancelled
- [ ] Already-cancelled bookings show status but no cancel action
- [ ] Completed bookings show status but no cancel action

## Emotional Guarantees

- **EG-01 No Surprises** — The cancellation policy is stated before the action, not after
- **EG-02 No Blame** — Cancellation is treated as normal, not penalized with guilt
- **EG-03 Calm Protection** — The free cancellation window gives owners room to change plans without stress

## Legal Guarantees

- **LG-02 Transparent** — Cancellation terms are disclosed before booking and before cancellation

## Notes

- Depends on US-162 (Booking), US-163 (View Bookings)
- Payment/refund handling is a future story — this story covers the booking state change and capacity restoration
- Consider: grace period for accidental cancellation (undo within 5 minutes?)
- **Demo:** View upcoming booking, click Cancel, see the policy, confirm — booking is cancelled, slot is freed, notification received
