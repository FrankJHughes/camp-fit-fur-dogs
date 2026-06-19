# DI and Registration Conventions (Frank)

Frank provides cross‑cutting dependency injection behaviors that products consume.
Frank must remain product‑agnostic and must not reference CampFitFurDogs code.

---

## Auto‑Registration

Frank automatically registers:

- CQRS handlers  
- Validators  
- Endpoints  
- Hosting providers  
- Test seams  

Auto‑registration is attribute‑driven:

````csharp
[AutoRegister]
public sealed class CreateOwnerHandler : ICommandHandler<CreateOwner>
````
  
Products must not manually register these types.

---

## Dispatcher Enforcement

- All commands and queries must go through Frank’s dispatcher.  
- Handlers must not be invoked directly.  
- Validators must be discovered automatically.  

---

## Prohibitions

Frank must not:

- depend on product assemblies  
- register product services  
- reference product domain/application/infrastructure  

Products must not:

- bypass the dispatcher  
- manually register handlers or validators  
