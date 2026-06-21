using System.Reflection;

namespace Frank.AutoRegistration.Shapes;

public sealed record ImplementedInterfaceGroup(
    Type ImplementedInterface,
    IEnumerable<TypeInfo> ImplementingClasses
);
