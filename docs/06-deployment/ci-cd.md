# CI/CD

This document describes the `docs/06-deployment` area and maps it back to the implementation under `src/CampFitFurDogs/Api` and repository configuration.

## Purpose

This section documents the responsibilities of the CI/CD subsystem in the codebase and keeps the deployment architecture readable for future contributors. It explains how the application is built, tested, packaged, and released across environments.

## Source alignment

- Primary implementation area: `src/CampFitFurDogs/Api` and repository configuration  
- Current folder: `docs/06-deployment`

## What belongs here

- the responsibilities of the CI/CD subsystem  
- the way the CI/CD pipeline connects to the broader platform  
- the runtime and infrastructure collaboration points  
- how build, test, and deployment automation is composed from API to persistence  

## Notes

Keep this document grounded in the actual CI/CD implementation and update it as the source architecture evolves.
