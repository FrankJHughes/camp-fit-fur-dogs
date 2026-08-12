namespace CampFitFurDogs.Infrastructure;

/// <summary>
/// Marker type used to reference the <see cref="CampFitFurDogs.Infrastructure"/>
/// assembly via <c>typeof()</c>.
/// <para>
/// This class has no behavior or state. Its sole purpose is to provide a stable
/// anchor point for assembly scanning operations, such as:
/// <list type="bullet">
/// <item><description>Locating EF Core configurations</description></item>
/// <item><description>Registering infrastructure services</description></item>
/// <item><description>Applying conventions based on assembly boundaries</description></item>
/// </list>
/// </para>
/// <para>
/// Marker types are a common pattern for avoiding hard‑coded assembly names and
/// ensuring compile‑time safety when referencing assemblies.
/// </para>
/// </summary>
public sealed class AssemblyMarker;
