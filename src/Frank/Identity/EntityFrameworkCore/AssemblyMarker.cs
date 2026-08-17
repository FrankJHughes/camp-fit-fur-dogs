namespace Frank.Identity.EntityFrameworkCore;

/// <summary>
/// Serves as a stable, discoverable anchor type for assembly scanning within the
/// Identity EntityFrameworkCore subsystem.
/// <para>
/// This marker class allows infrastructure components—such as EF Core configuration
/// loaders, DI scanners, and migration generators—to reference this assembly via
/// <c>typeof(AssemblyMarker).Assembly</c> without relying on fragile string‑based
/// assembly names.
/// </para>
/// <para>
/// The class is intentionally empty; its presence is the feature.
/// </para>
/// </summary>
public class AssemblyMarker { }
