# ADR‑0048 — Reader/Repository Separation Model

## Status  
Accepted

## Context  
As the system grew, query logic and mutation logic began to intermingle:

- Some queries were implemented using repositories.  
- Some repositories returned DTOs instead of aggregates.  
- Some slices performed read‑only operations through mutation‑capable services.  
- Query performance tuning was difficult due to aggregate hydration.  

A clear separation was required to maintain architectural purity and performance.

## Decision  
The system now uses a **Reader/Repository Separation Model**.

### Characteristics

1. **Repositories (Domain)**  
   - Operate on aggregates  
   - Support mutation  
   - Enforce invariants  
   - Participate in Unit of Work  
   - Never return DTOs  
   - Never perform cross‑aggregate joins

2. **Readers (Application)**  
   - Return DTOs  
   - Perform read‑optimized queries  
   - May join across tables  
   - Never mutate state  
   - Never return aggregates  
   - Are auto‑registered via `[AutoRegister]`

3. **Query/Command alignment**  
   - Commands use repositories  
   - Queries use readers  
   - No cross‑usage allowed

4. **Performance benefits**  
   - Readers can use projection queries  
   - Repositories hydrate only necessary aggregates  
   - Reduces unnecessary database load

5. **Purity alignment**  
   - Domain layer remains mutation‑focused  
   - Application layer handles read‑model concerns

## Consequences  

### Positive  
- Clear separation of read and write concerns.  
- Improved performance for read‑heavy operations.  
- Reduced accidental aggregate hydration.  
- Cleaner slice boundaries.  
- Easier to reason about invariants and mutations.

### Negative  
- Requires developers to understand when to use readers vs repositories.  
- Some slices require both, increasing file count.
