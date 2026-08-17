# Endpoints Map

This document summarizes the HTTP endpoints exposed by the Camp Fit Fur Dogs API.  
All endpoints follow the platform conventions provided by `Frank.Core` and `Frank.Identity`, including authentication, routing, and typed responses.

## Dogs endpoints

The dog feature exposes a set of CRUD-style operations. Each endpoint delegates to an application handler and returns a typed response model.

---

### Register a dog

```http
POST /api/dogs
Content-Type: application/json
Authorization: Bearer <token>

{
  "name": "Buddy",
  "breed": "Golden Retriever",
  "dateOfBirth": "2020-01-15",
  "sex": "Male"
}
```

**Response:** `201 Created`

```json
{
  "id": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx"
}
```

---

### Get a dog

```http
GET /api/dogs/{dogId}
Authorization: Bearer <token>
```

**Response:** `200 OK`

```json
{
  "id": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
  "name": "Buddy",
  "breed": "Golden Retriever",
  "dateOfBirth": "2020-01-15",
  "sex": "Male"
}
```

---

### List dogs by owner

```http
GET /api/dogs?ownerId={ownerId}
Authorization: Bearer <token>
```

**Response:** `200 OK`

```json
[
  {
    "id": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
    "name": "Buddy",
    "breed": "Golden Retriever",
    "dateOfBirth": "2020-01-15",
    "sex": "Male"
  }
]
```

---

### Edit a dog

```http
PUT /api/dogs/{dogId}
Content-Type: application/json
Authorization: Bearer <token>

{
  "name": "Buddy",
  "breed": "Golden Retriever"
}
```

**Response:** `200 OK`

---

### Delete a dog

```http
DELETE /api/dogs/{dogId}
Authorization: Bearer <token>
```

**Response:** `204 No Content`

---

## Notes on API behavior

- All endpoints require authenticated access via the platform identity subsystem.  
- Request and response models are validated and shaped by the application layer.  
- Domain rules (e.g., dog ownership, invariants) are enforced in the domain layer.  
- Persistence is handled by the infrastructure layer through EF Core patterns.  
- The API layer remains thin: no business logic, no persistence logic.

This map reflects the vertical-slice structure: each endpoint corresponds to a feature implemented across API, application, domain, and infrastructure.

