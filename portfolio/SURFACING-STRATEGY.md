# Surfacing Strategy

> How portfolio artifacts travel from the repo to an evaluator's screen,
> organized around capability milestones.

---

## Principles

1. **Zero-hop discoverability** — every key artifact is reachable from
   the repo root `README.md` within one click.
2. **Layered depth** — surface a summary first, link to detail second.
   Respect evaluator time.
3. **Living proof** — link to the actual milestone tracker, CI runs, and
   merged PRs. Static screenshots are supplements, not substitutes.
4. **Narrative arc** — the project tells a progression story through
   milestones: foundation → first feature → full product → showcase.

---

## Milestone-Driven Narrative

Each milestone unlocks a new level of demonstrable capability:

| Milestone | What an Evaluator Sees | Key Artifacts |
|-----------|----------------------|---------------|
| M0 (done) | Engineering foundation, architecture, CI, governance | `docs/adr/`, `.github/workflows/`, `CONTRIBUTING.md` |
| M1 | First working feature end-to-end through DDD layers | `src/`, domain tests, API endpoints |
| M2 | Product maturity — edge cases handled with emotional safety | Product stories, sprint reviews, test coverage |
| M3 | Fully documented, tooled, onboardable project | `docs/runbooks/`, developer guide, scaffold tool |

The milestone tracker is the single source of truth for project progress:
https://github.com/frankjhughes/camp-fit-fur-dogs/milestones

---

## Channel Map

| Channel | What to Surface | Link Target |
|---------|----------------|-------------|
| Repo root `README.md` | Milestone progress table, architecture summary | Milestone tracker, `portfolio/README.md` |
| LinkedIn Featured section | Repo link with milestone status callout | Repo root |
| Resume / CV | Repo URL + current milestone + one-line description | Repo root |
| GitHub Profile README | Pinned repo with descriptive tagline | Repo root |
| Portfolio website (future) | Embedded milestone progress, case study pages | Milestone tracker, `portfolio/` |

---

## Repo Root README Additions

The repo root `README.md` includes:

- **Milestone Progress** table with links to GitHub Milestones
- **Sprint History** link to `CHANGELOG.md`
- **Sprint Board** link to GitHub Projects
- **Portfolio** section linking to `portfolio/README.md`

---

## Maintenance Cadence

| Action | Trigger | Frequency |
|--------|---------|-----------|
| Update milestone progress in README | Story completed | Per sprint |
| Update sprint review | Sprint ends | Per sprint |
| Update CHANGELOG | PR merged | Per PR |
| Record demo or write case study | Milestone completed | Per milestone |
| Review and prune stale artifacts | Calendar reminder | Monthly |
| Create new sprint labels | Sprint planning | Per sprint |

---

## Anti-Patterns to Avoid

- **Dead links** — CI should lint internal markdown links.
- **Orphaned artifacts** — every file in `portfolio/` must be linked from
  the portfolio `README.md`.
- **Sprint-centric communication** — stakeholders care about capability
  milestones ("can it register a dog?"), not timeboxes ("Sprint 3 ended").
- **Stale milestone counts** — README status section must stay current
  with actual milestone progress on GitHub.
- **Wall of text** — case studies over 800 words need an executive summary.
