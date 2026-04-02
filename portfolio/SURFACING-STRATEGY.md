# Surfacing Strategy

> How portfolio artifacts travel from the repo to an evaluator's screen.

---

## Principles

1. **Zero-hop discoverability** — every key artifact is reachable from
   the repo root `README.md` within one click.
2. **Layered depth** — surface a summary first, link to detail second.
   Respect evaluator time.
3. **Living proof** — link to the actual sprint board, CI runs, and merged
   PRs. Static screenshots are supplements, not substitutes.
4. **Narrative arc** — each case study follows Problem → Approach →
   Outcome → Reflection.

---

## Channel Map

| Channel | What to Surface | Link Target |
|---|---|---|
| Repo root `README.md` | Project summary, architecture badge, "Portfolio" section with link | `/portfolio/README.md` |
| LinkedIn Featured section | Case-study PDF or Loom demo link | `/portfolio/case-studies/*.md` or hosted URL |
| Résumé / CV | Repo URL + one-line project description | Repo root |
| GitHub Profile README | Pinned repo with descriptive tagline | Repo root |
| Portfolio website (future) | Embedded demos, case-study pages | `/portfolio/demos/`, `/portfolio/case-studies/` |

---

## Repo Root README Additions

Add a **Portfolio** section to the repo root `README.md`:

```markdown
## 📂 Portfolio

This project doubles as a professional portfolio. Explore curated
artifacts demonstrating product ownership, technical architecture,
Agile process leadership, and creative direction.

→ [Portfolio Index](portfolio/README.md)
```

---

## Maintenance Cadence

| Action | Frequency |
|---|---|
| Update case studies after feature ships | Per sprint |
| Refresh screenshots after UI changes | As needed |
| Record new demo after milestone | Per milestone |
| Review and prune stale artifacts | Monthly |

---

## Anti-Patterns to Avoid

- **Dead links** — CI should lint internal markdown links.
- **Orphaned artifacts** — every file in `/portfolio/` must be linked from
  the portfolio `README.md`.
- **Stale screenshots** — a screenshot older than two sprints without a
  refresh note signals neglect.
- **Wall of text** — case studies over 800 words need an executive summary.
