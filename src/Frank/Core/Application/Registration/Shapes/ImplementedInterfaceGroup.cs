using System.Reflection;

namespace Frank.Core.Application.Registration.Shapes;

public sealed record ImplementedInterfaceGroup(
    Type ImplementedInterface,
    IEnumerable<TypeInfo> ImplementingClasses
);
