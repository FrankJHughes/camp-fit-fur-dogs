namespace Frank.Identity.Application;

/// <summary>
/// Serves as an assembly marker for the <c>Frank.Identity.Application</c>
/// namespace.
/// <para>
/// This type is used exclusively for assembly scanning, discovery, and
/// registration of CQRS handlers, validators, and other application‑layer
/// components. It contains no behavior and is never instantiated.
/// </para>
/// <para>
/// By providing a stable anchor point, this marker allows DI registration
/// methods to reference the correct assembly without relying on reflection
/// heuristics or string‑based namespace matching.
/// </para>
/// </summary>
public class AssemblyMarker { }
