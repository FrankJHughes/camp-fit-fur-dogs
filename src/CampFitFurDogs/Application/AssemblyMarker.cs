namespace CampFitFurDogs.Application;

/// <summary>
/// Serves as a marker type for the <c>CampFitFurDogs.Application</c> assembly.
/// <para>
/// This type is intentionally empty and exists solely to provide a stable
/// reference point for assembly‑based discovery operations, such as automatic
/// registration of CQRS handlers, validators, and other application‑layer
/// components.
/// </para>
/// <para>
/// Usage typically involves passing <c>typeof(AssemblyMarker).Assembly</c>
/// to registration methods that scan the application assembly for types
/// implementing specific interfaces.
/// </para>
/// </summary>
public sealed class AssemblyMarker;
