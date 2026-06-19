# Branching Conventions (CampFitFurDogs)

CampFitFurDogs uses a deterministic branching model aligned with story → task → PR workflow.

---

## Branch Types

Branches must follow these patterns:

- Story branches: `story/US-###-short-title`
- Task branches: `task/NNN-short-title`
- Hotfix branches: `hotfix/NNN-short-title`

Branches must not:

- contain uppercase letters  
- contain spaces  
- contain personal names  
- contain environment names  

---

## Story Branch Rules

Story branches must:

- be created only when a story enters “in progress”
- contain only tasks belonging to that story
- be short‑lived

Story branches must not:

- contain unrelated tasks  
- be merged directly into `main`  

---

## Task Branch Rules

Task branches must:

- be created from the story branch  
- contain a single vertical slice  
- be merged via PR only  

Task branches must not:

- contain multiple tasks  
- contain unrelated changes  
