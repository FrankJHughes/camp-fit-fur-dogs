# Layering Rules

These rules help maintain the repository’s intended architecture.

## Rule 1: keep the domain pure
Domain logic should not depend on ASP.NET Core, EF Core, or transport concerns.

## Rule 2: infrastructure implements abstractions
Persistence and external system code should appear behind interfaces and contracts declared at higher layers.

## Rule 3: keep the API thin
The API layer should compose behavior and route requests rather than contain domain logic.

## Rule 4: feature code stays together
A feature should remain cohesive across feature slices, not be split across unrelated folders.

## Rule 5: reuse the platform deliberately
The `Frank` modules are shared building blocks and should be used instead of reimplementing common infrastructure.
