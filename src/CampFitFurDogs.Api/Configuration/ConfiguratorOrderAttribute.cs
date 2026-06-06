namespace CampFitFurDogs.Api.Configuration;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class ConfiguratorAttribute : Attribute
{
    public int Order { get; }
    public ConfiguratorAttribute(int order) => Order = order;
}
