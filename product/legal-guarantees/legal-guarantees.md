# Legal Guarantees

Legal Guarantees are the product's promises about what the system **must do
by law**. They are not features — they are compliance attributes enforced as
acceptance criteria on every functional story that touches their domain.

Every story file references the LG codes that apply. This document is the
single source of truth for what each guarantee means, which law governs it,
and the concrete acceptance criteria that make it auditable.

Legal Guarantees and Emotional Guarantees are complementary:

- **Emotional Guarantees** define the ceiling — how the product *should* feel.
- **Legal Guarantees** define the floor — what the product *must* do.

A story can (and often does) reference both. EGs guide design decisions; LGs
gate deployment.

---

## LG-01 — The System Is Accessible to Everyone

**Governing Law:** ADA Title III, California Unruh Civil Rights Act

**Intent:** As a person with a disability, I need the system to be fully usable
with assistive technology so that I can participate independently.

**Value:** Web accessibility is the highest legal risk for any public-facing
website in California. The Unruh Act allows statutory damages of $4,000+ per
violation per visit — without proof of intent. WCAG 2.1 Level AA is the de
facto standard referenced in virtually every ADA Title III lawsuit. Beyond
compliance, accessible design improves usability for all customers.

### Acceptance Criteria

- All interactive elements are reachable and operable via keyboard alone.
- Focus order follows a logical reading sequence; focus indicators are always visible.
- All form inputs have associated `<label>` elements (not placeholder-only).
- All images have meaningful `alt` text (decorative images use `alt=""`).
- Text meets minimum contrast ratios (4.5:1 body, 3:1 large text).
- Color is never the sole means of conveying information.
- Page language is declared (`<html lang="en">`).
- Semantic HTML is used (`<nav>`, `<main>`, `<header>`, `<footer>`, `<section>`).
- ARIA attributes are used correctly when semantic HTML is insufficient.
- Dynamic content updates are announced via ARIA live regions.
- No keyboard traps; skip-navigation link is present on every page.
- Automated accessibility audit (axe-core) runs in CI and blocks on violations.

---

## LG-02 — The System Is Transparent About Data Practices

**Governing Law:** CalOPPA (California Online Privacy Protection Act)

**Intent:** As a customer, I need to understand what personal data the system
collects, why it is collected, and who it is shared with — before I provide
any information — so that I can make an informed trust decision.

**Value:** CalOPPA requires a conspicuous privacy policy for any commercial
website or app that collects personally identifiable information from
California residents. Unlike CCPA/CPRA, there is no revenue or consumer
threshold — it applies to every business. Transparency is also the
foundation of customer trust: owners who understand data practices feel
safe; owners who don't feel suspicious.

### Acceptance Criteria

- A privacy policy is conspicuously linked from every page (footer) and from the home page.
- The privacy policy discloses: what data is collected, how it is collected, why it is collected, who it is shared with, how it is protected, and how long it is retained.
- The privacy policy describes how customers can access, export, and delete their data.
- The privacy policy describes the process for notifying customers of material changes.
- An effective date is displayed prominently on the privacy policy.
- Registration and data-collection forms never collect data not disclosed in the privacy policy.
- Any new data collection point triggers a privacy policy review before deployment.

---

## LG-03 — The System Communicates Only With Consent

**Governing Law:** CAN-SPAM Act (email), TCPA (SMS/voice), FCC One-to-One
Consent Rule (2025)

**Intent:** As a customer, I need the system to contact me only through channels
I explicitly authorized so that I am never subjected to unsolicited messages.

**Value:** CAN-SPAM violations carry penalties up to $51,744 per email. TCPA
violations range from $500 to $1,500 per text message and are class-action
eligible. US mobile carriers actively block non-compliant SMS senders. The
FCC's 2025 one-to-one consent rule requires that SMS consent name a single
specific business. These are actively enforced — not theoretical.

### Acceptance Criteria

