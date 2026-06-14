namespace Frank.Abstractions.Hosting;

[AttributeUsage(AttributeTargets.Class)]
public sealed class HostingModuleAttribute : Attribute
{
    public int Order { get; }

    public HostingModuleAttribute(int order)
    {
        Order = order;
    }
}
