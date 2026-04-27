---
id: US-170
title: "Behavior Assessment"
epic: Customer
milestone: M3+
status: backlog
domain: customer
vertical_slice: true
dependencies:
  - US-018
  - US-167
---

# US-170: Behavior Assessment

## Intent

As an **owner**, I need to describe my dog's temperament, play style, and
behavioral tendencies so that Camp Fit Fur Dogs can match my dog with
compatible playmates and anticipate potential issues.

## Value

Group play is the most rewarding — and most risky — part of daycare. A
reactive dog placed with a boisterous puppy is a bite incident waiting to
happen. A shy dog overwhelmed by a large group is a traumatized animal.
Behavior assessments let staff make informed grouping decisions, spot warning
signs early, and provide the right environment for every dog's personality.

## Acceptance Criteria

### Owner-submitted assessment
- [ ] Dog profile includes a "Behavior & Temperament" section
- [ ] Owner completes a structured questionnaire covering:
  - Energy level (low / moderate / high)
  - Play style (solo / parallel / interactive / rough)
  - Comfort with other dogs (comfortable / selective / nervous / reactive)
  - Comfort with people (comfortable / shy / anxious / resource-guarding)
  - Known triggers (e.g., loud noises, food aggression, leash reactivity)
  - Previous daycare or group experience (none / some / experienced)
  - Any bite history (required disclosure — yes/no with details if yes)
- [ ] Questionnaire uses friendly, non-judgmental language throughout
- [ ] A free-text field captures anything the structured questions don't cover
- [ ] Assessment is required before first group service booking (daycare or training)

### Staff assessment (future admin portal)
- [ ] Staff can add their own assessment notes after the dog's trial day
- [ ] Staff assessment fields: Observed energy level, play compatibility, recommended group size, behavioral notes, red flags
- [ ] Staff assessment is visible alongside the owner assessment (both perspectives)

### Grouping support
- [ ] Behavior data is queryable: filter dogs by energy level, play style, comfort level
- [ ] Behavior flags (reactive, bite history, resource guarding) are prominently surfaced during check-in

## Emotional Guarantees

- **EG-01 No Surprises** — Staff know what to expect before the dog arrives
- **EG-02 No Blame** — Bite history disclosure is framed as safety, not judgment ("this helps us keep everyone safe")
- **EG-03 Calm Protection** — Dogs are grouped with compatible playmates
- **EG-04 Inclusive by Default** — Every temperament is accommodated — there is no "bad dog," only a dog that needs the right environment

## Legal Guarantees

- **LG-01 Accessible** — Questionnaire meets WCAG 2.1 AA
- **LG-05 Secure** — Behavior data is sensitive (especially bite history) and access-controlled

## Notes

- Depends on US-018 (Dog), US-167 (Health Profile)
- Bite history is legally significant — California has strict liability for dog bites. Knowing and documenting bite history protects the business.
- Consider: a trial day requirement before regular group daycare (common industry practice)
- Consider: periodic reassessment (every 6 months or after an incident)
- **Demo:** Fill in the behavior questionnaire — see the assessment summary on the dog profile — try to book group daycare without completing it — see the prerequisite prompt
