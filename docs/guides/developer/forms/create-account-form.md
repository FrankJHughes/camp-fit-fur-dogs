# Create Account Form Guide

This guide explains how the **Create Account** form works based on the implementation completed for **US‑126 (Create Account Page)**.  
It documents the *runtime behavior*, *validation flow*, *API interaction*, and *developer workflow* for the create‑account UI.

This guide does **not** define rules, boundaries, or architectural decisions — those live in governance, conventions, and ADRs.  
This guide focuses solely on **how the Create Account form works today**.

---

# Purpose of the Create Account Form

The Create Account form allows a new Owner to:

- Enter their account details  
- Submit the form  
- Trigger the backend account‑creation endpoint  
- Receive validation feedback  
- Navigate to the next step in onboarding  

This guide documents the UI and client‑side behavior only.

---

# Form Architecture

The Create Account form uses:

- **React Hook Form (RHF)** for form state  
- **Zod** for schema validation  
- **API client** for backend communication  
- **Shared UI components** for inputs, errors, and buttons  

This architecture ensures:

- Deterministic validation  
- Declarative form structure  
- Consistent error handling  
- Testability  

---

# Validation Schema

Validation is performed using a Zod schema.

The schema validates:

- Email format  
- Password length  
- Required fields  
- Matching password confirmation  

Example structure:

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

The schema is the **single source of truth** for client‑side validation.

---

# Form Fields

The form includes:

| Field | Type | Notes |
|-------|------|--------|
| Email | Text | Validated by Zod |
| Password | Password | Must meet minimum length |
| Confirm Password | Password | Must match password |
| Submit Button | Action | Disabled during submission |

All fields use shared UI components.

---

# Submit Flow

When the user submits the form:

1. RHF validates the form using the Zod schema  
2. If validation fails → errors are shown inline  
3. If validation succeeds → API client is called  
4. API returns success or error  
5. UI responds accordingly  

The submit handler looks like:

````  
const onSubmit = async (data) => {
  const result = await api.createAccount(data);

  if (result.ok) {
    router.push("/account/created");
  } else {
    setError("root", { message: result.error });
  }
};
````

---

# API Interaction

The form calls the API client method:

````  
api.createAccount({ email, password })
````

The API client:

- Sends a POST request  
- Handles JSON parsing  
- Normalizes error responses  
- Returns a typed result object  

Example:

````  
const result = await api.createAccount(data);

if (!result.ok) {
  // result.error contains a user‑friendly message
}
````

---

# Error Handling

The form handles three categories of errors:

### 1. Client‑Side Validation Errors  
- Email format  
- Password length  
- Password mismatch  

### 2. API Validation Errors  
- Email already exists  
- Weak password  
- Invalid input  

These appear as inline errors.

### 3. System Errors  
- Network failure  
- Server error  
- Unexpected response  

These appear as a root‑level error message.

---

# Loading & Disabled States

During submission:

- Submit button is disabled  
- Inputs are disabled  
- A loading indicator is shown  

This prevents duplicate submissions.

---

# Navigation Behavior

On success:

- The user is redirected to `/account/created`  
- The next step in onboarding begins  

On failure:

- The form remains visible  
- Errors are shown inline  

---

# Testing the Create Account Form

The form is tested at two levels:

### 1. Unit Tests (Zod + RHF)  
- Schema validation  
- Password mismatch  
- Required fields  
- Error messages  

### 2. Integration Tests (UI + API client)  
- Successful submission  
- API validation errors  
- Network errors  
- Disabled state behavior  

Example test:

````  
it("shows an error when passwords do not match", async () => {
  await user.type(emailInput, "test@example.com");
  await user.type(passwordInput, "password123");
  await user.type(confirmInput, "different");
  await user.click(submitButton);

  expect(screen.getByText("Passwords must match")).toBeInTheDocument();
});
````

---

# Troubleshooting

### Form submits but nothing happens  
- Check API client import  
- Check router navigation  
- Check Zod schema refinement  

### API always returns an error  
- Check backend endpoint URL  
- Check CORS configuration  
- Check request body shape  

### Validation not triggering  
- Ensure `resolver: zodResolver(CreateAccountSchema)` is set  

---

# Related Documents

- **[Create Account Feature Slice](ca://s?q=Generate_Create_Account_Slice_Guide)**  
- **[Authentication Architecture](ca://s?q=Generate_Authentication_Architecture_Guide)**  
- **[Authentication Operations](ca://s?q=Generate_Authentication_Operations_Guide)**  
- **[Session Management](ca://s?q=Generate_Session_Management_Guide)**  
