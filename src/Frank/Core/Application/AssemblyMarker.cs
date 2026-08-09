namespace Frank.Core.Application;

/// <summary>
/// Marker type used to identify the <c>Frank.Core.Application</c> assembly
/// for scanning, discovery, and registration purposes.
///
/// <para>
/// This type contains no behavior. Its sole purpose is to provide a stable,
/// strongly‑typed anchor point for reflection‑based operations such as:
/// </para>
/// <list type="bullet">
///   <item>module discovery</item>
///   <item>endpoint scanning</item>
///   <item>registration orchestration</item>
///   <item>assembly‑scoped configuration</item>
/// </list>
///
/// <para>
/// By convention, all Frank modules include an <c>AssemblyMarker</c> type
/// to support consistent and predictable assembly identification.
/// </para>
/// </summary>
public sealed class AssemblyMarker { }
