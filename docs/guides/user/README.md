# Camp Fit Fur Dogs — User Guide  
Product‑specific usage handbook

Welcome to the **Camp Fit Fur Dogs User Guide** — the handbook for developers, engineers, and teams who *use* the Camp Fit Fur Dogs API and system **built on top of the Frank Framework**.

This guide documents **only product‑specific behavior**.  
All framework‑level behavior (DI, hosting, startup, validation, observability, testing harness, etc.) is documented in:

- **Frank User Guide**  
- **Frank Developer Guide**  
- **Frank Tester Guide**

Camp Fit Fur Dogs (CFFD) is a product built using Frank.  
This guide explains how to interact with *this product* correctly.

---

# 1. Purpose of This Guide

This guide provides:

- Product overview  
- Product workflows  
- Product API usage  
- Product authentication rules  
- Product‑specific observability events  
- Product‑specific configuration and environment expectations  

This guide does **not** restate Frank’s rules.  
Frank governs the framework; CFFD governs the product.

---

# 2. Product Overview

Camp Fit Fur Dogs is a platform for:

- Owner account management  
- Dog profile management  
- Booking appointments  
- Staff operations  
- Customer communication  
- Operational workflows  

The product exposes:

- A public API  
- A secure owner‑facing UI  
- Staff‑only operational endpoints  

All behavior is deterministic and governed by Frank.

---

# 3. Authentication & Authorization (Product‑Specific)

Camp Fit Fur Dogs uses:

- OIDC login for owners  
- Session cookies issued by the API  
- Role‑based authorization for staff endpoints  

### 3.1 Owner Authentication

Owners authenticate via:

1. OIDC login  
2. Frank session cookie issuance  
3. Product‑specific identity mapping  

### 3.2 Staff Authentication

Staff authenticate via:

- Staff identity provider  
- Staff roles (`Admin`, `Staff`)  

### 3.3 Authorization Rules

- Owners may only access their own data  
- Staff may access operational endpoints  
- Admins may access administrative endpoints  

Authorization is enforced at the API boundary.

---

# 4. Using the API

The Camp Fit Fur Dogs API is organized into feature‑based slices:

```
/owners
/dogs
/bookings
/staff
/operations
```

Each slice exposes:

- Commands (POST)  
- Queries (GET)  
- Updates (PUT/PATCH)  
- Deletions (DELETE)  

### 4.1 Request/Response Shapes

All API endpoints use:

- JSON request bodies  
- JSON responses  
- Frank’s error shaping  
- Frank’s validation pipeline  

### 4.2 Example: Create Booking

**POST** `/bookings`

```json
{
  "dogId": "123",
  "start": "2025-01-01T10:00:00Z",
  "end": "2025-01-01T11:00:00Z",
  "serviceType": "Grooming"
}
```

Response:

```json
{
  "bookingId": "abc123",
  "status": "Confirmed"
}
```

### 4.3 Error Handling

Errors follow Frank’s error boundary rules:

- Validation errors → 400  
- Authorization errors → 403  
- Not found → 404  
- Domain rule violations → 409  
- Unexpected errors → 500 (safe, non‑revealing)  

---

# 5. Using the UI (If Applicable)

The Camp Fit Fur Dogs UI provides:

- Owner dashboard  
- Dog profile management  
- Booking creation and management  
- Staff operational tools  

UI behavior is product‑specific but built on Frank’s API.

### 5.1 Owner UI

Owners can:

- Register  
- Verify email  
- Add dogs  
- Create bookings  
- Manage bookings  

### 5.2 Staff UI

Staff can:

- View daily schedule  
- Manage bookings  
- Manage customer accounts  
- Perform operational tasks  

---

# 6. Product‑Specific Observability

Frank provides observability primitives.  
Camp Fit Fur Dogs defines **product‑specific events and metrics**.

### 6.1 Event Naming

All events follow:

```
cffd.<feature>.<action>
```

Examples:

- `cffd.bookings.create.started`  
- `cffd.bookings.create.completed`  
- `cffd.owners.register.started`  
- `cffd.owners.register.failed`  

### 6.2 Metric Naming

All metrics follow:

```
cffd.<feature>.<metric_name>
```

Examples:

- `cffd.bookings.count`  
- `cffd.owners.registration.count`  

### 6.3 Observability Rules

- No secrets, tokens, or PII in payloads  
- No manual correlation ID creation  
- No vendor‑specific logging or metrics  
- Use Frank’s `ITraceEvents` and `IMetrics`  

---

# 7. Product Configuration & Environment

Camp Fit Fur Dogs uses the following environment keys:

```
CFFD__Database__ConnectionString
CFFD__Auth__Authority
CFFD__Auth__Audience
CFFD__Email__ApiKey
CFFD__Frontend__BaseUrl
```

These are consumed through Frank’s:

- `IEnvironment`  
- `IConfiguration`  

No direct environment variable access is allowed.

---

# 8. What Camp Fit Fur Dogs Users Should *Not* Do

Users of the product should **not**:

- bypass the API  
- rely on internal endpoints  
- assume Frank behavior differs per product  
- store sensitive data in localStorage  
- rely on undocumented fields  
- assume booking rules are client‑enforced  
- assume authorization is handled by the UI  

All business rules are enforced server‑side.

---

# 9. Summary

The Camp Fit Fur Dogs User Guide explains:

- how to interact with the product  
- how to authenticate and authorize  
- how to use the API  
- how to use the UI  
- how to understand product‑specific observability  
- how to work with product‑specific configuration  

Frank provides the governed, deterministic foundation.  
Camp Fit Fur Dogs provides the product logic built on top of it.
