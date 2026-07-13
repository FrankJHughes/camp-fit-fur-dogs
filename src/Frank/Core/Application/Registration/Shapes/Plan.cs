using System.Reflection;

namespace Frank.Core.Application.Registration.Shapes;

public sealed record Plan(
    RegistrationAttribute AutoRegisterAttribute,
    Type ImplementedInterface,
    IEnumerable<TypeInfo> ImplementingClasses
);
