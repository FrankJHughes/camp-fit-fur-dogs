# Issue Lifecycle Conventions (CampFitFurDogs)

Issues represent tasks.  
They must follow a deterministic lifecycle aligned with story workflow.

---

## States

Valid states:

- `backlog`
- `ready`
- `in progress`
- `in review`
- `done`

Issues must not introduce custom states.

---

## Transitions

Issues must:

- move to `ready` only after story grooming  
- move to `in progress` only when a developer starts work  
- move to `in review` only when a PR is opened  
- move to `done` only when merged  

---

## Requirements

Issues must:

- link to the story  
- include acceptance criteria  
- include a summary  
- include a vertical slice description  

Issues must not:

- contain multiple tasks  
- contain story-level content  
