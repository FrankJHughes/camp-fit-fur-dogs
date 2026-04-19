# ADR-0020: Endpoint Auto-Discovery via IEndpoint

| Field     | Value       |
|-----------|-------------|
| Status    | Accepted    |
| Date      | 2026-04-18  |
| Deciders  | Frank Hughes|

## Context

Endpoint registration required manually wiring each endpoint through group files
(`CustomerEndpoints.cs`, `DogEndpoints.cs`) that called extension methods. Adding a
new slice meant touching three files: the endpoint class, the group file, and
`Endpoints.cs`. This violated the open/closed principle — every new slice modified
existing wiring code.

## Decision

Introduce an `IEndpoint` interface using C# static abstract interface members:

```csharp
public interface IEndpoint
{
    static abstract void Map(IEndpointRouteBuilder app);
}
```

`Endpoints.MapEndpoints()` assembly-scans for all `IEndpoint` implementations and
invokes `Map` on each, passing a shared `/api` route group. Endpoint classes own
their own route prefix (e.g., `/customers`, `/dogs/{id}`).

Group files (`CustomerEndpoints.cs`, `DogEndpoints.cs`) are eliminated. Each
endpoint class is a non-static class implementing `IEndpoint`.

Two architecture guardrail tests enforce the convention:

1. At least one `IEndpoint` implementation exists in the Api assembly.
2. Every class ending in `Endpoint` implements `IEndpoint`.

## Consequences

### Positive

- Adding a new endpoint requires only one new file — zero wiring changes.
- Guardrail tests prevent silent convention drift.
- Route prefixes live next to the handler logic, improving discoverability.

### Negative

- Reflection-based discovery adds a small startup cost (acceptable — runs once).
- Static abstract interface members require C# 11+ / .NET 7+.

### Neutral

- `Program.cs` is unchanged — still calls `app.MapEndpoints()`.
- Route paths remain identical: `/api/customers`, `/api/dogs`, `/api/dogs/{id}`.
