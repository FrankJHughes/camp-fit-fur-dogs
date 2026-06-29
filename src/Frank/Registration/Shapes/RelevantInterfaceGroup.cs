using System.Reflection;

namespace Frank.Registration.Shapes;

public sealed record RelevantInterfaceGroup(
    TypeInfo RelevantInterface,
    IEnumerable<Implementation> Implementations
);
