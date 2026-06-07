# ADR‑0045 — Session Store Scaling Strategy

## Status  
Accepted

## Context  
The introduction of server‑side session management (ADR‑0039) created new operational considerations:

- Sessions must remain valid across multiple API instances.  
- Session validation must be fast and deterministic.  
- Session storage must support cleanup and expiration.  
- PR Preview environments must not share session state with production.  
- Future horizontal scaling requires distributed session storage.

A strategy was needed to ensure the session store can scale with the system.

## Decision  
The system adopts a **scalable session store architecture** with the following characteristics:

1. **Database‑backed session storage**  
   - Sessions are stored in the primary relational database.  
   - Ensures consistency across API instances.  
   - Leverages existing migrations and infrastructure.

2. **Opaque session identifiers**  
   - No user data stored in the cookie.  
   - All identity resolution occurs server‑side.

3. **Expiration‑based cleanup**  
   - Sessions include `ExpiresAt`.  
   - Cleanup occurs via periodic background tasks or database TTL strategies.

4. **Environment isolation**  
   - Local, preview, and production use separate databases.  
   - Prevents cross‑environment session leakage.

5. **Future‑ready scaling**  
   - Architecture supports migration to Redis or distributed cache if needed.  
   - No product code changes required—only Infrastructure changes.

## Consequences  

### Positive  
- Sessions remain valid across horizontally scaled API instances.  
- Predictable, centralized session validation.  
- Clean separation between session cookie and session data.  
- Easy to migrate to distributed storage in the future.

### Negative  
- Database load increases slightly due to session reads/writes.  
- Requires periodic cleanup to avoid unbounded growth.  
