# Create Account Feature Slice Guide

This guide explains the **Create Account** feature slice implemented in **US‑126 (Create Account Page)**.  
It documents the *end‑to‑end vertical slice*, including UI, validation, API interaction, and backend behavior.

This guide does **not** define rules, boundaries, or architectural decisions — those live in governance, conventions, and ADRs.  
This guide focuses solely on **how the Create Account feature works today**.

---

# Feature Slice Overview

The Create Account slice includes:

- A frontend page (`/create-account`)
- A React Hook Form + Zod validation schema
- A typed API client method
- A backend endpoint for account creation
- A success redirect (`/account/created`)
- Tests for UI, validation, and API behavior

This slice follows the standard vertical‑slice pattern:

````  
UI → Validation → API Client → Backend Endpoint → Domain → Persistence
````

---

# Frontend Components

### 1. Page Component  
Located at:

````  
app/create-account/page.tsx
````

Responsibilities:

- Render the form  
- Handle submission  
- Display errors  
- Redirect on success  

### 2. Form Component  
Located at:

````  
components/forms/CreateAccountForm.tsx
````

Responsibilities:

- Initialize RHF  
- Bind Zod schema  
- Render fields  
- Display validation errors  
- Call API client  

---

# Validation Layer

Validation is performed using a Zod schema:

````  
const CreateAccountSchema = z.object({
  email: z.string().email(),
  password: z.string().min(8),
  confirmPassword: z.string(),
}).refine(data => data.password === data.confirmPassword, {
  message: "Passwords must match",
  path: ["confirmPassword"],
});
````

Validation occurs:

- On submit  
- On blur  
- On change (for some fields)  

Errors are displayed inline using shared UI components.

---

# API Client Layer

The form calls the API client method:

````  
api.createAccount({ email, password })
````

The API client:

- Sends a POST request  
- Parses JSON  
- Normalizes errors  
- Returns a typed result object  

Example:

````  
const result = await api.createAccount(data);

if (!result.ok) {
  setError("root", { message: result.error });
}
````

---

# Backend Endpoint

The backend exposes:

````  
POST /api/account
````

Responsibilities:

- Validate request body  
- Create Owner record  
- Return success or validation errors  

The endpoint does **not** log the user in — that is handled by US‑110.

---

# Domain Behavior

The domain layer:

- Creates a new Owner aggregate  
- Validates invariants  
- Persists the Owner  
- Returns the new `OwnerId`  

No session is created here.

---

# Success Flow

On success:

1. API returns `{ ok: true }`
2. Form redirects to `/account/created`
3. User sees confirmation screen

---

# Error Handling

### Client‑Side Errors  
- Invalid email  
- Weak password  
- Password mismatch  

### API Validation Errors  
- Email already exists  
- Invalid input  

### System Errors  
- Network failure  
- Server error  

System errors appear as a root‑level message.

---

# Testing the Feature Slice

### 1. Unit Tests  
- Zod schema  
- Password mismatch  
- Required fields  

### 2. Integration Tests  
- Successful submission  
- API validation errors  
- Network errors  
- Disabled submit button during loading  

Example:

````  
it("redirects on successful account creation", async () => {
  mockApi.createAccount.mockResolvedValue({ ok: true });

  await user.click(submitButton);

  expect(mockRouter.push).toHaveBeenCalledWith("/account/created");
});
````

---

# Troubleshooting

### Form does not submit  
- Check Zod resolver  
- Check API client import  

### API always returns error  
- Check backend endpoint URL  
- Check CORS configuration  

### Validation not triggering  
- Ensure RHF + Zod resolver is wired correctly  

---

# Related Documents

- **[Create Account Form](ca://s?q=Generate_Create_Account_Form_Guide)**  
- **[Authentication Architecture](ca://s?q=Generate_Authentication_Architecture_Guide)**  
- **[Session Management](ca://s?q=Generate_Session_Management_Guide)**  
- **[Identity Mapping](ca://s?q=Generate_Identity_Mapping_Guide)**  
