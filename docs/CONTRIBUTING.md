# Contributing to Camp Fit Fur Dogs

> **Planning conventions** — for artifact organization, story lifecycle,
> and naming rules, see [Planning Conventions](../planning/README.md).

Thanks for contributing! This guide keeps our workflow consistent and our history clean.

---

## Branch Naming

| Type | Pattern | Example |
|------|---------|---------|
| Feature | `feature/<story-slug>` | `feature/ci-baseline-build-and-test` |
| Bug fix | `fix/<description>` | `fix/test-runner-timeout` |
| Docs | `docs/<description>` | `docs/contributing-and-pr-template` |
| Chore | `chore/<description>` | `chore/planning-timestamps` |

Always branch from `main`. Keep branch names lowercase, kebab-case.

---

## Commit Messages

We follow [Conventional Commits](https://www.conventionalcommits.org/):

```
<type>: <short summary>

<optional body � what and why, not how>
```

### Types

| Type | When |
|------|------|
| `feat` | New functionality |
| `fix` | Bug fix |
| `docs` | Documentation only |
| `chore` | Tooling, config, maintenance |
| `test` | Adding or updating tests |
| `refactor` | Code change that neither fixes nor adds |

### Rules

- Subject line: imperative mood, no period, = 72 characters
- Body: wrap at 72 characters, explain *what* and *why*
- Reference the issue: `Closes #N` in the PR body, not the commit

---

## Pull Request Workflow

1. **One story = one PR.** Don't bundle unrelated changes.
2. **Fill out the PR template.** Every section exists for a reason.
3. **Self-review before requesting review.** Read the diff as if you didn't write it.
4. **Squash-merge into `main`.** Keep the history linear.
5. **Delete the branch after merge.**

---

## Definition of Done

A story is done when:

- [ ] Code compiles with zero warnings
- [ ] All tests pass
- [ ] PR template is filled out completely
- [ ] PR is squash-merged into `main`
- [ ] CI badge is green
