namespace Frank.Core.Application.Abstractions.Hosting;

/// <summary>
/// Identifies a class as a hosting module and specifies its execution order
/// within the application's hosting pipeline.
///
/// <para>
/// Hosting modules encapsulate startup‑time behaviors such as configuration
/// loading, service initialization, environment preparation, or infrastructure
/// bootstrapping. The <see cref="HostingModuleAttribute"/> allows the hosting
/// engine to discover these modules and execute them in a deterministic order.
/// </para>
///
/// <para>
/// Lower <see cref="Order"/> values typically indicate earlier execution,
/// enabling foundational modules to run before dependent ones. The exact
/// ordering semantics depend on the hosting coordinator that consumes this
/// attribute.
/// </para>
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class HostingModuleAttribute : Attribute
{
    /// <summary>
    /// Gets the execution order for the hosting module.
    ///
    /// <para>
    /// Modules with lower values generally run earlier in the hosting pipeline,
    /// allowing critical initialization steps to occur before higher‑order
    /// modules.
    /// </para>
    /// </summary>
    public int Order { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="HostingModuleAttribute"/>
    /// class with the specified execution order.
    /// </summary>
    /// <param name="order">
    /// The execution order for the hosting module.
    /// </param>
    public HostingModuleAttribute(int order)
    {
        Order = order;
    }
}
