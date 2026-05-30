# Contributor Governance

This document defines how contributors interact with the repository, how work is accepted, and how conventions and governance rules are enforced.  
It complements (but does not duplicate) the conventions in `docs/conventions/` and the Copilot Instructions.

Contributor governance ensures:

- Predictable, high‑quality contributions  
- No drift across code, docs, or workflows  
- Consistent application of conventions  
- Safe, reversible changes  
- A stable multi‑product architecture  

Contributors are responsible for following conventions; reviewers are responsible for enforcing them.

---

# 1. Contribution Principles

All contributions must follow these principles:

- **Convention First** — conventions are the source of truth  
- **Script First** — file generation and updates must be script‑driven  
- **Full‑File Patching** — no partial edits; regenerate entire files  
- **No Drift** — code, docs, stories, and catalog must remain aligned  
- **Small, Focused PRs** — each PR should do one thing  
- **Test‑Driven** — new behavior must be driven by tests  
- **Product Boundaries** — no cross‑product leakage  

Contributors must read conventions before contributing.

---

# 2. Roles and Responsibilities

## Contributors
Responsible for:

- Following conventions  
- Writing tests first  
- Updating documentation in the same PR  
- Maintaining story and catalog alignment  
- Ensuring changelog entries for user‑facing changes  
- Respecting product boundaries  
- Using scripts for file generation  

## Reviewers
Responsible for:

- Enforcing conventions  
- Enforcing governance rules  
- Rejecting PRs with drift  
- Ensuring CI passes  
- Ensuring stories and changelog entries are correct  
- Protecting product boundaries  

## Product Owner
Responsible for:

- Story acceptance  
- Milestone governance  
- EG/LG alignment  
- Backlog hygiene  
- Prioritization and sprint planning  

---

# 3. Contribution Workflow

All contributions follow this workflow:

1. **Create or update a story** (if needed)  
2. **Write failing tests** (red)  
3. **Implement the smallest change** (green)  
4. **Refactor safely** (refactor)  
5. **Update documentation**  
6. **Update changelog** (if user‑facing)  
7. **Update catalog** (if story metadata changes)  
8. **Open a PR using the PR template**  
9. **Ensure CI passes**  
10. **Request review**  

PRs that skip any step must be rejected.

---

# 4. PR Governance

All PRs must:

- Use the full PR template  
- Include a complete Summary  
- Reference a story ID  
- Include a Changes section  
- Include the Merge Checklist  
- Include updated docs (if behavior changed)  
- Include a changelog entry (if user‑facing)  
- Pass all CI checks  
- Not introduce drift  

PRs must not:

- Include PR numbers in changelog entries  
- Modify multiple unrelated areas  
- Introduce architectural changes without an ADR  
- Bypass conventions  
- Include partial file edits  

PRs that violate governance rules must be closed or corrected.

---

# 5. Story and Catalog Governance for Contributors

Contributors must ensure:

- Story metadata matches `catalog.csv`  
- EG/LG mappings are correct  
- Milestone is correct  
- Domain is correct  
- Dependencies are correct  
- File path is correct  
- Status is correct  

If a story changes meaningfully, the story file must be updated in the same PR.

Catalog drift is prohibited.

---

# 6. Documentation Governance for Contributors

Documentation must be:

- Updated in the same PR as the behavior change  
- Accurate  
- Consistent with conventions  
- Free of duplication  
- Free of stale content  

Documentation must follow:

- Script‑first rules  
- Fencing rules  
- Quoting rules  
- Full‑file regeneration rules  

Docs are a first‑class artifact.

---

# 7. Changelog Governance for Contributors

Contributors must:

- Add a changelog entry for user‑facing changes  
- Place entries under **[Unreleased]**  
- Reference story IDs  
- Use user‑facing language  
- Avoid PR numbers  
- Avoid implementation details  

A story marked “shipped” without a changelog entry is incomplete.

---

# 8. CI Governance for Contributors

Contributors must:

- Ensure CI passes  
- Ensure path‑based test skipping behaves correctly  
- Ensure skipped suites appear as “skipped”  
- Ensure no required suite is omitted  
- Ensure workflows remain deterministic  

CI failures are governance failures.

---

# 9. Product Boundary Governance

Contributors must not:

- Mix Camp Fit Fur Dogs and SharedKernel concerns  
- Introduce SharedKernel dependencies on product code  
- Place SharedKernel code in product folders  
- Place product code in SharedKernel  
- Mix frontend and backend concerns  
- Mix domains within a story  

Product boundaries are enforced by:

- Reviewers  
- CI  
- Architecture tests  

---

# 10. ADR Governance

Contributors must create an ADR when:

- Architecture changes  
- Dependencies change  
- Hosting or deployment changes  
- SharedKernel behavior changes  
- Security posture changes  

ADR must:

- Follow ADR conventions  
- Explain context, decision, and consequences  
- Reference conventions and governance rules  

---

# 11. Contributor Safety Rules

Contributors must:

- Avoid destructive operations  
- Avoid rewriting history on shared branches  
- Avoid force‑pushes to `main`  
- Avoid committing secrets  
- Avoid bypassing CI  
- Avoid manual file edits that violate script‑first rules  

Safety rules protect the integrity of the repository.

---

# 12. Governance Enforcement

- Reviewers enforce contributor governance  
- CI enforces structural and behavioral rules  
- Product Owner enforces story and milestone alignment  
- Scripts enforce metadata correctness  

No PR may merge if contributor governance is violated.

