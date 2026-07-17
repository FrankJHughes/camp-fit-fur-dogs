# Commit Message Conventions (CampFitFurDogs)

Commit messages must be deterministic, structured, and traceable to tasks.

---

## Format

Commit messages must follow:

````text
<type>: <short description>

Refs: #<task-number>
````

Valid types:

- `feat` — new functionality  
- `fix` — bug fix  
- `refactor` — internal change  
- `test` — test-only change  
- `docs` — documentation change  
- `chore` — non-functional change  

---

## Requirements

Commit messages must:

- be written in imperative mood  
- reference exactly one task  
- describe the change clearly  

Commit messages must not:

- reference stories directly  
- contain multiple task references  
- contain unrelated changes  
