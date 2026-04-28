# Repository Hygiene Governance

This document defines the rules that keep the repository coherent, consistent, and free of drift.  
It complements (but does not duplicate) the architecture, workflow, code, and documentation conventions.

Repository hygiene ensures that:

- Metadata stays consistent across files  
- Stories, catalog entries, and changelog entries remain aligned  
- CI workflows behave predictably  
- Documentation reflects reality  
- No part of the system silently diverges from another  

Hygiene is enforced through governance, automation, and review.

---

# 1. Hygiene Principles

The repository must remain:

- **Consistent** — metadata matches across all artifacts  
- **Traceable** — every change has a clear source and purpose  
- **Deterministic** — scripts and workflows behave the same on all machines  
- **Isolated** — products do not leak into each other  
- **Up-to-date** — no stale or contradictory files  

Hygiene violations create drift, which is treated as a governance issue.

---

# 2. Story File Hygiene

Each story file under `product/stories/**` must:

- Match its metadata in `catalog.csv`  
- Use the correct file path and naming convention  
- Include EG/LG mappings  
- Include milestone, domain, and status  
- Include dependencies  
- Follow Story Template grammar  

A story file is the **canonical source** for:

- Acceptance criteria  
- EG/LG mappings  
- Dependencies  
- Intent and value  

The catalog reflects the story; it does not override it.

---

# 3. Catalog Hygiene (`catalog.csv`)

`catalog.csv` is the authoritative index of all stories.

Governance rules:

- Every story must appear exactly once  
- Story metadata must match the story file  
- Milestone, domain, urgency, importance, and quadrant must be correct  
- EG/LG codes must match the story file  
- File path must be correct and stable  
- Status must reflect reality (backlog, in-progress, shipped)  

Catalog drift is prohibited.

---

# 4. Changelog Hygiene

Changelog entries must:

- Reference story IDs, not PR numbers  
- Appear under **[Unreleased]** until a release  
- Be user-facing  
- Match the story’s shipped status  
- Use consistent language and formatting  

A story marked “shipped” without a changelog entry is incomplete.

A changelog entry referencing a story not marked “shipped” is invalid.

---

# 5. CI Workflow Hygiene

CI workflows must remain:

- Deterministic  
- Path-based  
- Aligned with `ci-deps.json`  
- Free of drift between workflow logic and governance rules  

Governance requires:

- Path filters must match the repository structure  
- Zones must remain accurate (backend, frontend, shared-kernel, infra, docs-only)  
- Test suite mapping must remain correct  
- Infra changes must trigger all suites  
- Docs-only changes must skip all suites  
- Merges to `main` must run all suites  

Any change to CI workflows must be reflected in:

- `ci-deps.json`  
- CI governance  
- Documentation (if applicable)

---

# 6. Documentation Hygiene

Documentation must:

- Reflect current behavior  
- Be updated in the same PR as the change  
- Follow the conventions in `docs/conventions/docs.md`  
- Avoid duplication across files  
- Maintain canonical ownership  

Governance requires:

- No stale documentation  
- No contradictory documentation  
- No duplicated rules across files  
- No undocumented behavior changes  

Documentation drift is a governance violation.

---

# 7. Multi-Product Hygiene

Camp Fit Fur Dogs and Frank (SharedKernel) are separate products.

Governance requires:

- Separate changelogs  
- Separate milestones  
- Separate story domains  
- Separate architectural boundaries  
- No cross-product leakage  
- No shared folders except SharedKernel  

SharedKernel is a product, not a folder.

---

# 8. File Encoding and Line Endings

All generated files must use:

- **Encoding:** `utf8NoBOM`  
- **Line endings:** LF (enforced by `.gitattributes`)  

This prevents corruption across platforms.

---

# 9. Script Hygiene

Scripts must:

- Be idempotent  
- Use `-LiteralPath` for all file operations  
- Avoid wildcard interpretation  
- Use safe quoting rules  
- Follow script-first conventions  

Scripts must never:

- Modify files partially  
- Perform search-and-replace on Markdown headings  
- Generate nested fences  
- Introduce encoding drift  

Scripts are the primary mechanism for maintaining hygiene.

---

# 10. Drift Detection and Correction

Drift may occur between:

- Story files and catalog  
- Catalog and changelog  
- CI workflows and path filters  
- Documentation and behavior  
- Conventions and copilot-instructions  

Governance requires:

- Drift must be corrected immediately  
- Drift must be corrected in the same PR that introduces it  
- Drift must not accumulate  
- Drift must not be ignored  

Reviewers are responsible for detecting drift.  
CI is responsible for preventing drift where possible.

---

# 11. Governance Enforcement

- Reviewers enforce hygiene rules  
- CI enforces structural and metadata consistency  
- Product Owner enforces story and milestone hygiene  
- Scripts enforce encoding and file correctness  

No PR may merge if hygiene rules are violated.

