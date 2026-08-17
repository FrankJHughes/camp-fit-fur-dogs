# Vertical Slice Principles

The solution favors vertical slices over a horizontally layered model.

## Why this matters

- features stay cohesive
- business behavior is easier to trace
- change impact is easier to understand
- persistence and API concerns stay near the feature they serve

## Example

The dog feature is implemented across the following conceptual areas:

- `src/CampFitFurDogs/Api`
- `src/CampFitFurDogs/Application`
- `src/CampFitFurDogs/Domain`
- `src/CampFitFurDogs/Infrastructure`

This allows the product code to read as a set of business capabilities rather than as a collection of unrelated layers.
