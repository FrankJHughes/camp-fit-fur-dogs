using System.Reflection;

namespace Frank.DependencyInjection.AutoRegistration.Shapes;

public sealed record Implementation(
    TypeInfo ImplementingClass,
    Type ImplementedInterface
);
