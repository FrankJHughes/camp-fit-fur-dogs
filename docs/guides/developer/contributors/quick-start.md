# Contributor Quick‑Start

A fast guide for new contributors.

---

# 1. Adding a New Slice

See: Adding a New Feature Slice Guide

---

# 2. Running the API

````bash
dotnet run --project src/CampFitFurDogs.Api
````

---

# 3. Running the Frontend

````bash
npm run dev
````

---

# 4. Running Tests

````bash
dotnet test
npm test
````

---

# 5. Debugging CI

- Check Neon branch  
- Check db-conn.txt  
- Check Render preview  
- Check Vercel preview  
- Run integration tests locally

---

# 6. Folder Structure

Backend: `src/CampFitFurDogs.*`  
Frontend: `frontend/src/*`  
Docs: `docs/guides/*`

---

# 7. Rules to Remember

- One slice = one verb + one noun  
- Endpoints are pure  
- Queries use readers  
- Commands use repositories  
- No domain logic in Infrastructure  
- No direct environment access  
- All errors use ProblemDetails
