# Hosting Modules

Hosting modules allow the CampFitFurDogs API to adapt its behavior based on the deployment environment. They provide a clean, composable mechanism for applying environment-specific configuration without scattering conditional logic throughout the startup pipeline.

## Pattern

Each hosting module implements the platform’s `IHostingModule` interface:

```csharp
public interface IHostingModule
{
    Task ApplyAsync(WebApplicationBuilder builder);
}
```

A hosting module receives the `WebApplicationBuilder` and applies configuration changes, service registrations, or environment-specific adjustments. This keeps startup declarative and aligned with the platform’s hosting model.

## Current Modules

### RenderPrPreviewHostingModule

This module activates when the API is running in Render.com’s PR preview environment. It applies environment-specific behavior such as:

- loading preview-only configuration values  
- adjusting database connection strings  
- enabling or disabling preview-only features  
- configuring authentication settings for preview URLs  

This ensures that PR previews behave consistently and safely without modifying production configuration.

## Registration

Hosting modules are constructed in `Hosting.cs`:

```csharp
public static IHostingModule[] ConstructHostingModules()
{
    return [
        new RenderPrPreviewHostingModule(),
        // Add more modules here
    ];
}
```

During startup, the platform applies all hosting modules:

```csharp
await Hosting.AdaptToHostingEnvironment(builder);
```

This ensures each module has an opportunity to modify the hosting environment before the application is built.

## Adding New Modules

To introduce new environment-specific behavior:

1. **Create a class** implementing `IHostingModule`  
2. **Implement `ApplyAsync()`** to apply configuration changes  
3. **Add the module** to `ConstructHostingModules()`  
4. The module will be **automatically applied** during startup  

Hosting modules should remain small, focused, and environment-specific. They provide a clean extension point for future deployment environments such as staging, QA, or local developer overrides.

