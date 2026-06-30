using System.Reflection;

namespace Frank.Registration.Shapes;

public sealed record Implementation(
    TypeInfo ImplementingClass,
    Type ImplementedInterface
);
