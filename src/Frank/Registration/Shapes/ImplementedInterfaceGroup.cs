using System.Reflection;

namespace Frank.Registration.Shapes;

public sealed record ImplementedInterfaceGroup(
    Type ImplementedInterface,
    IEnumerable<TypeInfo> ImplementingClasses
);
