# API Endpoints

The **Endpoints** folder contains all HTTP API endpoint definitions for the
Camp Fit Fur Dogs platform.  
Each endpoint implements `IEndpoint` and is automatically discovered and mapped
through the Frank.Core endpoint discovery system.

Endpoints in this folder are organized by feature area:

- **Dogs** — dog registration, editing, listing, retrieval, and removal  
- **Health** — service health‑check endpoint  
- **Root Registration** — a single extension method that registers all endpoint groups

All routes are mapped **relative to the `/api` group**, which is created in
Program.cs:

```csharp
app.MapRegisteredApiEndpoints("/api")
    .WithTags("API")
    .WithDescription("Camp Fit Fur Dogs API");
```

---

## Folder Structure

```
Endpoints/
│
├── Dogs/
│   ├── EditDogEndpoint.cs
│   ├── GetDogEndpoint.cs
│   ├── ListDogsByCurrentUserEndpoint.cs
│   ├── RegisterDogEndpoint.cs
│   ├── RemoveDogEndpoint.cs
│   └── ServiceCollectionExtensions.cs
│
├── Health/
│   ├── GetHealthEndpoint.cs
│   └── ServiceCollectionExtensions.cs
│
└── ServiceCollectionExtensions.cs
```

---

## Dogs Endpoints

The Dogs endpoints expose the full CRUD surface for managing dog profiles owned
by authenticated users.

### EditDogEndpoint
**PUT /dogs/{id}**  
(Automatically becomes `PUT /api/dogs/{id}`)

Edits an existing dog profile.  
Validates ownership, dispatches `EditDogCommand`, returns `204 No Content`.

### GetDogEndpoint
**GET /dogs/{id}**  
(Automatically becomes `GET /api/dogs/{id}`)

Returns a full dog profile.  
Validates ownership, dispatches `GetDogQuery`, returns `200 OK` or `404`.

### ListDogsByCurrentUserEndpoint
**GET /dogs**  
(Automatically becomes `GET /api/dogs`)

Returns all dogs belonging to the authenticated user.  
Dispatches `ListDogsByOwnerQuery`, maps results to summaries.

### RegisterDogEndpoint
**POST /dogs**  
(Automatically becomes `POST /api/dogs`)

Registers a new dog.  
Dispatches `RegisterDogCommand`, returns `201 Created`.

### RemoveDogEndpoint
**DELETE /dogs/{id}**  
(Automatically becomes `DELETE /api/dogs/{id}`)

Removes a dog owned by the authenticated user.  
Validates ownership, dispatches `RemoveDogCommand`, returns `204 No Content`.

### Dogs ServiceCollectionExtensions
Registers all Dogs endpoints using Frank.Core’s discovery system.

---

## Health Endpoints

### GetHealthEndpoint
**GET /health**  
(Automatically becomes `GET /api/health`)

Anonymous health‑check endpoint returning `{ "Status": "Up" }`.  
Used for uptime monitoring and load balancer probes.

### Health ServiceCollectionExtensions
Registers all Health endpoints using Frank.Core’s discovery system.

---

## Root Endpoint Registration

### ServiceCollectionExtensions (root)
Aggregates all endpoint groups:

```csharp
services.AddCampFitFurDogsApiEndpoints();
```

This ensures that:

- Dogs endpoints  
- Health endpoints  

are all discovered and mapped automatically.

---

## Design Principles

Endpoints in this folder follow these principles:

- **Minimal API style** — clean, lightweight endpoint definitions  
- **CQRS separation** — all business logic flows through commands and queries  
- **Ownership enforcement** — dog operations always validate the authenticated user  
- **Automatic discovery** — no manual route registration required  
- **Feature isolation** — each folder represents a clear vertical slice  
- **Group‑relative routing** — endpoints never hard‑code `/api`  

---

## Summary

The Endpoints folder defines the complete HTTP API surface for Camp Fit Fur Dogs:

- Dog management (register, edit, list, fetch, remove)  
- Health monitoring  
- Unified endpoint registration  

This structure keeps the API modular, discoverable, and aligned with the
Frank.Core endpoint architecture.
