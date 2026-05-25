# Secrets Setup Guide

This guide explains how developers securely manage secrets—especially the API connection string—during local development.  
The project uses **dotnet user‑secrets**, which stores secrets in an OS‑protected location outside the repository.

Secrets are **never** committed to source control.

---

## Why We Use User Secrets

- Keeps sensitive values out of the repository  
- Works automatically in Development  
- Integrates with .NET configuration precedence  
- Supports script‑first workflows  
- Safe for all contributors (each developer has their own secret store)

---

## 1. Initialize User Secrets (first time only)

From the `CampFitFurDogs.Api` project directory:

```
dotnet user-secrets init
```

This adds a `UserSecretsId` to the `.csproj`.  
It is safe to commit—no secrets are stored in the repo.

---

## 2. Set the Neon (or other) connection string

```
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<your connection string>"
```

Example:

```
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=...;Port=5432;Database=...;Username=...;Password=...;Ssl Mode=Require;Trust Server Certificate=true;"
```

The value is stored securely in the OS secret store.

---

## 3. Verify the secret

```
dotnet user-secrets list
```

You should see:

```
ConnectionStrings:DefaultConnection = ********
```

---

## 4. Clearing or Switching Secrets

To remove the connection string:

```
dotnet user-secrets remove "ConnectionStrings:DefaultConnection"
```

To clear all secrets for this project:

```
dotnet user-secrets clear
```

---

## 5. Using Scripts to Toggle Databases

The repository includes helper scripts under `scripts/database/`:

- `Activate-NeonDb.ps1` — sets the Neon connection string  
- `Activate-LocalDb.ps1` — removes the secret so local Postgres is used  

These scripts call `dotnet user-secrets` under the hood.

---

## 6. Behavior When Cloning the Repository

When a new developer clones the repo:

- They get the `UserSecretsId`  
- They **do not** get any secrets  
- They must run the setup steps above  

This ensures secrets remain local and secure.

---

## Summary

- Secrets never enter source control  
- User secrets provide a safe, OS‑protected store  
- Scripts automate switching between Neon and local databases  
- The setup is simple, repeatable, and secure for all contributors
