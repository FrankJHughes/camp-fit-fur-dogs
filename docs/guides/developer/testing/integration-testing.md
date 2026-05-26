# Integration Testing Guide

This guide explains how integration tests run for Camp Fit Fur Dogs and how they interact with Neon.

## What Integration Tests Cover

- API + database behavior
- EF Core migrations against a real Postgres instance
- End-to-end flows for key vertical slices (e.g., customers, dogs)

## CI Behavior (GitHub Actions)

On every pull request targeting main:

1. A Neon branch database is created.
2. EF Core migrations are applied.
3. The CampFitFurDogs.IntegrationTests project is executed.
4. The Neon branch database is deleted.

## Local Integration Testing

Run integration tests locally:

.\scripts\integration\Run-IntegrationTests.ps1 -ConnectionString "<your connection string>"

## Branch Protection

The main branch should require:

- Pull requests
- The Integration Tests workflow to pass
- No force pushes

## Summary

Integration tests ensure main is always validated end-to-end.
