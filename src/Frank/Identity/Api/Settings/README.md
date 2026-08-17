# Identity API — Settings

The **Settings** folder contains configuration objects used by the Identity API
platform.  
These settings are bound from application configuration (e.g., `appsettings.json`)
and provide strongly‑typed access to values required by the Identity subsystem.

Although these settings live in the Identity API assembly, they are **not limited
to identity endpoints**.  
They reside here because the Identity platform needs reliable, centralized
configuration for cross‑application flows such as login, logout, and callback
navigation.

---

## Folder Structure

```
Settings/
└── FrontendSettings.cs
```

---

# FrontendSettings

Represents configuration describing how the Identity API should communicate with
the frontend application.

### Purpose

Identity flows often require redirecting the user back to the frontend
application — for example:

- After login  
- After logout  
- After OIDC callback processing  
- When generating frontend‑facing navigation links  

To support these flows, the Identity subsystem needs a stable, environment‑aware
base URL for the frontend.

### Properties

#### `BaseUrl : string`
The absolute base URL of the frontend application.

Examples:

- `https://app.example.com`
- `https://localhost:5173`

This value is used when constructing redirect targets and cross‑application
navigation URLs.

### Why It Lives Here

- Identity login/logout flows must redirect to the frontend  
- Identity callback endpoints must know where to send the user next  
- The Identity API should not hard‑code frontend URLs  
- Configuration must be centralized and environment‑specific  

---

# Summary

The Settings folder provides:

### **FrontendSettings**
A strongly‑typed configuration object describing the frontend application’s base
URL, used for redirect flows and cross‑application navigation.

This ensures that identity flows remain environment‑aware, configurable, and
free of hard‑coded URLs.

---
