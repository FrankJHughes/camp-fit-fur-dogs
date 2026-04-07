# CI Workflow: Sprint Readiness Validation

## Purpose
Ensure sprint-level Definition of Ready is enforced.

## Rules
- All stories listed in sprint YAML must have Story YAML.
- Story YAML must contain:
  - sprint assignment
  - story_points
  - dependencies resolved
  - ready: true

## Failure Conditions
- Missing Story YAML
- Missing planning metadata
- Unresolved dependencies

