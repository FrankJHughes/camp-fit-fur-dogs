# Story Governance

This document defines how stories are created, maintained, prioritized, and moved through the delivery lifecycle.  
It complements (but does not duplicate) the Story Template and Workflow Conventions.

Stories represent **user-facing capabilities**. Tasks represent **technical work**.  
Governance ensures stories remain stable, human-centered, and aligned with product milestones.

---

# 1. Story Lifecycle

Stories move through the following lifecycle:

1. **Backlog**  
   - Story exists as a file under `product/stories/<domain>/US-XXX-title.md`.  
   - Must follow Story Template grammar.  
   - Must include EG/LG mappings.  
   - Must include milestone and domain metadata.

2. **Ready for Sprint**  
   - Meets Definition of Ready (from Product Owner Guide).  
   - Acceptance criteria are complete, observable, and testable.  
   - Dependencies are listed and resolved or scheduled.  
   - Story is labeled with urgency and importance.

3. **In Sprint**  
   - Story is pulled into a sprint only by the Product Owner.  
   - Sprint Story file is created under `.github/ISSUE_TEMPLATE/story.md` via GitHub UI.  
   - Acceptance criteria are copied into the sprint story for visibility.  
   - Story cannot be materially rewritten once the sprint begins.

4. **In Progress**  
   - Work proceeds using TDD and slice-based development.  
   - Tasks may be created, but the story remains the governing artifact.  
   - Any change to acceptance criteria requires Product Owner approval.

5. **Review & Acceptance**  
   - PR must reference the story ID.  
   - PR must satisfy the Definition of Done.  
   - Product Owner verifies acceptance criteria and EG/LG alignment.

6. **Done**  
   - Story is marked shipped in `catalog.csv`.  
   - Milestone progress is updated.  
   - Changelog entry is added if user-facing.

---

# 2. Story Structure Governance

Stories must follow the Story Template:

- Human actor (Owner, Staff, Admin)  
- Capability expressed as a verb  
- Business value expressed as an outcome  
- Acceptance criteria  
- Emotional Guarantees  
- Notes and dependencies  

Stories must **never**:

- Describe implementation details  
- Reference technical components (API, DB, backend, etc.)  
- Contain tasks disguised as stories  
- Contain acceptance criteria that require reading code to verify

---

# 3. Milestone Governance

Each story must declare a milestone:

- **M0** — Foundation  
- **M1** — Authentication & Core UX  
- **M2** — Hosting, Deployment, and Security  
- **M3** — Booking & Customer Experience  

Milestones govern:

- Release sequencing  
- Cross-team coordination  
- Dependency planning  
- Changelog grouping  

Stories may not move between milestones without Product Owner approval.

---

# 4. Urgency & Importance (Covey Quadrants)

Each story must be labeled with:

- **Urgency**: high, medium, low  
- **Importance**: high, medium, low  
- **Covey Quadrant**: Q1–Q4 (derived automatically)

Quadrants:

- **Q1** — Urgent & Important (commitment-critical)  
- **Q2** — Not Urgent & Important (strategic, high leverage)  
- **Q3** — Urgent & Not Important (avoid unless necessary)  
- **Q4** — Not Urgent & Not Important (rare; usually removed)

Quadrant determines:

- Sprint planning priority  
- Whether a story is eligible for deferral  
- Whether a story requires explicit justification to include

---

# 5. Emotional & Legal Guarantees Integration

Stories must explicitly list:

- **Emotional Guarantees (EG)** — user trust and emotional safety  
- **Legal Guarantees (LG)** — compliance and regulatory obligations  

Governance rules:

- EG/LG must be copied into the sprint story  
- EG/LG must be validated during acceptance  
- EG/LG must be reflected in acceptance criteria where relevant  
- EG/LG drift between story files and catalog is prohibited

---

# 6. Backlog Hygiene

The backlog must remain:

- **Flat** — no epics inside epics  
- **Stable** — story IDs never change  
- **Consistent** — metadata must match `catalog.csv`  
- **Current** — outdated stories must be updated or removed  
- **Traceable** — dependencies must be explicit and accurate  

Every story must have:

- A unique ID  
- A domain  
- A milestone  
- EG/LG mappings  
- Urgency/importance  
- A file path recorded in `catalog.csv`

---

# 7. Dependency Governance

Dependencies must be:

- Listed in the story frontmatter  
- Represented in `catalog.csv`  
- Resolved or scheduled before sprint inclusion  
- Avoided when possible (prefer vertical slices)

Circular dependencies are prohibited.

---

# 8. Sprint Inclusion Rules

A story may enter a sprint only if:

- It meets Definition of Ready  
- It has no unresolved blockers  
- It fits within sprint capacity  
- It aligns with the sprint goal  
- It has clear acceptance criteria  
- It has EG/LG mappings  
- It has urgency/importance labels  

Stories cannot be added mid-sprint except for:

- Critical production issues  
- Security vulnerabilities  
- Compliance requirements

---

# 9. Story Completion Rules

A story is complete only when:

- All acceptance criteria pass  
- All EG/LG requirements are satisfied  
- All tasks are complete  
- Documentation is updated  
- Changelog entry is added (if applicable)  
- Catalog is updated  
- Product Owner approves the PR  

Stories may not be closed by developers alone.

---

# 10. Governance Enforcement

- PR reviewers enforce story governance  
- Product Owner enforces milestone and EG/LG rules  
- CI enforces catalog consistency  
- Scripts enforce metadata correctness  
- Backlog grooming enforces hygiene  

Governance violations must be corrected before merge.

