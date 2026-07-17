# Pull Request Conventions (CampFitFurDogs)

Pull requests represent the completion of a single task.  
They must be small, reviewable, and deterministic.

---

## PR Requirements

PRs must:

- reference exactly one task  
- reference the story in the template  
- include acceptance criteria covered  
- include screenshots for UI changes  
- pass all CI checks  
- be reviewed by at least one engineer  

PRs must not:

- include unrelated changes  
- include formatting-only changes mixed with logic  
- merge without review  

---

## PR Size

PRs must be:

- small  
- focused  
- reviewable within 10 minutes  

Large PRs must be split into multiple tasks.

---

## Merge Rules

PRs must:

- use squash merge  
- produce a single commit on the story branch  
- delete the task branch after merge  
