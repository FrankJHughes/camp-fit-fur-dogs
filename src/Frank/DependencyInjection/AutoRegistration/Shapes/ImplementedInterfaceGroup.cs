using System.Reflection;

namespace Frank.DependencyInjection.AutoRegistration.Shapes;

public sealed record ImplementedInterfaceGroup(
    Type ImplementedInterface,
    IEnumerable<TypeInfo> ImplementingClasses
);
