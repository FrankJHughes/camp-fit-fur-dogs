---
id: US-109
title: "Architecture Boundary Roslyn Analyzer"
epic: ""
milestone: ""
status: backlog
domain: infra
urgency: ""
importance: ""
covey_quadrant: ""
vertical_slice: false
emotional_guarantees: ""
legal_guarantees: ""
---
# US-109 — Architecture Boundary Roslyn Analyzer

## Intent

As a **developer**, I want compile-time enforcement of clean architecture boundaries via a custom Roslyn analyzer — so that layer violations appear as red squiggles in the IDE and build errors in CI, without writing any test code.

## Value a custom Roslyn analyzer — so that layer violations appear as red squiggles in the IDE and build errors in CI, without writing any test code.

## Value

- **Instant feedback** — violations surface in the editor as the developer types, not minutes later in a test run.
- **Zero per-product test code** — no `CleanArchitectureGuardrails

- **Instant feedback** — violations surface in the editor as the developer types, not minutes later in a test run.
- **Zero per-product test code** — no `CleanArchitectureGuardrails` subclass, no assembly markers, no test project dependency. The analyzer reads layer metadata from project properties.
- **Build-time enforcement** — `dotnet build` fails on` subclass, no assembly markers, no test project dependency. The analyzer reads layer metadata from project properties.
- **Build-time enforcement** — `dotnet build` fails on violations. A developer cannot accidentally merge a boundary violation even if they skip tests.
- **Portfolio signal** — demonstrates Roslyn analyzer authoring, diagnostic violations. A developer cannot accidentally merge a boundary violation even if they skip tests.
- **Portfolio signal** — demonstrates Roslyn analyzer authoring, diagnostic reporting, code fix providers, and NuGet analyzer packaging.
- **Defense in depth** — complements the test-time reporting, code fix providers, and NuGet analyzer packaging.
- **Defense in depth** — complements the test-time guardrails (US-108 SharedKernel.Testing) and runtime validation (US-108 `AddSharedKernel()`). Three layers of enforcement: compile, test, runtime.

## Problem

After US-108, boundary enforcement exists guardrails (US-108 SharedKernel.Testing) and runtime validation (US-108 `AddSharedKernel()`). Three layers of enforcement: compile, test, runtime at two points:

| Enforcement point | When violations surface | Feedback latency |
|---|---|---|
| SharedKernel.Testing (.

## Problem

After US-108, boundary enforcement exists at two points:

| Enforcement point | When violations surface | Feedback latency |
|---|---|---|
| SharedKernel.Testing (test time) | `dotnet test` or CI | Seconds to minutes |
| `AddSharedKernel()` (runtime) | Application startup | Seconds (dev) to minutes (CItest time) | `dotnet test` or CI | Seconds to minutes |
| `AddSharedKernel()` (runtime) | Application startup | Seconds (dev) to minutes (CI) |

Neither catches violations **while the developer is writing code**. A developer can type `using CampFitFurDogs.Infrastructure;` inside) |

Neither catches violations **while the developer is writing code**. A developer can type `using CampFitFurDogs.Infrastructure;` inside a Domain class, save the file, and see no error until they run tests a Domain class, save the file, and see no error until they run tests or start the app. The IDE offers no protection.

Roslyn analyzers run inside the compiler pipeline. They see every syntax tree and semantic model in or start the app. The IDE offers no protection.

Roslyn analyzers run inside the compiler pipeline. They see every syntax tree and semantic model in real time. A custom analyzer can flag forbidden `using` statements, constructor injections, and type references **as the code is written** — same experience as a compiler error.

## Solution

### Part A: Analyzer project

