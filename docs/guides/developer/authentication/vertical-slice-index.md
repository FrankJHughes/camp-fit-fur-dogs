# Vertical Slice Index

This index lists all vertical slices in Camp Fit Fur Dogs and links to their corresponding guides.  
It helps contributors discover existing slices and understand where new slices belong.

Vertical slices follow the rule:

> One verb, one noun. Not a feature area.

Each slice encapsulates **API → Application → Domain → Infrastructure** behavior for a single use case, following **Vertical Slice Architecture** and **API Endpoint Purity**.

---

# Authentication Slices

| Slice | Description | Guide |
|-------|-------------|--------|
| OwnerLogin | Initiates OIDC login | [Authentication Operations Guide](ca://s?q=Show_authentication_operations_guide) |
| AuthCallback | Handles callback → identity → session | [Authentication Operations Guide](ca://s?q=Show_authentication_operations_guide) |
| Logout | Ends session | [Session Management Guide](ca://s?q=Show_session_management_guide) |

---

# Customer Slices

| Slice | Description | Guide |
|-------|-------------|--------|
| CreateAccount | Owner onboarding | [Create Account Slice Guide](ca://s?q=Show_create_account_feature_slice_guide) |
| GetCustomerProfile | Query customer profile | [Query Slice Guide](ca://s?q=Show_query_slice_guide) |

---

# Dog Slices

| Slice | Description | Guide |
|-------|-------------|--------|
| RegisterDog | Register a dog | [Register Dog Slice Guide](ca://s?q=Show_register_dog_slice_guide) |
| GetDogProfile | Query dog profile | [Dog Profile Slice Guide](ca://s?q=Show_dog_profile_slice_guide) |

---

# Infrastructure Slices

Infrastructure slices represent **cross‑cutting API behavior** implemented as vertical slices when appropriate.

| Slice | Description | Guide |
|-------|-------------|--------|
| RateLimiting | API throttling | [Infrastructure Slice Guide](ca://s?q=Show_infrastructure_slice_guide) |
| SecurityHeaders | HTTP security headers | [Infrastructure Slice Guide](ca://s?q=Show_infrastructure_slice_guide) |
| CORS | Web origin governance | [CORS Governance Guide](ca://s?q=Show_cors_governance_guide) |

---

# Adding a New Slice

See: **[Adding a New Feature Slice Guide](ca://s?q=Show_add_new_feature_slice_guide)**  
