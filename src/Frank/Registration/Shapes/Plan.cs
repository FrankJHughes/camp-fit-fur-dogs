using System.Reflection;

namespace Frank.Registration.Shapes;

public sealed record Plan(
    RegistrationAttribute AutoRegisterAttribute,
    Type ImplementedInterface,
    IEnumerable<TypeInfo> ImplementingClasses
);
