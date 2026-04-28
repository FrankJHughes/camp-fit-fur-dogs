# Covey Quadrant Prioritization

Inspired by Stephen Covey's *The 7 Habits of Highly Effective People*, every
backlog story is labeled with **urgency** and **importance** to guide sprint
planning decisions. The combination maps to one of four quadrants that
determine how the story should be treated.

---

## The Four Quadrants

```
                    URGENT                  NOT URGENT
             ┌─────────────────────┬─────────────────────┐
             │                     │                     │
  IMPORTANT  │   Q1: DO FIRST      │   Q2: SCHEDULE      │
             │                     │                     │
             │   Legal blockers,   │   Architecture,     │
             │   launch gates,     │   security layers,  │
             │   critical path     │   core business     │
             │   dependencies      │   features          │
             │                     │                     │
             ├─────────────────────┼─────────────────────┤
             │                     │                     │
  NOT        │   Q3: DELEGATE      │   Q4: DEFER         │
  IMPORTANT  │                     │                     │
             │   Convenience       │   Nice-to-have      │
             │   features that     │   enhancements      │
             │   feel expected     │   with low demand    │
             │   but are not       │   or no timeline     │
             │   critical path     │   pressure           │
             │                     │                     │
             └─────────────────────┴─────────────────────┘
```

---

## Definitions

### Urgency

How time-sensitive is this story? Does it block other work or have external
deadlines?

| Level | Criteria |
|-------|----------|
| **high** | Blocks other stories in the current or next milestone; has an external deadline; is legally required before a milestone gate |
| **medium** | Enables future work but does not block the current sprint; has a soft timeline |
| **low** | No time pressure; can be scheduled at any point without impacting other work |

### Importance

How much does this story matter to the product's mission, revenue, legal
posture, or user trust?

| Level | Criteria |
|-------|----------|
| **high** | Core business capability; legal requirement; directly affects revenue, safety, or user trust; architectural foundation that shapes future work |
| **medium** | Enhances the product meaningfully but is not mission-critical; improves efficiency, polish, or operational capability |
| **low** | Nice-to-have; adds marginal value; users would not notice its absence |

### Quadrant mapping

| Urgency | Importance | Quadrant | Action |
|---------|------------|----------|--------|
| high | high | **Q1: Do First** | Pull into the next available sprint. These are the critical path. |
| low | high | **Q2: Schedule** | Plan deliberately. These build long-term value. Resist the temptation to deprioritize them in favor of Q1 — Covey warns that neglecting Q2 creates future Q1 crises. |
| high | low | **Q3: Delegate** | Timebox. Do the minimum viable version or defer if Q2 work is more valuable. In a solo project, "delegate" means "simplify." |
| low | low | **Q4: Defer** | Park in the backlog. Revisit only when Q1 and Q2 are clear. |

---

## Frontmatter Fields

Stories include urgency and importance in their YAML frontmatter:

```yaml
---
id: US-162
title: "Book a Service"
urgency: high
importance: high
covey_quadrant: Q1
milestone: M3
# ... other fields
---
```

The `covey_quadrant` field is derived from urgency × importance and is
included for quick filtering and dashboard reporting. It should always be
consistent with the urgency and importance values.

---

## Sprint Planning with Covey Quadrants

The quadrant mix in a sprint should follow Covey's guidance:

1. **Always include Q2 work.** If a sprint is 100% Q1, something is wrong —
   you are firefighting instead of building. Aim for at least 40% Q2.
2. **Q1 gets priority but not monopoly.** Pull Q1 stories first, but leave
   room for Q2.
3. **Q3 is seductive.** Convenience features feel urgent because users
   expect them (social login, email templates). Resist unless Q2 is healthy.
4. **Q4 is a signal.** If a story stays Q4 for 3+ sprints, consider retiring
   it. It is not earning its place in the backlog.

---

## Reassessment

Urgency and importance are not permanent. A story's quadrant can change:

- A legal deadline approaches → urgency increases
- A competitor launches a feature → importance shifts
- A dependency is resolved → urgency decreases
- User feedback reveals low demand → importance decreases

Reassess during sprint planning. If a story's quadrant changes, update the
frontmatter and note the reason in the story's changelog.
