using System.Reflection;

namespace Frank.AutoRegistration.Shapes;

public sealed record RelevantInterfaceGroup(
    TypeInfo RelevantInterface,
    IEnumerable<Implementation> Implementations
);
