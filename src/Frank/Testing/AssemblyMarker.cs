namespace Frank.Testing;

/// <summary>
/// An empty marker type used to reference the <c>Frank.Testing</c> assembly
/// in a safe and strongly typed manner.
/// <para>
/// Marker types provide a stable anchor for reflection‑based operations such as:
/// </para>
/// <list type="bullet">
/// <item><description>Endpoint assembly discovery</description></item>
/// <item><description>Module or feature scanning</description></item>
/// <item><description>Test harness configuration</description></item>
/// <item><description>Locating resources without relying on string‑based assembly names</description></item>
/// </list>
/// <para>
/// This class intentionally contains no logic and is never instantiated.
/// Its sole purpose is to serve as a reliable assembly identifier within the
/// Frank testing infrastructure.
/// </para>
/// </summary>
public sealed class AssemblyMarker { }
