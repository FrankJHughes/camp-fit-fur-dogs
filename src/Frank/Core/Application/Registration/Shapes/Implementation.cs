using System.Reflection;

namespace Frank.Core.Application.Registration.Shapes;

public sealed record Implementation(
    TypeInfo ImplementingClass,
    Type ImplementedInterface
);