- No commercial email is sent without a prior opt-in or existing business relationship.
- Every commercial email includes a visible, functional unsubscribe link.
- Unsubscribe requests are honored within 10 business days.
- Every commercial email includes the physical mailing address of the business.
- "From" and "Reply-To" addresses accurately identify the sender.
- No SMS is sent without prior express written consent documented with: phone number, timestamp, consent language displayed, and collection method.
- SMS consent is one-to-one — specific to Camp Fit Fur Dogs, not shared or bundled.
- Every SMS includes opt-out instructions ("Reply STOP to unsubscribe").
- STOP requests are processed immediately; a confirmation message is sent.
- SMS is only sent during acceptable hours (8 AM – 9 PM in the recipient's local time zone).
- Consent records are immutable and retained for the customer relationship duration + 5 years.
- The outbox checks consent status before dispatching any non-transactional message.
- Transactional messages (password reset, security alerts) are clearly distinguished from commercial messages.

---

## LG-04 — The Customer Controls Their Data

**Governing Law:** CalOPPA, CA Civil Code §1798.100+ (CCPA/CPRA future-proofing)

**Intent:** As a customer, I need to be able to access, export, and permanently
delete all my personal data without contacting support so that I remain in
full control of my information.

**Value:** CalOPPA requires that customers be able to review and request changes
to their personal information. While CCPA/CPRA does not currently apply (Camp
Fit Fur Dogs is below the $26.6M revenue / 100K consumer thresholds),
building data control now is both a trust signal and future-proofing. When
customers know they can leave — and take their data with them — they feel
safe staying.

### Acceptance Criteria

- Customers can view all personal data the system holds about them from their profile.
- Customers can request a full data export (structured, human-readable format).
- Customers can request permanent account deletion with a documented grace period.
- Data export and deletion are self-service — no support ticket required.
- Deletion uses soft-delete during the grace period, then permanently purges all associated data.
- The privacy policy describes the self-service export and deletion options.
- Data export requests are rate-limited to prevent abuse.
- Account deletion terminates all active sessions immediately.

---

## LG-05 — The System Protects Data With Reasonable Security

**Governing Law:** CA Civil Code §1798.81.5 (Reasonable Security), CA Data
Breach Notification Law (§1798.29 / §1798.82)

**Intent:** As a customer, I need my personal data to be protected with
industry-standard security measures so that I can trust the system with
sensitive information.

**Value:** California law requires businesses to implement and maintain
"reasonable security procedures and practices" appropriate to the nature of
the information. What constitutes "reasonable" is informed by industry
standards (OWASP, NIST). Data breach notification is mandatory — businesses
must notify affected California residents "in the most expedient time
possible" following discovery.

### Acceptance Criteria

- Passwords are hashed with a strong adaptive algorithm (BCrypt, Argon2) — never stored in plaintext.
- Sensitive data at rest is encrypted or access-controlled.
- All data in transit uses TLS (HTTPS enforced).
- Secrets (API keys, connection strings) are stored in environment variables or secret managers — never in source code.
- Authentication tokens have bounded lifetimes and are securely stored (HttpOnly, Secure, SameSite cookies).
- Security-relevant events (login, password change, data export) are logged for audit.
- Failed login attempts are rate-limited to prevent brute force.
- A data breach response plan exists and is documented in the repo.
- Dependencies are monitored for known vulnerabilities (Dependabot, `dotnet list package --vulnerable`).

---

## How Legal Guarantees Are Used

Legal Guarantees operate at two levels:

### 1. Story-level acceptance criteria

Each functional story references its applicable LG codes in a
`## Legal Guarantees` section, alongside the existing `## Emotional Guarantees`
section. During story refinement and review, the team verifies that the story's
acceptance criteria satisfy the referenced guarantees.

```markdown
## Emotional Guarantees

- **EG-01 No Surprises** — The registration flow shows clear progress indicators
- **EG-02 No Blame** — Validation errors never imply the customer did something wrong

## Legal Guarantees

- **LG-01 Accessible** — Registration form is keyboard navigable and screen reader compatible
- **LG-02 Transparent** — Email collection is disclosed in the privacy policy
- **LG-03 Consensual** — Marketing opt-in checkbox is unchecked by default with explicit consent language
```

### 2. Definition of Done (standing AC)

The following checks apply to every PR that touches the corresponding domain:

| LG | DoD Check | Applies When |
|----|-----------|-------------|
| LG-01 | axe-core CI audit passes with zero violations | Any PR touching user-facing UI |
| LG-02 | Privacy policy reviewed if new data collection point added | Any PR adding a new data field |
| LG-03 | Consent check exists in outbox handler before dispatch | Any PR adding outbound communication |
| LG-04 | New data included in export and subject to deletion cascade | Any PR adding a new data entity |
| LG-05 | No secrets in source; TLS enforced; auth tokens bounded | Every PR |

### Relationship to compliance stories

Legal Guarantees are **not standalone slices** — they produce no vertical
implementation. They are compliance attributes absorbed into the acceptance
criteria of every feature they touch. The compliance stories (US-155 through
US-158) establish the infrastructure; the Legal Guarantees ensure ongoing
compliance as new features ship.

### Maintenance

Legal Guarantees should be reviewed annually or when relevant legislation
changes. California privacy law is evolving rapidly — the CCPA/CPRA thresholds
may lower, the ADPPA (federal privacy law) may pass, and WCAG 2.2 may become
the new accessibility benchmark. When a law changes, update the affected LG
and audit existing stories.
