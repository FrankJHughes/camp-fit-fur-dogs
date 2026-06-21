using System.Reflection;

namespace Frank.AutoRegistration.Shapes;

public sealed record Plan(
    AutoRegisterAttribute AutoRegisterAttribute,
    Type ImplementedInterface,
    IEnumerable<TypeInfo> ImplementingClasses
);
