namespace CampFitFurDogs.Api.Horizontals.Startup;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class StartupModuleAttribute : Attribute
{
    public int Order { get; }
    public StartupModuleAttribute(int order) => Order = order;
}
