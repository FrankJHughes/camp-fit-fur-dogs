using Microsoft.Extensions.DependencyInjection;

namespace Frank.Core.Application.Registration;

/// <summary>
/// Specifies registration rules for an interface discovered during the
/// assembly‑scanning process.
///
/// <para>
/// This attribute is applied directly to an interface to indicate that it
/// participates in automatic registration. The registration system uses the
/// attribute to determine:
/// </para>
///
/// <list type="bullet">
///   <item>The service lifetime to use when registering implementations</item>
///   <item>The minimum and maximum number of allowed implementations</item>
///   <item>Whether the concrete implementing type should also be registered</item>
/// </list>
///
/// <para>
/// The attribute is consumed by the <see cref="Planner"/>, validated by
/// <see cref="Validator"/>, and executed by <see cref="Registrar"/>.
/// </para>
/// </summary>
[AttributeUsage(AttributeTargets.Interface, Inherited = false)]
public sealed class RegistrationAttribute(ServiceLifetime lifetime) : Attribute
{
    /// <summary>
    /// Gets the <see cref="ServiceLifetime"/> that should be used when
    /// registering implementations of the decorated interface.
    /// </summary>
    public ServiceLifetime Lifetime { get; } = lifetime;

    /// <summary>
    /// Gets or sets the maximum number of allowed registrations for the
    /// decorated interface.
    ///
    /// <para>
    /// Defaults to <see cref="int.MaxValue"/>, meaning there is no upper limit
    /// unless explicitly specified.
    /// </para>
    /// </summary>
    public int MaxRegistrationCount { get; set; } = int.MaxValue;

    /// <summary>
    /// Gets or sets the minimum number of required registrations for the
    /// decorated interface.
    ///
    /// <para>
    /// Defaults to <c>0</c>, meaning the interface may have no implementations
    /// unless a higher minimum is specified.
    /// </para>
    /// </summary>
    public int MinRegistrationCount { get; set; } = 0;

    /// <summary>
    /// Gets or sets a value indicating whether the concrete implementing type
    /// should also be registered in addition to the interface mapping.
    ///
    /// <para>
    /// When <c>true</c>, the implementing class is registered as both:
    /// </para>
    /// <list type="bullet">
    ///   <item><c>ImplementedInterface → ImplementingClass</c></item>
    ///   <item><c>ImplementingClass → ImplementingClass</c></item>
    /// </list>
    ///
    /// <para>
    /// This is useful when consumers may want to resolve the concrete type directly.
    /// </para>
    /// </summary>
    public bool RegisterConcreteType { get; init; } = false;
}
