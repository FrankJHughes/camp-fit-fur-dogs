# Emotional Guarantees

Emotional Guarantees are the product's promises about how the system makes customers **feel**. They are not features — they are quality attributes enforced as acceptance criteria on every functional story.

Every story file references the EG codes that apply. This document is the single source of truth for what each guarantee means, why it matters, and the concrete acceptance criteria that make it testable.

---

## EG-01 — The System Never Surprises the Customer

**Intent:** As a Customer, I want to always understand where I am in a process, so I never feel lost or anxious about what comes next.

**Value:** Eliminates the cognitive burden of navigating unfamiliar workflows. When the customer always knows their position, what's next, and how to go back, the system feels safe to explore.

### Acceptance Criteria

- Multi-step workflows display a progress indicator showing current step, total steps, and step labels.
- The customer can navigate backward to any previous step without losing entered data.
- Every screen provides a clear primary action and an obvious way to cancel or exit.
- Confirmation screens summarize all inputs before final submission.
- After completing a workflow, the system provides clear confirmation of what happened and what to expect next.

---

## EG-02 — The System Never Blames the Customer

**Intent:** As a Customer, I want the system to reassure me when something goes wrong, so I never feel like the failure is my fault.

**Value:** Transforms error moments from frustration into trust. By taking ownership of failures and offering clear recovery paths, the system maintains the customer's confidence and willingness to continue.

### Acceptance Criteria

- Error messages use human-readable language — no status codes, stack traces, or technical jargon.
- Every error message includes at least one actionable recovery step.
- Error messages never blame the customer ("you entered invalid data" → "we couldn't process that — here's what to try").
- Transient failures (network, timeout) display a reassuring message with automatic retry or a simple retry button.
- If an operation fails after the customer invested effort (e.g., a long form), the system preserves their input.

---

## EG-03 — The System Protects Information Calmly

**Intent:** As a Customer, I want my personal information displayed only when relevant and in context, so I feel confident nothing is being exposed unnecessarily.

**Value:** Protects customer confidence by handling personal data (names, contact info, dog health notes) with visible restraint. When the system shows only what's needed, the customer never feels surveilled or exposed.

### Acceptance Criteria

- Screens display only the personal data relevant to the current task.
- Sensitive fields (e.g., phone numbers, emergency contacts) are masked by default and revealed only on explicit action.
- The system never displays personal data in URLs, page titles, or browser tabs.
- Data-sharing boundaries are communicated clearly before any action that transmits information externally.

---

## EG-04 — The System Is Forgiving of Interruption and Absence

**Intent:** As a Customer, I want to step away and come back without losing my place or my work, so I never feel punished for having a life outside the app.

**Value:** Respects the customer's time and attention. Dog owners are busy — interrupted by a walk, a vet call, or a puppy emergency. The system should welcome them back warmly, not make them start over.

### Acceptance Criteria

- In-progress form data is preserved across session interruptions (e.g., browser close, timeout) using local or server-side draft persistence.
- The system never silently discards unsaved work — if recovery isn't possible, it communicates what was lost and why.
- Session expiration redirects to login with a return path to the customer's previous location.
- Idle timeouts use a visible countdown with an option to extend before disconnecting.
- Returning customers land in a meaningful context (e.g., dashboard, last-viewed dog) — never a blank or generic screen.

---

## EG-05 — The System Treats the Customer as a Responsible Partner

**Intent:** As a Customer, I want the system to treat me as a responsible, caring dog owner, so I never feel judged or patronized.

**Value:** Honors the customer's relationship with their dog by maintaining a peer-level, respectful tone. The system supports without overstepping — it offers help but never imposes.

### Acceptance Criteria

- Language treats the customer as a knowledgeable partner, not a novice ("your dog's vaccination schedule" not "make sure you vaccinate your dog").
- Suggestions and recommendations are offered, never mandated ("you might consider..." not "you must...").
- The system never implies negligence or judgment based on data patterns (e.g., gaps in visit history).
- Health and care information is presented factually without emotional manipulation or scare tactics.
- The customer's naming conventions and preferences for their dog are respected throughout the interface.

---

## EG-06 — Risk Is Explicit and Reversible

**Intent:** As a Customer, I want to feel confident that my changes are safe and reversible, so I never hesitate to explore or update information.

**Value:** Removes the fear of making mistakes. When every change feels safe to try, the customer engages more freely and maintains a sense of control over their data.

### Acceptance Criteria

- Destructive actions (delete, remove, cancel) require explicit confirmation with a clear description of consequences.
- The confirmation dialog distinguishes between reversible and irreversible actions.
- Where possible, destructive actions are soft-deletes with an undo window.
- Edit operations show a clear diff or summary of what changed before saving.
- The system communicates risk level proportionally — routine saves are frictionless, high-impact changes get proportional safeguards.

---

## EG-07 — The System Behaves Consistently

**Intent:** As a Customer, I want the system to behave the same way every time I use it, so I can build confidence through familiarity rather than relearning the interface.

**Value:** Predictability builds trust. When the same action always produces the same result, the customer stops second-guessing and starts moving with confidence. Inconsistency — even in small things — erodes the sense of safety.

### Acceptance Criteria

- Identical actions produce identical outcomes regardless of where they appear in the interface (e.g., "Save" always saves, "Cancel" always cancels without side effects).
- Visual patterns are reused — buttons, cards, forms, and feedback indicators look and behave the same across features.
- Navigation patterns are uniform — the customer never has to learn a new mental model for a different section of the app.
- Terminology is consistent — the same concept uses the same word everywhere (never "dog" in one place and "pet" in another).
- Loading, success, and error states follow the same visual and interaction pattern across all features.

---

## How Emotional Guarantees Are Used

Each functional story references its applicable EG codes in an `## Emotional Guarantees` section. During story refinement and review, the team verifies that the story's acceptance criteria satisfy the referenced guarantees.

Emotional Guarantees are **not standalone slices** — they produce no vertical implementation. They are quality attributes absorbed into the acceptance criteria of every feature they touch.
