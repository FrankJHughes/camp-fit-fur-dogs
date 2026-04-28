---
id: US-168
title: "Vaccination Tracking"
epic: Customer
milestone: M3
status: backlog
domain: customer
vertical_slice: true
dependencies:
  - US-018
  - US-159
  - US-167
---

# US-168: Vaccination Tracking

## Intent

As an **owner**, I need to record my dog's vaccination status and upload proof
so that Camp Fit Fur Dogs can verify my dog meets the health requirements for
services.

## Value

Vaccination compliance is non-negotiable for any group dog care facility.
Unvaccinated dogs in a group setting risk outbreaks of parvovirus, bordetella,
and canine influenza — any of which could shut down the business and harm
animals. Tracking vaccinations digitally eliminates the paper shuffle, gives
owners a clear checklist, and enables the booking system to enforce
prerequisites automatically.

## Acceptance Criteria

### Required vaccinations
- [ ] The system defines a configurable list of required vaccinations per service type
- [ ] Default required vaccinations:
  - Rabies (required by California law for all dogs)
  - DHPP (Distemper, Hepatitis, Parainfluenza, Parvovirus)
  - Bordetella (required for group daycare and training)
  - Canine Influenza (recommended for group settings)
- [ ] Each vaccination requirement includes: vaccine name, whether it is mandatory or recommended, and validity period (e.g., rabies = 1 year or 3 years)

### Owner vaccination records
- [ ] Dog health profile includes a "Vaccinations" section
- [ ] Owner can add vaccination records: vaccine name, date administered, expiration date, veterinarian name
- [ ] Owner can upload proof of vaccination (photo or PDF of vet records)
- [ ] File uploads are stored securely and associated with the vaccination record
- [ ] Vaccination status is calculated per vaccine: Current, Expiring Soon (within 30 days), Expired, Missing
- [ ] A visual compliance dashboard shows the dog's overall vaccination status at a glance (all green = good to go)

### Prerequisite enforcement
- [ ] The booking flow (US-162) checks vaccination compliance before allowing a booking
- [ ] If a required vaccination is missing or expired, the booking is blocked with a clear message: "Your dog needs a current [vaccine name] to attend [service]. Update your records here."
- [ ] Expiring-soon vaccinations generate a reminder notification (via outbox) to the owner

### Notifications
- [ ] Vaccination expiration reminder sent 30 days before expiration
- [ ] Reminder includes: vaccine name, expiration date, and a link to update records

## Emotional Guarantees

- **EG-01 No Surprises** — Owners know exactly what's needed before booking
- **EG-02 No Blame** — Expiration reminders are helpful, not scolding
- **EG-03 Calm Protection** — Every dog in a group session meets the same health standard

## Legal Guarantees

- **LG-01 Accessible** — Vaccination checklist and file upload meet WCAG 2.1 AA
- **LG-05 Secure** — Uploaded vet records are access-controlled and encrypted at rest

## Notes

- Depends on US-018 (Dog), US-159 (Service Catalog — prerequisites), US-167 (Health Profile)
- California law requires rabies vaccination for all dogs — this is a legal requirement, not just a business policy
- File upload: support JPEG, PNG, and PDF; max file size TBD (5MB reasonable for phone photos of vet records)
- Consider: vet verification workflow (staff marks vaccination as verified vs. self-reported) — future story
- **Demo:** Go to dog profile, add rabies vaccination with date and upload a photo of the vet certificate — see the compliance dashboard go green for rabies
