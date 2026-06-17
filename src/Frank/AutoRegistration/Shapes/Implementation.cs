using System.Reflection;

namespace Frank.AutoRegistration.Shapes;

public sealed record Implementation(
    TypeInfo ImplementingClass,
    Type ImplementedInterface
);
