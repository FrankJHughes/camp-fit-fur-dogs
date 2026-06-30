# ADR‑0061 — Registration Engine Architecture

## Status  
Accepted  
Supersedes: ADR‑0015 (AutoRegistration)

## Context  
CampFitFurDogs previously relied on **AutoRegistration**, a scanning‑based DI mechanism that used attributes (`[AutoRegister]`) and global assembly scanning to automatically register services. This approach introduced several architectural problems:

- Non‑deterministic registration order  
- Hidden registration behavior  
- Difficulty enforcing purity rules  
- Over‑registration and accidental registration of unintended types  
- Tight coupling between Frank and product assemblies  
- Lack of capability‑specific control  
- Inability to validate registration constraints cleanly  
- Complex guardrails required to detect registration drift  

As the architecture matured — especially with the introduction of:

- **StartupEngine**  
- **HostingEngine**  
- **Endpoint Engine**  
- **Dispatcher pipelines**  
- **ImmutableContextBuilder**  
- **New identity model**  
- **Purity rules v3**  
- **Guardrail separation (DI‑dependent vs pure‑reflection)**  

— it became clear that DI registration needed to be:

- deterministic  
- capability‑driven  
- metadata‑guided  
- reflection‑based  
- governed  
- explicit  
- testable  
- free of global scanning  

AutoRegistration could not meet these requirements.

A new architecture was required.

---

## Decision  
Introduce the **Registration Engine**, a deterministic, capability‑driven DI registration system that replaces AutoRegistration entirely.

### Key elements of the decision:

### 1. `[Registration]` becomes metadata only  
````csharp
[Registration(ServiceLifetime.Scoped, MinRegistrationCount = 1)]
public interface IFooService { }
````

The attribute no longer triggers scanning.  
It simply provides **metadata** used by capabilities.

### 2. Capabilities define discovery rules  
Capabilities use `DiscoveryOptions` to specify:

- which interfaces to include  
- which implementations to include  
- how to validate them  
- how to register them  

This makes DI registration **modular**, **explicit**, and **governed**.

### 3. Registration Engine orchestrates the pipeline  
The engine performs:

1. **Scanning** (based on capability rules)  
2. **Planning** (interface → implementation mapping)  
3. **Validation** (min/max constraints)  
4. **Registration** (DI container wiring)  

### 4. No global scanning  
Frank no longer scans assemblies automatically.  
All scanning is **capability‑driven**.

### 5. No AutoRegister attribute  
`[AutoRegister]` is removed.  
All registration is governed by `[Registration]` + capability rules.

### 6. No Scrutor  
Scrutor is removed from DI governance.  
Registration Engine replaces all suffix‑based scanning.

### 7. No implicit registration  
Every governed interface must be explicitly included by a capability.

### 8. Purity rules updated  
DI purity rules now enforce:

- Application → Domain only  
- Infrastructure → Application abstractions only  
- No DI attributes in Domain  
- No DI logic in slices  
- No manual registration of governed interfaces  
- No service locator  
- No scanning outside Frank  

### 9. Guardrails updated  
Guardrails now validate:

- governed interfaces are registered  
- no duplicate registrations  
- no manual handler registration  
- no manual infrastructure registration  
- readers registered correctly  
- dispatchers registered correctly  
- domain event handlers registered correctly  

Pure‑reflection guardrails validate:

- layering  
- DTO purity  
- handler purity  
- endpoint purity  
- Frank dependency purity  

---

## Consequences  

### Positive

- **Deterministic DI registration**  
- **Capability‑driven architecture**  
- **Stronger purity enforcement**  
- **Cleaner separation between Frank and product assemblies**  
- **Predictable registration behavior**  
- **Simplified guardrails**  
- **Better testability**  
- **No accidental registration drift**  
- **No global scanning overhead**  
- **Clear contributor rules**  
- **Better alignment with StartupEngine + HostingEngine**  

### Negative / Tradeoffs

- Capabilities must define discovery rules explicitly  
- Contributors must understand capability boundaries  
- Registration Engine requires more initial setup than AutoRegistration  
- Migration requires updating guardrails and documentation  

### Supersedes

This ADR **supersedes ADR‑0015**, which defined the AutoRegistration architecture.

AutoRegistration is now deprecated and removed.

---

## Migration  

### Required changes:

- Replace `[AutoRegister]` with `[Registration]`  
- Remove Scrutor usage  
- Update DI guardrails  
- Update Startup modules  
- Update documentation  
- Update Architecture.Tests  
- Update Api.Tests guardrails  
- Update DI purity rules  
- Update slice templates  

### Completed changes:

- Registration Engine implemented  
- All capabilities migrated  
- Guardrails updated  
- StartupEngine aligned  
- HostingEngine aligned  
- Identity model aligned  
- Purity rules updated  
- Documentation updated  

---

## Summary  
The Registration Engine replaces AutoRegistration with a deterministic, capability‑driven DI architecture that:

- enforces purity  
- improves testability  
- eliminates global scanning  
- aligns with Frank’s modular architecture  
- strengthens guardrails  
- simplifies contributor workflow  
- ensures long‑term maintainability  

This ADR formalizes the decision and supersedes ADR‑0015.

