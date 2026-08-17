# Conventions

The codebase follows a layered, feature-centered architecture with explicit boundaries between product and platform concerns.

## Layering conventions

- `Domain` contains business rules, aggregates, and value objects.
- `Application` contains use cases, commands, queries, and orchestration.
- `Infrastructure` contains EF Core and external integration code behind abstractions.
- `Api` contains endpoint composition, startup, middleware, and hosting configuration.

## Vertical-slice approach

Features are organized as slices rather than a purely horizontal service layout. For example, a domain capability often spans API, application, domain, and infrastructure folders together.

## Naming conventions

- Feature names should match the business capability they represent.
- Commands and handlers should describe the use case explicitly.
- Interfaces should be kept separate from implementations whenever a real abstraction exists.

## Documentation conventions

Every subsystem should explain:

- its responsibilities
- the main folders it owns
- its boundary with broader platform services
- the main extension points for future work
