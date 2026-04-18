# US-105 — Documentation Debloat & Consolidation

## Intent

As a **contributor**, I want the documentation suite debloated, deduplicated, and reorganized around canonical ownership so that every topic has exactly one source of truth, navigation is intuitive, and onboarding docs don't contradict each other.

## Value

- **~540 lines of duplicated prose eliminated** — zero information loss.
- **Canonical ownership map** — every topic has one authoritative file; all others link to it.
- **Faster onboarding** — contributors follow one path instead of reconciling 3–4 conflicting copies.
- **Easier maintenance** — updating a convention means editing one file, not hunting across 4.
- **Clean navigation** — one entry point (`docs/README.md`), no competing nav hubs.

## Scope — Findings & Actions

### F-1: Source Control Rules Duplicated 4 Ways
- `copilot-instructions.md`, `developer-guide.md`, `governance.md`, `scrum-master-guide.md`
- **Action:** `copilot-instructions.md` is canonical. Strip inline source control content from the other 3. Replace with one-sentence links.

### F-2: Milestone Table Duplicated 3 Ways
- `docs/README.md`, `product-owner-guide.md`, `scrum-master-guide.md`
- **Action:** `product-owner-guide.md` is canonical. Strip from docs/README.md and scrum-master-guide.md. Replace with links.

### F-3: Definition of Ready Duplicated 2 Ways
- `governance.md`, `product-owner-guide.md`
- **Action:** `product-owner-guide.md` is canonical. Strip from governance.md. Replace with link.

### F-4: PR Checklist / DoD Duplicated 3+ Ways
- `.github/PULL_REQUEST_TEMPLATE.md`, `governance.md`, `scrum-master-guide.md`, `copilot-instructions.md`
- **Action:** PR template is canonical. Strip from governance.md and scrum-master-guide.md. `copilot-instructions.md` keeps its copy (Copilot needs it inline).

### F-5: Repository Structure Duplicated 2 Ways
- `developer-guide.md` §3, `folder-structure.md` §1
- **Action:** `folder-structure.md` is canonical. Remove §3 from developer-guide.md. Link.

### F-6: Convention Maintenance Duplicated 2 Ways
- `product-owner-guide.md`, `scrum-master-guide.md`
- **Action:** `copilot-instructions.md` §Maintaining This Document is canonical. Strip from both guides. Link.

### F-7: Three Overlapping Navigation Hubs
- `docs/README.md`, `docs/guides/README.md`, `governance.md` §Where to Find Things
- **Action:** `docs/README.md` is the single entry point. Merge guides/README.md table into docs/README.md. Remove nav section from governance.md.

### F-8: `developer-guide.md` Is a Monolith (~250 lines)
- §3 duplicates folder-structure.md, §5 duplicates CONTRIBUTING.md, §6 duplicates copilot-instructions.md TDD rules, §8 duplicates source control rules.
- **Action:** Slim to 4 sections: Prerequisites, First-Time Setup, Developer Loop, Quick Links. ~80 lines.

### F-9: `governance.md` Is a Monolith (~200 lines)
- DoD, DoR, branch strategy, PR rules, and nav all duplicated elsewhere.
- **Action:** Slim to process and policy only: Roles, Cadence, ADR Process, Documentation Rules, Security, CI. ~120 lines.

### F-10: Stale References & Misplaced Files
- `guides/README.md` links to `docs/architecture/` (should be `docs/adr/`)
- `docs/README.md` missing link to developer/ subfolder docs
- `frontend-testing-guide.md` at `guides/` level instead of `guides/developer/`
- **Action:** Fix links, move frontend-testing-guide.md to `guides/developer/`.

## Canonical Ownership Map (Post-Consolidation)

| Topic | Canonical Owner | Everyone Else |
|-------|----------------|---------------|
| Source control rules | `copilot-instructions.md` | Link |
| PR checklist / DoD | `.github/PULL_REQUEST_TEMPLATE.md` | Link (copilot-instructions keeps inline copy) |
| Definition of Ready | `product-owner-guide.md` | Link |
| Milestones / Roadmap | `product-owner-guide.md` | Link |
| Repo folder structure | `folder-structure.md` | Link |
| Convention maintenance | `copilot-instructions.md` | Link |
| Navigation / index | `docs/README.md` | No other nav hubs |

## Acceptance Criteria

- [ ] Source control rules exist only in `copilot-instructions.md`; 3 other files link to it (F-1)
- [ ] Milestone table exists only in `product-owner-guide.md`; 2 other files link to it (F-2)
- [ ] Definition of Ready exists only in `product-owner-guide.md`; governance links to it (F-3)
- [ ] PR checklist exists only in PR template + `copilot-instructions.md`; 2 other files link to it (F-4)
- [ ] Repository structure exists only in `folder-structure.md`; developer-guide links to it (F-5)
- [ ] Convention maintenance exists only in `copilot-instructions.md`; 2 guides link to it (F-6)
- [ ] `docs/README.md` is the single navigation hub; `guides/README.md` content merged in, governance nav section removed (F-7)
- [ ] `developer-guide.md` slimmed to ~80 lines: Prerequisites, First-Time Setup, Developer Loop, Quick Links (F-8)
- [ ] `governance.md` slimmed to ~120 lines: Roles, Cadence, ADR Process, Doc Rules, Security, CI (F-9)
- [ ] Stale `docs/architecture/` link fixed to `docs/adr/` (F-10)
- [ ] `frontend-testing.md` moved to `guides/developer/` (F-10)
- [ ] `docs/README.md` links to developer/ subfolder docs (F-10)
- [ ] No information lost — every cut is a duplicate whose canonical source is intact
- [ ] All cross-references resolve (no broken links)
- [ ] CHANGELOG.md [Unreleased] updated

## Emotional Guarantees

- A contributor never reads conflicting guidance in two different docs.
- A contributor never wonders which version of a rule is authoritative.
- A maintainer never has to update the same content in multiple files.