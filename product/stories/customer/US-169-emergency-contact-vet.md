---
id: US-169
title: "Emergency Contact & Vet Info"
epic: Customer
milestone: M3
status: backlog
domain: customer
vertical_slice: true
dependencies:
  - US-018
  - US-167
---

# US-169: Emergency Contact & Vet Info

## Intent

As an **owner**, I need to provide emergency contacts and my dog's veterinarian
information so that Camp Fit Fur Dogs can reach someone and get veterinary
guidance if an emergency occurs while my dog is in their care.

## Value

If a dog collapses, has a seizure, or is injured during play, staff need to
act immediately. Knowing the dog's vet and having an emergency contact who can
authorize treatment is the difference between a controlled response and a
crisis. This information is typically required by pet care facility regulations
and insurance policies.

## Acceptance Criteria

### Emergency contacts
- [ ] Each dog profile includes at least one emergency contact (in addition to the owner)
- [ ] Emergency contact fields: Name, Relationship, Phone Number, Can Authorize Veterinary Treatment (boolean)
- [ ] Owner can add up to 3 emergency contacts per dog
- [ ] At least one emergency contact with treatment authorization is required before first booking
- [ ] Phone numbers are validated (E.164 format, consistent with US-149)

### Veterinarian information
- [ ] Each dog profile includes a primary veterinarian section
- [ ] Vet fields: Clinic Name, Veterinarian Name, Phone Number, Address (optional), Notes
- [ ] Vet information is optional but recommended — a prompt encourages completion

### Booking prerequisite
- [ ] The booking flow (US-162) checks that at least one authorized emergency contact exists
- [ ] Missing emergency contact blocks booking with a clear message and link to add one

### Staff access
- [ ] Emergency contact and vet info are prominently accessible during check-in (future admin portal)
- [ ] Emergency info is printable or exportable for daily staff reference sheets

## Emotional Guarantees

- **EG-01 No Surprises** — Owners understand why this information is needed (safety framing, not bureaucracy)
- **EG-03 Calm Protection** — Staff can respond to emergencies with the right contacts immediately
- **EG-06 Explicit Risk** — Treatment authorization is an explicit, deliberate choice

## Legal Guarantees

- **LG-01 Accessible** — Emergency contact form meets WCAG 2.1 AA
- **LG-04 Controllable** — Emergency contact data is included in export and deletion
- **LG-05 Secure** — Emergency contact data is treated as sensitive personal data

## Notes

- Depends on US-018 (Dog), US-167 (Health Profile)
- Treatment authorization is a critical legal concept — the "Can Authorize Veterinary Treatment" checkbox should link to a brief explanation of what it means
- Consider: emergency action consent form (signed digitally) — may require legal review
- Consider: multiple dogs sharing the same emergency contacts (owner-level vs. dog-level)
- **Demo:** Add an emergency contact and vet for your dog — try to book without one — see the prerequisite message — add the contact — booking succeeds
