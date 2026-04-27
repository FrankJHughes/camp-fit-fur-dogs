---
id: US-163
title: "View My Bookings"
epic: Booking
milestone: M3
status: backlog
domain: booking
vertical_slice: true
dependencies:
  - US-162
---

# US-163: View My Bookings

## Intent

As an **owner**, I want to see all my upcoming and past bookings so that I
can keep track of my dog's schedule and review history.

## Value

Visibility into bookings reduces anxiety ("Did I book that?") and enables
self-service management (cancel, reschedule). A well-designed bookings page
also drives rebooking — owners who see their history are more likely to
book again.

## Acceptance Criteria

- [ ] Bookings page is accessible from the owner's profile/dashboard
- [ ] Upcoming bookings are displayed prominently with: service name, date, time, dog name, status
- [ ] Past bookings are displayed in a separate section with the same details
- [ ] Upcoming bookings show a "Cancel" action (navigates to US-164)
- [ ] Bookings are sorted by date (nearest first for upcoming, most recent first for past)
- [ ] Empty state shows a friendly message with a link to browse services (US-161)
- [ ] Page supports pagination or infinite scroll for owners with long history
- [ ] Each booking links to the service detail for reference

## Emotional Guarantees

- **EG-01 No Surprises** — Booking status is always clear and current
- **EG-05 Respect for Attention** — The page is scannable; upcoming bookings are immediately visible

## Legal Guarantees

- **LG-01 Accessible** — Booking list and actions meet WCAG 2.1 AA
- **LG-04 Controllable** — Booking data is included in data export (US-153)

## Notes

- Depends on US-162 (Book a Service)
- Consider: calendar view option alongside list view
- Consider: filter by dog (owners with multiple dogs)
- **Demo:** View bookings page — see upcoming grooming appointment for Thursday, past daycare sessions from last week
