namespace Frank.Core.Infrastructure;

/// <summary>
/// Assembly marker used to identify the Frank.Core.Infrastructure assembly
/// for reflection-based discovery, registration, and scanning operations.
/// <para>
/// This type contains no logic and exists solely as a stable anchor point
/// for assembly resolution.
/// It is used by infrastructure components that need to reference this
/// assembly without relying on file names, conventions, or brittle heuristics.
/// </para>
/// </summary>
public sealed class AssemblyMarker { }
