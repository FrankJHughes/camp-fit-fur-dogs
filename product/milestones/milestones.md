# Product Milestones

Milestones are delivery horizons — not sprints. Each milestone represents a
meaningful product capability boundary that could stand on its own. Milestones
are sequenced by dependency and value: M0 enables M1, M1 enables M2, and so on.

A story's `milestone` frontmatter field assigns it to a horizon. Milestones
inform sprint planning but do not dictate it — a sprint may pull stories from
multiple milestones when dependencies allow.

---

## M0 — Foundation (Complete)

**Theme:** Build the engine — domain primitives, CQRS, persistence, CI/CD.

**Status:** ✅ Shipped (Sprints 1–6)

**What shipped:**
- Frank (SharedKernel) v0.1.0: DDD building blocks — entities, value objects,
  aggregates, repository contracts, CQRS abstractions, dispatchers, DI conventions
- Frank v0.2.0: Foundation Extraction — Unit of Work, BCrypt hashing, EF Entity
  Auto-Discovery, Readers
- Dog CRUD: Register, View, Edit, Remove, List (domain + API + frontend pages)
- Next.js frontend with React
- CI/CD pipeline (build, test, lint)
- Feature Slice Walkthrough (US-052)

**Exit criteria:** A developer can clone the repo, run tests, and see a working
dog management application with a clean DDD architecture.

---

## M1 — Identity & Launch Readiness

**Theme:** Users can create accounts, log in, and the product can legally go
live on the public internet.

**Goal:** The minimum bar to deploy a public-facing application that collects
user data without legal exposure.

**Key stories:**
- **Auth:** Create Account Page (US-126), Login (US-110), Session Management
  (US-111), Email Verification (US-148), Password Reset Email (US-146)
- **Communication infra:** Outbox Pattern (US-143), Transactional Email (US-144)
- **Legal floor:** Privacy Policy & Terms (US-156), Web Accessibility / WCAG 2.1
  AA (US-155)
- **Security baseline:** Rate Limiting (US-132), Account Lockout (US-133),
  Security Headers (US-134), CORS Policy (US-135)
- **Deployment:** Frontend Hosting (US-139), API Hosting (US-140), Database
  Hosting (US-141), Custom Domain & SSL (US-142)
- **SEO:** Page-Level SEO (US-136), Technical SEO Foundation (US-137), Core
  Web Vitals (US-138)
- **Infra:** Audit Timestamps (US-127), Soft-Delete & Restore (US-125), CI
  Path-Based Test Skipping (US-171)

**Exit criteria:** A real person can register an account with email/password,
verify their email, log in, manage dogs, and reset their password — on a live
URL with HTTPS, a privacy policy, and accessible UI.

---

## M2 — Communication & Trust

**Theme:** Multi-channel communication, security trust features, and social
login convenience.

**Goal:** Owners feel confident their data is safe, can be reached through
their preferred channel, and can log in with their existing identity provider.

**Key stories:**
- **Communication:** Welcome Email (US-145), Notification Preferences (US-147),
  Customer Contact Profile (US-149), SMS Infrastructure (US-150)
- **Social login:** Login with Microsoft (US-128), Google (US-129), Apple
  (US-130), Amazon (US-131)
- **Security trust:** Login Activity & Security Notifications (US-152),
  Breached Password Detection (US-154), Optional 2FA (US-151)
- **Data rights:** Account Data Export & Deletion (US-153)
- **Compliance:** CAN-SPAM & TCPA Hardening (US-158), Cookie Consent (US-157)

**Exit criteria:** Owners receive communications via their preferred channel,
can log in with social providers, see login activity, and can export/delete
their data self-service.

---

## M3 — Booking & Operations

**Theme:** The core business flow — owners can browse services, book
appointments, and manage their dog's health records.

**Goal:** Camp Fit Fur Dogs becomes a functioning business — services are
bookable, capacity is managed, and health/safety prerequisites are enforced.

**Key stories:**
- **Catalog:** Service Catalog (US-159), Schedule & Availability (US-160),
  Browse Available Services (US-161)
- **Booking:** Book a Service (US-162), View My Bookings (US-163), Cancel
  Booking (US-164), Booking Notifications (US-165), Waitlist (US-166)
- **Health & safety:** Dog Health Profile (US-167), Vaccination Tracking
  (US-168), Emergency Contact & Vet Info (US-169), Behavior Assessment (US-170)
- **Eligibility:** Booking Eligibility & Prerequisites Management (US-172)

**Exit criteria:** An owner can browse services, verify their dog meets
prerequisites, book a daycare session, receive confirmation and reminder
notifications, cancel if plans change, and join a waitlist if capacity is full.

---

## M4 — Growth & Maturity (Future)

**Theme:** Payments, admin operations, and business intelligence.

**Goal:** Camp Fit Fur Dogs is a complete SaaS product with revenue processing,
staff tools, and operational reporting.

**Potential story areas (not yet drafted):**
- Payments & billing (Stripe integration, invoicing, refunds, membership plans)
- Check-in & check-out (daily attendance, drop-off/pickup tracking)
- Admin portal (staff management, roles & permissions, dashboard)
- Reporting & analytics (attendance trends, revenue dashboards, utilization)
- Recurring bookings (weekly daycare subscriptions)
- Staff scheduling & assignment
- Incident reports & communication

**Exit criteria:** The business can process payments, track daily attendance,
manage staff, and measure key metrics.

---

## Milestone Rules

1. **A story belongs to exactly one milestone** — no story spans milestones.
2. **Dependencies within a milestone are fine** — a milestone is internally
   ordered by story dependencies.
3. **Cross-milestone dependencies flow forward** — M2 can depend on M1; M1
   cannot depend on M2.
4. **Shipped stories remain assigned to their original milestone** — they are
   not moved to M0 retroactively.
5. **Milestones are groomed quarterly** — new stories are assigned during
   sprint planning; milestone scope is reviewed when the product vision shifts.
