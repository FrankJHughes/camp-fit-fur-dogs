# Story Lifecycle Conventions (CampFitFurDogs)

Stories represent user-facing value and define the scope of work.  
Tasks implement stories; stories do not contain code.

---

## States

Valid states:

- `backlog`
- `ready`
- `in progress`
- `in review`
- `done`

Stories must not introduce custom states.

---

## Transitions

Stories must:

- move to `ready` only after grooming  
- move to `in progress` only when the first task begins  
- move to `in review` only when all tasks are in review  
- move to `done` only when all tasks are merged  

---

## Requirements

Stories must:

- follow the required grammar  
- include acceptance criteria  
- include dependencies  
- include vertical slice boundaries  
- include emotional/legal guarantees when applicable  

Stories must not:

- contain implementation details  
- contain UI mockups  
- contain architectural decisions  
