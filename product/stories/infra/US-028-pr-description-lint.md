# PR Description Lint — CI Enforcement of PR Template

## Intent

Add a GitHub Actions workflow that validates every pull request description against the conventions established in `.github/PULL_REQUEST_TEMPLATE.md`, so that missing issue links and incomplete merge checklists are caught by CI before review — not after merge.

## Value

The PR template exists (US-013) and branch protection requires CI to pass (US-020), but nothing validates the PR *description itself*. `gh pr create --body` silently replaces the template, making it easy to ship a PR that never references an issue and never includes the merge checklist. This story closes that gap by making the template machine-enforceable, not just convention-dependent.

## Acceptance Criteria

- [ ] `.github/workflows/pr-lint.yml` triggers on `pull_request` types `opened`, `edited`, and `synchronize`
- [ ] Workflow fails if the PR body does not contain a valid `Closes #<number>` or `Fixes #<number>` issue reference
- [ ] Workflow fails if the PR body is missing any of the 6 merge checklist items from `.github/PULL_REQUEST_TEMPLATE.md`
- [ ] Failure messages name the specific missing element and reference the PR template
- [ ] Workflow passes on a well-formed PR body that includes both the issue link and all checklist items
- [ ] `pr-lint` is added as a required status check in branch protection rules for `main`
- [ ] `CONTRIBUTING.md` is updated to document the new check and how to satisfy it when using `gh pr create --body`

## Out of Scope

- Validating commit message format (future story)
- Auto-populating the PR body from CLI tooling (requires DX architecture decision from US-025)

## Emotional Guarantees

- **EG-02 (No blame)**: Failure messages guide — "PR body is missing an issue link. Add 'Closes #<number>' to connect this PR to its story." — not "You forgot to link an issue."
