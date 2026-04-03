# Sprint Planning Experience — Product Brief

## Vision
Planning a sprint is a documented, repeatable ceremony that produces
version-controlled, machine-readable outputs — sprint scope is explicit,
GitHub issues sync automatically, and governance rules are enforced by
the platform rather than by memory.

## Principles
1. **Sprint scope is an artifact.** A manifest declares the sprint goal,
   time boundaries, capacity, and story references. Scope changes are
   visible in version history.
2. **Ceremony is documented.** A runbook defines what happens before,
   during, and after planning — no cold starts.
3. **Sync is automatic.** Story artifacts flow to the project board
   without manual intervention after merge.
4. **Governance is enforced.** Branch protection and merge rules are
   platform-level, not honor-system.

## Success Criteria
- Sprint scope is version-controlled and auditable
- Planning follows a documented, repeatable checklist
- GitHub issues are created or updated automatically on merge
- No code reaches main without passing CI and required reviews

## Capability Map
| Capability              | Story                             | Location  |
|-------------------------|------------------------------------|-----------|
| Sprint manifests        | Sprint Manifest                    | backlog   |
| Planning ceremony       | Planning Runbook                   | sprint-2  |
| Automatic issue sync    | Post-Merge Sprint Bootstrap        | sprint-2  |
| Branch governance       | Merge Protection Governance        | sprint-2  |

## Epic
Sprint Planning Experience
