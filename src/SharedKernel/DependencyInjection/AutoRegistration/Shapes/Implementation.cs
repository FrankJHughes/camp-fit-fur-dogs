using System.Reflection;

namespace SharedKernel.DependencyInjection.AutoRegistration.Shapes;

public sealed record Implementation(
    TypeInfo ImplementingClass,
    Type ImplementedInterface
);
