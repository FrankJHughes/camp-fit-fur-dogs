# Environment Seam Conventions (Frank)

Frank provides an environment abstraction that allows products and hosting
providers to operate without reading environment variables directly.

---

## Purpose

The environment seam:

- isolates environment variable access  
- enables deterministic testing  
- prevents environment‑specific logic from leaking into product code  
- supports hosting provider evaluation  

---

## Requirements

Environment access must go through:

````csharp
IEnvironment
````

Implementations must:

- be immutable  
- be side‑effect‑free  
- expose only typed accessors  
- never throw for missing values (providers handle that)  

---

## Testability

Tests must:

- replace `IEnvironment` with a fake  
- never rely on real environment variables  
- assert provider behavior deterministically  

---

## Prohibitions

Frank must not:

- expose raw environment variable APIs  
- allow direct calls to `Environment.GetEnvironmentVariable`  
- allow products to bypass the seam  

Products must not:

- read environment variables directly  
- implement their own environment accessors  
