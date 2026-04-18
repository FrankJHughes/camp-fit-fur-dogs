# API — DDD Layer Wiring

## Intent
Wire the DDD layers (Domain, Application, Infrastructure, API) via dependency injection so that the API project composes the full stack without circular references.

## Value
Proves the layered architecture compiles and runs, and gives developers a clear pattern for registering new services.

## Acceptance Criteria
- [x] Each layer has a DependencyInjection extension method (e.g., AddDomain, AddInfrastructure)
- [x] API Program.cs calls each layer registration in dependency order
- [x] No project references flow upward (Domain depends on nothing, API depends on all)
- [x] Integration test confirms DI container resolves all registered services

## Emotional Guarantees: N/A
