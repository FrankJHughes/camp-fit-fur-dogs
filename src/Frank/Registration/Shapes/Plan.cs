using System.Reflection;

namespace Frank.Registration.Shapes;

public sealed record Plan(
    AutoRegisterAttribute AutoRegisterAttribute,
    Type ImplementedInterface,
    IEnumerable<TypeInfo> ImplementingClasses
);
