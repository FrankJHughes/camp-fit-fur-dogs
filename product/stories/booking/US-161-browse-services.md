---
id: US-161
title: "Browse Available Services"
epic: Booking
milestone: M3
status: backlog
domain: booking
vertical_slice: true
dependencies:
  - US-103
  - US-159
  - US-160
---

# US-161: Browse Available Services

## Intent

As an **owner**, I want to browse available services and see when they are
offered so that I can decide what to book for my dog.

## Value

This is the front door to the booking flow. Owners need to understand what
services are available, what they cost, what prerequisites exist, and when
slots are open — all before committing to a booking. A clear, inviting
service browsing experience reduces friction and builds confidence.

## Acceptance Criteria

- [ ] Services page displays all active services from the catalog (US-159)
- [ ] Services are grouped or filterable by type (Daycare, Training, Grooming)
- [ ] Each service card shows: name, description, price, duration, and a "View Availability" action
- [ ] Selecting a service shows available dates and time slots (US-160)
- [ ] Available slots show remaining capacity (e.g., "3 spots left")
- [ ] Fully booked slots are visually distinguished but still visible (drives waitlist interest)
- [ ] Service prerequisites are displayed before the booking action (e.g., "Requires current rabies vaccination")
- [ ] "Book Now" button navigates to the booking flow (US-162) with the selected service and slot pre-filled
- [ ] Page is responsive — works on mobile (owners booking on the go)
- [ ] No authentication required to browse — booking requires login

## Emotional Guarantees

- **EG-01 No Surprises** — Pricing, prerequisites, and availability are visible before any commitment
- **EG-05 Respect for Attention** — The page is scannable; owners can find what they need quickly

## Legal Guarantees

- **LG-01 Accessible** — Service cards, filters, and date pickers meet WCAG 2.1 AA
- **LG-02 Transparent** — Pricing is displayed before any action is taken

## Notes

- Depends on US-103 (Next.js frontend), US-159 (Service Catalog), US-160 (Schedule & Availability)
- Public page — no auth required to browse (encourages exploration)
- Consider: calendar view vs. list view for availability
- Consider: search by date ("What's available this Saturday?")
- **Demo:** Visit the services page, filter by Grooming, select a service, see available slots for next week, click "Book Now"
