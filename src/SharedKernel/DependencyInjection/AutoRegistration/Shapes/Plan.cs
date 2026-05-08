using System.Reflection;

namespace SharedKernel.DependencyInjection.AutoRegistration.Shapes;

public sealed record Plan(
    AutoRegisterAttribute AutoRegisterAttribute,
    Type ImplementedInterface,
    IEnumerable<TypeInfo> ImplementingClasses
);
