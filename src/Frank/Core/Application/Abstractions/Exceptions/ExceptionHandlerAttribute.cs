namespace Frank.Core.Application.Abstractions.Exceptions;

/// <summary>
/// Indicates that a class is an exception handler and defines its execution
/// order within the exception‑handling pipeline.
///
/// <para>
/// Exception handlers marked with <see cref="ExceptionHandlerAttribute"/> can be
/// discovered and executed in a deterministic sequence. The <see cref="Order"/>
/// property allows multiple handlers to be composed, ensuring that higher‑order
/// handlers run earlier or later depending on the desired behavior.
/// </para>
///
/// <para>
/// This attribute is intended for classes that participate in centralized
/// exception processing, such as logging, mapping exceptions to responses, or
/// applying domain‑specific error handling policies.
/// </para>
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class ExceptionHandlerAttribute : Attribute
{
    /// <summary>
    /// Gets the execution order for the exception handler.
    ///
    /// <para>
    /// Lower values typically indicate earlier execution, while higher values
    /// run later in the pipeline. The exact ordering semantics depend on the
    /// dispatcher or middleware that consumes this attribute.
    /// </para>
    /// </summary>
    public int Order { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExceptionHandlerAttribute"/>
    /// class with the specified execution order.
    /// </summary>
    /// <param name="order">
    /// The execution order for the handler.
    /// </param>
    public ExceptionHandlerAttribute(int order)
    {
        Order = order;
    }
}
