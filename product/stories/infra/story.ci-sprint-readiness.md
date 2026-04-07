# US-YY — CI Enforcement: Sprint Readiness Requires Story YAML

## Story ID
story.usYY.ci-sprint-readiness

## Parent Epic
epic.documentation-governance

## Intent
As a product owner,  
I want CI to enforce sprint-level readiness rules,  
so that a sprint cannot be marked Ready unless all included stories have complete Story YAML.

## Problem
Sprint readiness currently depends on human discipline.  
If a story is added to a sprint without YAML, or with incomplete planning metadata, the sprint can appear Ready even though it violates governance rules.

This creates:
- drift risk  
- inconsistent sprint planning  
- emotionally unsafe sprint commitments  
- unclear readiness signals  

## Outcome
CI validates sprint readiness by ensuring:
- Every story listed in sprint YAML has a corresponding Story YAML file  
- Story YAML contains required planning metadata:
  - sprint assignment  
  - story_points  
  - dependencies resolved  
  - ready: true  
- Sprint readiness checks fail if any story is missing YAML or metadata  

## Acceptance Criteria
- CI job runs on PRs that modify sprint YAML or story YAML
- CI fails if:
  - A story in sprint YAML has no Story YAML file
  - Story YAML is missing required fields
  - Dependencies are unresolved
  - story_points is null
  - ready is false
- CI outputs clear, actionable error messages
- CI links to governance documentation for sprint readiness rules
- CI prevents merging a sprint that is not Ready

## Emotional Safety Guarantees
- Teams cannot accidentally commit to an unsafe sprint
- Contributors receive clear, deterministic feedback
- Sprint readiness becomes objective, not interpretive
- Governance rules are enforced consistently and compassionately

