# Story YAML Governance Rules

## Story Markdown (planning/stories/*.md)
- Backlog artifact.
- Created as soon as the story exists.
- Contains product intent, value, emotional safety, acceptance criteria.
- Does NOT require YAML.

## Story YAML (planning/fragments/stories/*.yml)
- Sprint-planning artifact.
- Contains planning metadata: sprint, readiness, dependencies, story points.
- MAY be created early, but early creation is NOT encouraged.
- Tooling SHOULD support YAML creation at sprint-planning time.

## Governance Rule
Story YAML is optional before sprint planning, but REQUIRED for any story included in a sprint.
A sprint cannot be considered Ready if it contains stories without YAML.

