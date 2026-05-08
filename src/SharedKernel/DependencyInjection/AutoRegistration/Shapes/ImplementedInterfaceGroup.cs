using System.Reflection;

namespace SharedKernel.DependencyInjection.AutoRegistration.Shapes;

public sealed record ImplementedInterfaceGroup(
    Type ImplementedInterface,
    IEnumerable<TypeInfo> ImplementingClasses
);
