using System.Reflection;

namespace Frank.DependencyInjection.AutoRegistration.Shapes;

public sealed record RelevantInterfaceGroup(
    TypeInfo RelevantInterface,
    IEnumerable<Implementation> Implementations
);
