# Release Conventions (CampFitFurDogs)

Releases represent stable, production-ready builds.  
They must be deterministic, traceable, and aligned with CI/CD guarantees.

---

## Release Requirements

Releases must:

- be created from `main` only  
- be triggered by a tagged commit  
- run full CI  
- run full build pipelines  
- run database migrations  
- deploy backend and frontend  
- publish release notes  

Releases must not:

- be created from feature branches  
- skip CI  
- skip migrations  
- contain unreviewed code  

---

## Versioning

Releases must use semantic versioning:

````text
MAJOR.MINOR.PATCH
````

- MAJOR — breaking changes  
- MINOR — new features  
- PATCH — bug fixes  

---

## Release Notes

Release notes must include:

- list of tasks included  
- list of stories completed  
- migration notes (if any)  
- breaking changes (if any)  

Release notes must not include:

- internal implementation details  
- commit logs  
- developer notes  

---

## Hotfixes

Hotfixes must:

- use `hotfix/NNN-short-title` branches  
- be merged into `main`  
- be tagged with a PATCH version  
- run full CI  

Hotfixes must not:

- introduce new features  
- skip review  
