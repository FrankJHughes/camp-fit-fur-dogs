# Authentication Overview

The application uses **OIDC-based authentication** via an external identity provider (Auth0).  
Owners authenticate using the provider’s hosted login page; **no passwords are stored locally** and **no identity provider tokens are persisted**.

---

# Flow Summary

1. Client calls **GET `/api/auth/login`**  
2. API constructs an Auth0 authorization URL  
3. API returns **302 Redirect** to the external identity provider  
4. User authenticates externally  
5. Provider redirects back to **GET `/api/auth/callback`** with an authorization code  
6. API exchanges the code for tokens (not persisted)  
7. API loads or creates the Owner record  
8. API issues a secure session cookie  
9. API redirects the user to the frontend dashboard  

---

# Principles

- Authentication is **external**  
- Login initiation endpoint is **pure**  
- Callback endpoint performs **token exchange + session issuance**  
- No identity provider tokens are stored  
- Session cookie is the only authentication state  

---

# See Also

- **[Login Endpoint](ca://s?q=Show_login_endpoint_doc)**  
- **[Callback Endpoint](ca://s?q=Show_callback_endpoint_doc)**  
- **[Authentication Configuration](ca://s?q=Show_authentication_configuration_doc)**  
