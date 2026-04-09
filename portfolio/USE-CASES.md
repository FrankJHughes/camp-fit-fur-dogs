# Portfolio Use Cases

> How Camp Fit Fur Dogs artifacts surface professional capability to
> potential employers — organized around project milestones.

---

## UC-01 · Hiring Manager Reviews Repo

**Actor:** Non-technical hiring manager

**Trigger:** Clicks GitHub link on resume or LinkedIn.

**Flow:**
1. Lands on repo root → reads [`README.md`](../README.md) — project summary, milestone
   progress table, and engineering practices overview.
2. Sees milestone tracker link → clicks through to GitHub Milestones page
   → sees progress bars for M1, M2, M3.
3. Navigates to [`portfolio/README.md`](README.md) → sees curated index of
   portfolio-relevant artifacts.
4. Reviews [`product/VISION.md`](../product/VISION.md) → understands product thinking and
   emotional safety commitments.

**Success:** Manager understands scope, quality bar, and Frank's role
within 3 minutes. Milestone progress shows the project is actively
advancing, not abandoned.

---

## UC-02 · Technical Evaluator Assesses Architecture

**Actor:** Senior engineer or engineering manager

**Trigger:** Asked to evaluate candidate's technical ability.

**Flow:**
1. Opens [`docs/adr/`](../docs/adr/) → reviews 10 Architecture Decision Records covering
   DDD layers, DX toolchain, infrastructure, and governance choices.
2. Inspects [`src/`](../src/) → sees clean DDD layer separation (Api, Application,
   Domain, Infrastructure, SharedKernel).
3. Opens milestone M1 on GitHub → sees which stories are in progress and
   how domain aggregates map to customer-facing features.
4. Inspects CI/CD pipeline ([`.github/workflows/ci.yaml`](../.github/workflows/ci.yaml)) → confirms
   deterministic builds and test gates.
5. Reads recent PRs → evaluates commit hygiene, PR descriptions, and
   branch discipline.

**Success:** Evaluator confirms production-grade engineering practices
and sees a DDD implementation serving real domain operations, not just
a scaffold.

---

## UC-03 · Product Interviewer Validates Process Skill

**Actor:** Product manager or Scrum coach

**Trigger:** Interview prep or async evaluation.

**Flow:**
1. Opens [`product/stories/`](../product/stories/) → reviews story-first markdown backlog,
   capability themes, and emotional safety guarantees.
2. Opens milestone tracker → sees how 22 remaining stories are organized
   into three capability milestones (not just sprints).
3. Opens GitHub Projects board → sees sprint cadence and issue lifecycle.
4. Opens [`docs/governance/governance.md`](../docs/governance/governance.md) → reads living governance
   document with versioned process decisions.
5. Opens [`CONTRIBUTING.md`](../CONTRIBUTING.md) → sees 2-step story workflow (story file as
   backlog, issue as sprint commitment).
6. Opens [`docs/sprint-reviews/sprint-2.md`](../docs/sprint-reviews/sprint-2.md) → sees structured retro with
   honest "what could improve" section.

**Success:** Interviewer validates Agile fluency, stakeholder empathy,
and the discipline to separate planning cadence (sprints) from
capability goals (milestones).

---

## UC-04 · Creative Director Reviews Digital Craft

**Actor:** Creative lead or design-oriented hiring manager

**Trigger:** Portfolio review for hybrid roles.

**Flow:**
1. Opens [`product/emotional-guarantees/`](../product/emotional-guarantees/) → sees design principles that
   prioritize user emotional safety alongside functionality.
2. Reviews milestone M2 stories → sees edge case handling driven by
   empathy, not just requirements (e.g., "Reassurance on Failure,"
   "Respect Owner," "Confirm Destructive Action").
3. Reviews [`product/VISION.md`](../product/VISION.md) → sees product narrative combining
   technical rigor with human-centered design.

**Success:** Reviewer sees evidence of intentional design thinking
paired with technical execution — not just CRUD screens, but a system
that treats users with care.
