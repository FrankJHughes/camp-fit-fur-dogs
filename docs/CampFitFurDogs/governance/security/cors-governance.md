# CORS Governance

Defines allowed origins and rules for all environments.

---

# Local

```
http://localhost:3000
```

---

# Preview

```
https://campfitfurdogsapi-pr-<number>.onrender.com
```

---

# Production

```
https://campfitfurdogs.com
```

---

# Rules

- Only these origins allowed  
- No wildcard origins  
- No dynamic origins  
- Auth0 origins configured separately  
- API must reject all other origins
