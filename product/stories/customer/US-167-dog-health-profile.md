---
id: US-167
title: "Dog Health Profile"
epic: Customer
milestone: M3
status: backlog
domain: customer
vertical_slice: true
dependencies:
  - US-018
  - US-029
---

# US-167: Dog Health Profile

## Intent

As an **owner**, I need to record my dog's health information — medical
conditions, allergies, dietary restrictions, and medications — so that Camp
Fit Fur Dogs staff can provide safe, informed care.

## Value

A dog with a peanut allergy getting the wrong treat is a liability nightmare.
A dog on seizure medication needs staff who know the protocol. Health profiles
transform Camp Fit Fur Dogs from "a place that watches your dog" to "a place
that *knows* your dog." This is both a safety requirement and a massive trust
builder — owners who see the health profile form think: "These people take
this seriously."

## Acceptance Criteria

### Domain model
- [ ] `DogHealthProfile` entity (or value object on Dog aggregate) with: MedicalConditions, Allergies, DietaryRestrictions, CurrentMedications, SpayNeuterStatus, Weight, DateOfBirth (age calculation), SpecialInstructions
- [ ] Each field is optional — owners fill in what they know
- [ ] `MedicalCondition` value object: Name, Severity (mild/moderate/severe), Notes
- [ ] `Allergy` value object: Allergen, Reaction, Severity
- [ ] `Medication` value object: Name, Dosage, Frequency, AdministrationInstructions

### UI
- [ ] Dog profile page (US-029) includes a "Health & Safety" section
- [ ] Health information is editable by the owner at any time
- [ ] Form uses clear, non-medical language (e.g., "What is your dog allergic to?" not "List known allergens")
- [ ] Critical health information (severe allergies, seizure conditions) is visually flagged with a prominent indicator
- [ ] A free-text "Special Instructions" field captures anything not covered by structured fields

### Safety
- [ ] Health profile data is visible to staff during check-in (future admin portal)
- [ ] Critical health flags are surfaced prominently — not buried in a long form
- [ ] Health data changes are auditable (timestamp of last update)

## Emotional Guarantees

- **EG-01 No Surprises** — Staff have the information they need before the dog arrives
- **EG-02 No Blame** — The form never judges ("no wrong answers — every detail helps us care better")
- **EG-03 Calm Protection** — Critical health information is impossible to miss during check-in

## Legal Guarantees

- **LG-01 Accessible** — Health form is keyboard navigable and screen reader compatible
- **LG-04 Controllable** — Health data is included in data export (US-153) and deletion cascade (US-153)
- **LG-05 Secure** — Health information is treated as sensitive personal data

## Notes

- Depends on US-018 (Dog aggregate), US-029 (View Dog Profile)
- Health profile is part of the Dog aggregate or a closely related entity — it travels with the dog
- Consider: should health profile completion be required before first booking? (prerequisite check in US-162)
- Consider: printable health summary for staff use during the day
- **Demo:** Navigate to your dog's profile, fill in the health section — add "chicken allergy (severe)" and "takes Apoquel twice daily" — see the data saved and flagged
