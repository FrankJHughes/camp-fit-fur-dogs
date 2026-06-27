namespace Frank.Abstractions.Problem;

[AttributeUsage(AttributeTargets.Class)]
public sealed class ExceptionHandlerAttribute : Attribute
{
    public int Order { get; }

    public ExceptionHandlerAttribute(int order)
    {
        Order = order;
    }
}
