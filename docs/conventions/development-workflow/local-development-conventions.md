# Local Development Conventions (CampFitFurDogs)

Local development must be deterministic, environment‑free, and aligned with the
preview environment model.

---

## Requirements

Local development must:

- use `docker compose` for backend + database  
- use seeded data  
- use fake external integrations  
- use local frontend dev server  
- use `.env.local` for local-only overrides  

Local development must not:

- depend on real identity providers  
- depend on real external services  
- require manual database setup  

---

## Database Rules

Local database must:

- be ephemeral  
- be seeded deterministically  
- not require migrations to be applied manually  

---

## Frontend Rules

Frontend must:

- run via `npm run dev`  
- use local API URLs  
- not call preview or production APIs  

---

## Prohibitions

Local development must not:

- require environment variables beyond `.env.local`  
- require manual configuration of hosting providers  
