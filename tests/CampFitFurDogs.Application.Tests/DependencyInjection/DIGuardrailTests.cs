using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

using SharedKernel.DependencyInjection;

namespace CampFitFurDogs.Application.Tests.DependencyInjection;

public class DiGuardrailTests
{
    private static readonly string[] AllowedInterfaceOnly =
    {
        "ICommandDispatcher",
        "IQueryDispatcher",
        "IDomainEventDispatcher"
    };

    [Fact]
    public void All_Application_Registrations_Must_Include_Concrete_Types()
    {
        // Arrange
        var services = new ServiceCollection();

        services.AddSharedKernel(
            [typeof(CampFitFurDogs.Application.AssemblyMarker).Assembly]);

        var descriptors = services.ToList();

        var offenders =
            from d in descriptors
            where d.ServiceType.IsInterface
            let impl = d.ImplementationType
            where impl is not null
                  && impl.Namespace is not null
                  && impl.Namespace.StartsWith("CampFitFurDogs.Application")
                  && !AllowedInterfaceOnly.Contains(d.ServiceType.Name)
                  && !descriptors.Any(x => x.ServiceType == impl)
            select $"{d.ServiceType.Name} -> {impl.Name}";

        Assert.False(offenders.Any(),
            "Found interface-only registrations:\n" +
            string.Join("\n", offenders));
    }
}
