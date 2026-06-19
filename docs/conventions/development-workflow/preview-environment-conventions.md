# Preview Environment Conventions (CampFitFurDogs)

Preview environments provide isolated, ephemeral deployments for every PR.  
They must be deterministic, reproducible, and aligned with Frank’s hosting model.

---

## Requirements

Preview environments must:

- deploy automatically for every PR  
- use Render hosting provider  
- use ephemeral databases  
- use seeded data  
- expose backend and frontend URLs via artifacts  
- run migrations automatically  
- run with no manual configuration  

Preview environments must not:

- persist data  
- share state across PRs  
- require environment variables beyond those provided by Render  
- require manual intervention  

---

## Database Rules

Preview databases must:

- be created per PR  
- be seeded deterministically  
- be destroyed when the PR closes  
- not share data with local or production environments  

---

## Artifact Requirements

Preview deploys must publish:

- `pr-{n}-db/db-conn.txt`  
- `pr-{n}-frontend/frontend-url.txt`  

These artifacts must be consumed by:

- CI validation  
- manual QA  
- automated preview tests  

---

## Prohibitions

Preview environments must not:

- use production secrets  
- use production identity providers  
- use shared infrastructure  
- require manual configuration of hosting providers  
