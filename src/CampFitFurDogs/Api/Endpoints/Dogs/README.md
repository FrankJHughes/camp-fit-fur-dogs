# 🐶 Dogs — API Endpoints

The **Dogs** endpoint folder contains all HTTP API endpoints responsible for
managing dog profiles within the Camp Fit Fur Dogs platform.  
Each endpoint implements `IEndpoint` and is automatically discovered and mapped
through the Frank.Core endpoint registration system.

Endpoints enforce ownership, validate authenticated users, and delegate business
logic to the application layer via CQRS commands and queries.

All routes are mapped **relative to the `/api` group**, which is created in
Program.cs:

```csharp
app.MapRegisteredApiEndpoints("/api")
    .WithTags("API")
    .WithDescription("Camp Fit Fur Dogs API");
```

---

## 📘 Endpoints

### **EditDogEndpoint**

**Route:** `PUT /dogs/{id}`  
(Automatically becomes `PUT /api/dogs/{id}`)

Edits an existing dog owned by the authenticated user.

Workflow:

1. Resolve current user  
2. Fetch dog via `GetDogQuery` to ensure ownership  
3. Dispatch `EditDogCommand`  
4. Return `204 No Content`

---

### **GetDogEndpoint**

**Route:** `GET /dogs/{id}`  
(Automatically becomes `GET /api/dogs/{id}`)

Returns the full profile of a dog owned by the authenticated user.

Workflow:

1. Resolve current user  
2. Fetch dog via `GetDogQuery`  
3. Return `404` if not found  
4. Return dog profile (`200 OK`)

---

### **ListDogsByCurrentUserEndpoint**

**Route:** `GET /dogs`  
(Automatically becomes `GET /api/dogs`)

Returns all dogs belonging to the authenticated user.

Workflow:

1. Resolve current user  
2. Fetch list via `ListDogsByOwnerQuery`  
3. Map results to `GetDogSummaryEndpointResponse`  
4. Return `ListDogsByCurrentUserEndpointResponse`

---

### **RegisterDogEndpoint**

**Route:** `POST /dogs`  
(Automatically becomes `POST /api/dogs`)

Registers a new dog under the authenticated user.

Workflow:

1. Resolve current user  
2. Parse request  
3. Dispatch `RegisterDogCommand`  
4. Return `201 Created` with `RegisterDogEndpointResponse`

---

### **RemoveDogEndpoint**

**Route:** `DELETE /dogs/{id}`  
(Automatically becomes `DELETE /api/dogs/{id}`)

Removes a dog owned by the authenticated user.

Workflow:

1. Resolve current user  
2. Fetch dog via `GetDogQuery`  
3. Return `404` if not found  
4. Dispatch `RemoveDogCommand`  
5. Return `204 No Content`

---

## 🧩 Registration

### **ServiceCollectionExtensions**

Registers all dog‑related endpoints using Frank.Core’s discovery system.

- Scans the assembly containing `CampFitFurDogs.Api.AssemblyMarker`
- Filters to only include endpoint implementations under the Dogs namespace
- Adds them via `AddFrankCoreApiEndpoints`

Call from your API host:

```csharp
services.AddCampFitFurDogsApiEndpointsDogs();
```

---

## 🧭 Design Principles

Dog endpoints follow these principles:

- **Ownership enforcement** — every operation validates the authenticated user  
- **CQRS separation** — endpoints delegate logic to commands/queries  
- **Minimal API style** — clean, lightweight endpoint definitions  
- **Consistent response shapes** — DTOs defined in the abstractions layer  
- **Automatic discovery** — no manual endpoint registration required  
- **Group‑relative routing** — endpoints never hard‑code `/api`  

---

## ✅ Summary

This folder defines the full HTTP API surface for dog management:

- Register  
- Edit  
- Fetch  
- List  
- Remove  

These endpoints form the operational layer of the Camp Fit Fur Dogs dog‑management feature set.
