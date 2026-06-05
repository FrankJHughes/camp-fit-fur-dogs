using System.Reflection;

namespace Frank.DependencyInjection.AutoRegistration.Shapes;

public sealed record Plan(
    AutoRegisterAttribute AutoRegisterAttribute,
    Type ImplementedInterface,
    IEnumerable<TypeInfo> ImplementingClasses
);
