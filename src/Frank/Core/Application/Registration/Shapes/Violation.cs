namespace Frank.Core.Application.Registration.Shapes;

/// <summary>
/// Represents a registration‑rule violation discovered during validation of a
/// <see cref="Plan"/>.
///
/// <para>
/// A violation occurs when the number of discovered implementations for an
/// interface falls outside the allowed range defined by the interface’s
/// <see cref="RegistrationAttribute"/>:
/// </para>
///
/// <code>
/// MinRegistrationCount ≤ ActualRegistrationCount ≤ MaxRegistrationCount
/// </code>
///
/// <para>
/// Violations are surfaced by <see cref="Registration.Validator"/> and consumed
/// by the <see cref="Registration.Orchestrator"/>, which throws an exception
/// containing formatted diagnostic output when any violations exist.
/// </para>
///
/// <para>
/// This record contains no behavior; it is a structural carrier used to report
/// validation failures in a clear and consistent manner.
/// </para>
/// </summary>
public sealed record Violation(
    /// <summary>
    /// The registration plan that failed validation.
    /// Contains the interface being registered, the implementing classes, and
    /// the associated <see cref="RegistrationAttribute"/> defining constraints.
    /// </summary>
    Plan Plan,

    /// <summary>
    /// The number of implementations actually discovered for the interface.
    /// </summary>
    int ActualRegistrationCount,

    /// <summary>
    /// The minimum number of implementations required by the registration rules.
    /// </summary>
    int MinRegistrationCount,

    /// <summary>
    /// The maximum number of implementations allowed by the registration rules.
    /// </summary>
    int MaxRegistrationCount
);
