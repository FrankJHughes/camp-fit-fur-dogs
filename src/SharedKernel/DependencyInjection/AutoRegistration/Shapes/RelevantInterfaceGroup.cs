using System.Reflection;

namespace SharedKernel.DependencyInjection.AutoRegistration.Shapes;

public sealed record RelevantInterfaceGroup(
    TypeInfo RelevantInterface,
    IEnumerable<Implementation> Implementations
);
