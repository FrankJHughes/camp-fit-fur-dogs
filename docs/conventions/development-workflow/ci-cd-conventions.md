# CI/CD Conventions (CampFitFurDogs)

CI/CD enforces deterministic builds, tests, and preview deployments.  
Workflows must remain stable, predictable, and aligned with Frank’s hosting model.

---

## CI Requirements

CI must:

- run on every PR  
- run tests  
- run linters  
- run type checks  
- build backend and frontend  
- validate migrations  
- validate story/task/PR templates  

CI must not:

- depend on environment variables  
- depend on external services  
- hit real identity providers  

---

## Preview Environment Requirements

Preview environments must:

- be created for every PR  
- use Render hosting provider  
- use ephemeral databases  
- use seeded data  
- expose frontend and backend URLs via artifacts  

Preview environments must not:

- persist data  
- share state across PRs  
- require manual configuration  

---

## Merge Requirements

A PR may merge only when:

- CI passes  
- preview deploy succeeds  
- review is complete  
- acceptance criteria are satisfied  