```
src/SharedKernel.Analyzers/ real time. A custom analyzer can flag forbidden `using` statements, constructor injections, and type references **as the code is written** — same experience as a compiler error.

## Solution

### Part A: Analyzer project

```
src/SharedKernel.Analyzers/
  LayerDependencyAnalyzer.cs           (DiagnosticAnalyzer)
  LayerConfiguration.cs                (reads layer metadata from .csproj)
  DiagnosticDescriptors.cs             (all
  LayerDependencyAnalyzer.cs           (DiagnosticAnalyzer)
  LayerConfiguration.cs                (reads layer metadata from .csproj)
  DiagnosticDescriptors.cs             (all diagnostic IDs and messages)
```

The analyzer ships as part of the SharedKernel solution. Products reference it as an analyzer project reference diagnostic IDs and messages)
```

The analyzer ships as part of the SharedKernel solution. Products reference it as an analyzer project reference or (later) as a NuGet analyzer package.

### Part B: Layer declaration via MSBuild properties

Products declare their layer membership in each `.csproj` — no JSON config file, no attributes, no magic or (later) as a NuGet analyzer package.

### Part B: Layer declaration via MSBuild properties

Products declare their layer membership in each `.csproj` — no JSON config file, no attributes, no magic:

```xml
<!-- CampFitFurDogs.Domain.csproj -->
<PropertyGroup>
  <ArchitectureLayer>Domain</ArchitectureLayer>
</PropertyGroup>
```

```xml
<!-- CampFitFurDogs.Application.csproj -->
<PropertyGroup>
  <ArchitectureLayer>Application</ArchitectureLayer>
</PropertyGroup>
```

```xml
<!-- CampFitFurDogs.Infrastructure.csproj -->
<PropertyGroup>
  <ArchitectureLayer>:

```xml
<!-- CampFitFurDogs.Domain.csproj -->
<PropertyGroup>
  <ArchitectureLayer>Domain</ArchitectureLayer>
</PropertyGroup>
```

```xml
<!-- CampFitFurDogs.Application.csproj -->
<PropertyGroup>Infrastructure</ArchitectureLayer>
</PropertyGroup>
```

```xml
<!-- CampFitFurDogs.Api.csproj -->
<PropertyGroup>
  <ArchitectureLayer>Api</ArchitectureLayer>
</PropertyGroup>
```

The analyzer reads `ArchitectureLayer` from `AnalyzerConfigOptions
  <ArchitectureLayer>Application</ArchitectureLayer>
</PropertyGroup>
```

```xml
<!-- CampFitFurDogs.Infrastructure.csproj -->
<PropertyGroup>
  <ArchitectureLayer>Infrastructure</ArchitectureLayer>
</PropertyGroup>
```

```xml
<!-- CampFitFurDogs.Api.csproj -->
<PropertyGroup>
  <ArchitectureLayer>Api</ArchitectureLayer>
</PropertyGroup>
```

The analyzer reads `ArchitectureLayer` from `AnalyzerConfigOptions` and applies the rule set for that layer. SharedKernel projects do not set this property — they are foundation, not product layers.

### Part C: Diagnostic` and applies the rule set for that layer. SharedKernel projects do not set this property — they are foundation, not product layers.

### Part C: Diagnostic rules

Each rule maps to a guardrail from Architecture.Tests:

| Diagnostic ID | Rule | Severity | Applies to layer |
|---|---|---|---|
| ` rules

Each rule maps to a guardrail from Architecture.Tests:

| Diagnostic ID | Rule | Severity | Applies to layer |
|---|---|---|---|
| `CFFD001` | Domain must not reference Application | Error | Domain |
| `CFFD002` | Domain must not reference Infrastructure | Error | Domain |
| `CFFD003` | Domain must not reference Api | Error | Domain |
| `CFFD004` | Domain must not use EF Core types | Error | Domain |
| `CFFD005` | Domain must not use ASP.NET Core types | Error | Domain |
| `CFFD006` | Domain must not use System.Text.Json attributesCFFD001` | Domain must not reference Application | Error | Domain |
| `CFFD002` | Domain must not reference Infrastructure | Error | Domain |
| `CFFD003` | Domain must not reference Api | Error | Domain |
| `CFFD004` | Domain must not use EF Core types | Error | Domain |
| `CFFD005` | Domain must not use ASP.NET Core types | Error | Domain |
| `CFFD006` | Domain must not use System.Text.Json attributes | Error | Domain |
| `CFFD007` | Domain must not use DataAnnotations | Error | Domain |
| `CFFD008` | Application must not reference Infrastructure | Error | Application |
| `CFFD009` | Application must not reference Api | Error | Application |
| `CFFD010` | Application DTOs must not reference EF Core types | Error | Application |
| `CFFD011` | Query | Error | Domain |
| `CFFD007` | Domain must not use DataAnnotations | Error | Domain |
| `CFFD008` | Application must not reference Infrastructure | Error | Application |
| `CFFD009` | Application must not reference Api | Error | Application |
| `CFFD010` | Application DTOs must not reference EF Core types | Error | Application |
| `CFFD011` | Query handlers must not depend on repository interfaces | Warning | Application |
| `CFFD012` | Api must not reference Infrastructure directly | Error | Api |
| `CFFD013` | Api endpoint return types must not leak handlers must not depend on repository interfaces | Warning | Application |
| `CFFD012` | Api must not reference Infrastructure directly | Error | Api |
| `CFFD013` | Api endpoint return types must not leak Domain or EF types | Warning | Api |
| `CFFD014` | Infrastructure must not reference Api | Error | Infrastructure |

### Part D: Analyzer implementation

The analyzer is a standard `DiagnosticAnalyzer` that registers symbol actions:

```csharp
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class LayerDependencyAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        DiagnosticDescriptors.All;

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(
            GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStart Domain or EF types | Warning | Api |
| `CFFD014` | Infrastructure must not reference Api | Error | Infrastructure |

### Part D: Analyzer implementation

The analyzer is a standard `DiagnosticAnalyzer` that registers symbol actions:

```csharp
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class LayerDependencyAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        DiagnosticDescriptors.All;

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(
            GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(compilationContext =>
        {
            var layer = GetLayerFromOptions(compilationContext.Options);
            if (layer is null) return; // not a layered project

            var rules = RulesFor(layer);Action(compilationContext =>
        {
            var layer = GetLayerFromOptions(compilationContext.Options);
            if (layer is null) return; // not a layered project

            var rules = RulesFor(layer);

            compilationContext.RegisterSymbolAction(
                ctx => AnalyzeSymbol(ctx, rules),
                SymbolKind.NamedType);

            compilationContext.RegisterSyntaxNodeAction(
                ctx => AnalyzeUsing(ctx, rules),
                SyntaxKind.UsingDirective);
        });
    }
}
```

Key design

            compilationContext.RegisterSymbolAction(
                ctx => AnalyzeSymbol(ctx, rules),
                SymbolKind.NamedType);

            compilationContext.RegisterSyntaxNodeAction(
                ctx => AnalyzeUsing(ctx, rules),
                SyntaxKind.UsingDirective);
        });
    }
}
```

Key design decisions:

- **`RegisterCompilationStartAction`** — reads layer config once per compilation, not per file.
- **`EnableConcurrentExecution`** — analyzer runs on decisions:

- **`RegisterCompilationStartAction`** — reads layer config once per compilation, not per file.
- **`EnableConcurrentExecution`** — analyzer runs on multiple files in parallel for performance.
- **`GeneratedCodeAnalysisFlags.None`** — skips EF migrations and other generated code.
- **Namespace-based detection** — the analyzer checks referenced multiple files in parallel for performance.
- **`GeneratedCodeAnalysisFlags.None`** — skips EF migrations and other generated code.
- **Namespace-based detection** — the analyzer checks referenced type namespaces against forbidden patterns for the current layer. No assembly-level scanning needed.

### Part E: Code fix provider (optional stretch goal)

```csharp
[ExportCodeFixProvider(LanguageNames.CSharp)]
public class Rem type namespaces against forbidden patterns for the current layer. No assembly-level scanning needed.

### Part E: Code fix provider (optional stretch goal)

```csharp
[ExportCodeFixProvider(LanguageNames.CSharp)]
public class RemoveForbiddenUsingCodeFix : CodeFixProvider
{
    // Offers "Remove forbidden using directive" quick fix
}
```

When the analyzer flagsoveForbiddenUsingCodeFix : CodeFixProvider
{
    // Offers "Remove forbidden using directive" quick fix
}
```

When the analyzer flags a forbidden `using`, the code fix provider offers a one-click removal. This is a stretch goal — the diagnostic alone provides the a forbidden `using`, the code fix provider offers a one-click removal. This is a stretch goal — the diagnostic alone provides the core value.

### Part F: Analyzer tests

Roslyn analyzers have their own test framework (`Microsoft.CodeAnalysis.CSharp.Analyzer.Testing`). Each diagnostic core value.

### Part F: Analyzer tests

Roslyn analyzers have their own test framework (`Microsoft.CodeAnalysis.CSharp.Analyzer.Testing`). Each diagnostic gets a test class verifying it fires on violation code and stays silent on clean code:

```
tests/SharedKernel. gets a test class verifying it fires on violation code and stays silent on clean code:

```
tests/SharedKernel.Analyzers.Tests/
  CFFD001_DomainMustNotReferenceApplicationTests.cs
  CFFD002_DomainMustNotReferenceInfrastructureTests.cs
  CFFD008_ApplicationMustNotReferenceInfrastructureTests.cs
  ...
```

Each test uses `AnalyzerTest<TAnalyzer>` with inline source strings — no project references needed. Tests verify:

- Violation code produces the expected diagnostic at the expected location.
- Clean code produces no diagnostics.
- The diagnostic message includes the offending type name, the current layer, and the forbidden layer.

### Part G: ProductAnalyzers.Tests/
  CFFD001_DomainMustNotReferenceApplicationTests.cs
  CFFD002_DomainMustNotReferenceInfrastructureTests.cs
  CFFD008_ApplicationMustNotReferenceInfrastructureTests.cs
  ...
```

Each test uses `AnalyzerTest<TAnalyzer>` with inline source strings — no project references needed. Tests verify:

- Violation code produces the expected diagnostic at the expected location.
- Clean code produces no diagnostics.
- The diagnostic message includes the offending type name, the current layer, and the forbidden layer.

### Part G: Product integration

Product projects add an analyzer reference:

```xml
<!-- In Directory.Build.props or each .csproj -->
<ItemGroup>
  <ProjectReference
    Include="..\SharedKernel.Analyzers\CampFitFurDogs. integration

Product projects add an analyzer reference:

```xml
<!-- In Directory.Build.props or each .csproj -->
<ItemGroup>
  <ProjectReference
    Include="..\CampFitFurSharedKernel.Analyzers.csproj"
    OutputItemType="Analyzer"
    ReferenceOutputAssembly="false" />
</ItemGroup>
```

That is it. No test code, no configuration beyond the `ArchitectureLayer` property. The analyzer runs inside `dotnet build` and inside the IDE.

## DeliverablesDogs.SharedKernel.Analyzers\SharedKernel.Analyzers.csproj"
    OutputItemType="Analyzer"
    ReferenceOutputAssembly="false" />
</ItemGroup>
```

That is it. No test code, no configuration

- [ ] ADR-0023: Architecture Boundary Roslyn Analyzer
- [ ] `SharedKernel.Analyzers` project targeting `netstandard2.0` (Roslyn analyzer requirement)
- [ ] `LayerDependencyAnalyzer` diagnostic analyzer beyond the `ArchitectureLayer` property. The analyzer runs inside `dotnet build` and inside the IDE.

## Deliverables

- [ ] ADR-0023: Architecture Boundary Roslyn Analyzer
- [ ] `SharedKernel.Analyzers` project targeting `netstandard2.0` (Roslyn analyzer requirement)
- [ ] `LayerDependencyAnalyzer` diagnostic analyzer with all 14 diagnostic rules
- [ ] `LayerConfiguration` with all 14 diagnostic rules
- [ ] `LayerConfiguration` reads `ArchitectureLayer` from MSBuild properties via `AnalyzerConfigOptions`
- [ ] `DiagnosticDescriptors` with clear, actionable messages for each violation
- [ ] All product `.csproj` files updated with `<ArchitectureLayer>` property
- [ ] `Directory.Build.props` updated with analyzer project reference
- [ ] `C reads `ArchitectureLayer` from MSBuild properties via `AnalyzerConfigOptions`
- [ ] `DiagnosticDescriptors` with clear, actionable messages for each violation
- [ ] All product `.csproj` files updated with `<ArchitectureLayer>` property
- [ ] `Directory.Build.props` updated with analyzer project reference
- [ ] `SharedKernel.Analyzers.Tests` project created
- [ ] Analyzer verification tests for all 14 diagnostic rules (violation + clean code)
- [ ] Project added to `CampFitFurDogs.sln`
- [ ] `copampFitFurDogs.SharedKernel.Analyzers.Tests` project created
- [ ] Analyzer verification tests for all 14 diagnostic rules (violation + clean code)
- [ ] Project added to `CampFitFurDogs.sln`
- [ ] `copilot-instructions.md` updated with analyzer conventions
- [ ] CHANGELOG updated
- [ ] CI passes (including analyzer tests)

### Stretch deliverables

- [ ] `RemoveForbiddenUsingCodeFix` code fix provider
- [ ] Code fix testsilot-instructions.md` updated with analyzer conventions
- [ ] CHANGELOG updated
- [ ] CI passes (including analyzer tests)

### Stretch deliverables

- [ ] `RemoveForbiddenUsingCodeFix` code fix provider
- [ ] Code fix tests
- [ ] NuGet packaging configuration for future distribution

## Acceptance Criteria

- [ ] A `using CampFitFurDogs.Infrastructure;` statement in a Domain file
- [ ] NuGet packaging configuration for future distribution

## Acceptance Criteria

- [ ] A `using CampFitFurDogs.Infrastructure;` statement in a Domain file produces diagnostic `CFFD002` as a build error
- [ ] The same violation shows as a red squiggle in VS Code and Visual Studio before building
- [ ] A `using CampFitFurDogs.Api;` statement in an Application file produces diagnostic `CFFD009` as a build error
- [ produces diagnostic `CFFD002` as a build error
- [ ] The same violation shows as a red squiggle in VS Code and Visual Studio before building
- [ ] A `using CampFitFurDogs.Api;` statement in an Application file produces diagnostic `CFFD009` as a build error
- [ ] A query handler constructor accepting `ICustomerRepository` produces diagnostic `CFFD011` as a warning
- [ ] Clean code (no violations) produces zero diagnostics
- [ ] EF Core migrations ] A query handler constructor accepting `ICustomerRepository` produces diagnostic `CFFD011` as a warning
- [ ] Clean code (no violations) produces zero diagnostics
- [ ] EF Core migrations and generated code are excluded from analysis
- [ ] SharedKernel projects (no `ArchitectureLayer` property) are excluded from analysis
- [ ] All 14 diagnostic rules have passing verification tests (both positive and generated code are excluded from analysis
- [ ] SharedKernel projects (no `ArchitectureLayer` property) are excluded from analysis
- [ ] All 14 diagnostic rules have passing verification tests (both positive and negative cases)
- [ ] `dotnet build` fails when a layer violation exists — CI catches violations without running tests
- [ ] Diagnostic messages include the offending type name, the current layer, and the forbidden layer
- [ ] Analyzer runs concurrently and does not noticeably slow IDE and negative cases)
- [ ] `dotnet build` fails when a layer violation exists — CI catches violations without running tests
- [ ] Diagnostic messages include the offending type name, responsiveness or build time
- [ ] ADR-0023 accepted and indexed
- [ ] CI passes

## Emotional Guarantees

- A developer sees boundary violations the moment they type the wrong `using` — the current layer, and the forbidden layer
- [ ] Analyzer runs concurrently and does not noticeably slow IDE responsiveness or build time
- [ ] ADR-0023 accepted and indexed
- [ ] CI passes

## Emotional Guarantees

- A developer sees boundary violations the moment they type the wrong `using` — no waiting for tests, no waiting for app startup.
- A developer never merges a layer violation because the build itself refuses to pass.
- A developer adding the analyzer to a no waiting for tests, no waiting for app startup.
- A developer never merges a layer violation because the build itself refuses to pass.
- A developer adding the analyzer to a new product sets one MSBuild property per project and gets full enforcement — no test classes, no configuration new product sets one MSBuild property per project and gets full enforcement — no test classes, no configuration files, no learning curve.
- A developer debugging a false positive can suppress a specific diagnostic with a standard `#pragma warning disable files, no learning curve.
- A developer debugging a false positive can suppress a specific diagnostic with a standard `#pragma warning disable CFFD001` — the mechanism is familiar and discoverable.

## Dependencies

- US-108 (Foundation Extraction) — required (establishes the SharedKernel project structure and boundary definitions that the analyzer enforces)

## Estimated Effort

~12 hours (Roslyn analyzer auth CFFD001` — the mechanism is familiar and discoverable.

## Dependencies

- US-108 (Foundation Extraction) — required (establishes the SharedKernel project structure and boundary definitions that the analyzer enforces)

## Estimated Effort

~12 hours (Roslyn analyzer authoring + 14 diagnostic rules + analyzer test framework setup + verification tests + MSBuild integration + docs)

## Notes

- The analyzer targets `netstandard2.0`oring + 14 diagnostic rules + analyzer test framework setup + verification tests + MSBuild integration + docs)

## Notes

- The analyzer targets `netstandard2.0` — this is a Roslyn requirement, not a choice. Analyzers must be loadable by both VS Code ( — this is a Roslyn requirement, not a choice. Analyzers must be loadable by both VS Code (OmniSharp/.NET 6+) and Visual Studio (Roslyn 4.x).
- The analyzer complements but does not replace SharedKernel.Testing guardrails. Test-time guardrails can enforce rules that areOmniSharp/.NET 6+) and Visual Studio (Roslyn 4.x).
- The analyzer complements but does not replace SharedKernel.Testing guardrails. Test-time guardrails can enforce rules that are impractical for a syntax/semantic analyzer (e.g., "every IEndpoint implementation must be discoverable by assembly scanning"). The two enforcement impractical for a syntax/semantic analyzer (e.g., "every IEndpoint implementation must be discoverable by assembly scanning"). The two enforcement layers cover different rule categories.
- If the analyzer is later published as a NuGet package, the `OutputItemType="Analyzer"` reference becomes a standard `PackageReference` with analyzer assets layers cover different rule categories.
- If the analyzer is later published as a NuGet package, the `OutputItemType="Analyzer"` reference becomes a standard `PackageReference` with analyzer assets — zero product code change.

