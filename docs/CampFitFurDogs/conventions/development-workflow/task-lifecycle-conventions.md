# Task Lifecycle Conventions (CampFitFurDogs)

Tasks represent the smallest unit of deliverable work.  
Each task implements a single vertical slice of a story.

---

## States

Valid states:

- `backlog`
- `ready`
- `in progress`
- `in review`
- `done`

Tasks must not introduce custom states.

---

## Transitions

Tasks must:

- move to `ready` only after grooming  
- move to `in progress` only when a developer begins work  
- move to `in review` only when a PR is opened  
- move to `done` only when merged  

---

## Requirements

Tasks must:

- link to exactly one story  
- include acceptance criteria  
- include a summary  
- include a vertical slice description  
- be implemented in a single PR  

Tasks must not:

- contain multiple slices  
- contain story-level content  
- be merged without review  
