using System.Reflection;

namespace Frank.Core.Application.Registration.Shapes;

public sealed record RelevantInterfaceGroup(
    TypeInfo RelevantInterface,
    IEnumerable<Implementation> Implementations
);
