# Shapes

The **Shapes** folder contains the structural data types used throughout the Frank.Core.Application registration pipeline. These types are intentionally lightweight and behavior‑free. They exist to carry information between the scanning, planning, validation, and registration stages.

Shapes define the *form* of the data flowing through the pipeline, not the *logic* that processes it.

---

## Overview

The registration pipeline moves through four major phases:

1. **Scanning**  
   Assemblies are inspected to discover relevant interfaces and concrete implementations.

2. **Planning**  
   Discovered relationships are transformed into registration plans based on `RegistrationAttribute` rules.

3. **Validation**  
   Plans are checked to ensure implementation counts fall within required ranges.

4. **Registration**  
   Validated plans are applied to the DI container.

The Shapes folder contains the immutable records that represent the data passed between these phases.

---

## Shape Types

### Implementation
Represents a single discovered relationship between:

- a concrete implementing class (`TypeInfo`)
- the interface it implements (`Type`)

Produced by the scanner.

---

### ImplementedInterfaceGroup
Groups all implementing classes for a single implemented interface.

Used by the planner to generate `Plan` objects.

---

### RelevantInterfaceGroup
Represents:

- a relevant interface (one that passed inclusion predicates)
- all discovered implementations for that interface

This is the bridge between scanning and planning.

---

### Plan
A fully constructed registration plan containing:

- the `RegistrationAttribute` applied to the interface  
- the implemented interface  
- the implementing classes discovered  

Plans are validated and then executed by the registrar.

---

### Violation
Represents a registration‑rule failure when:

```
MinRegistrationCount ≤ ActualRegistrationCount ≤ MaxRegistrationCount
```

is not satisfied.

Returned by the validator and surfaced by the orchestrator.

---

## Design Principles

- **Immutability**  
  All shapes are `record` types, ensuring predictable, side‑effect‑free data flow.

- **Separation of Concerns**  
  Shapes contain *structure only*.  
  All logic resides in Scanner, Planner, Validator, Registrar, and Orchestrator.

- **Pipeline Clarity**  
  Each shape corresponds to a distinct stage of the registration pipeline, making the flow easy to reason about and debug.
