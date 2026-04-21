using FluentAssertions;
using System.Reflection;

namespace CampFitFurDogs.Architecture.Tests;

public class CommandsQueriesMustLiveInAbstractionsGuardrailTests
{
    [Fact]
    public void Commands_And_Queries_Must_Live_In_Application_Abstractions()
    {
        var sharedKernelAssembly = typeof(SharedKernel.AssemblyMarker).Assembly;

        // Find ICommand<T> and IQuery<T> interfaces
        var commandInterface = sharedKernelAssembly
            .GetTypes()
            .FirstOrDefault(t =>
                t.IsInterface &&
                t.IsGenericTypeDefinition &&
                t.Name == "ICommand`1");

        var queryInterface = sharedKernelAssembly
            .GetTypes()
            .FirstOrDefault(t =>
                t.IsInterface &&
                t.IsGenericTypeDefinition &&
                t.Name == "IQuery`1");

        commandInterface.Should().NotBeNull("ICommand<TResponse> must exist");
        queryInterface.Should().NotBeNull("IQuery<TResponse> must exist");

        var appAssembly = typeof(SharedKernel.AssemblyMarker).Assembly;

        var allTypes = appAssembly.GetTypes();

        // Find all commands/queries by interface assignment
        var commandQueryTypes = allTypes
            .Where(t =>
                t.IsClass &&
                !t.IsAbstract &&
                (
                    t.GetInterfaces().Any(i =>
                        i.IsGenericType &&
                        (
                            i.GetGenericTypeDefinition() == commandInterface ||
                            i.GetGenericTypeDefinition() == queryInterface
                        )
                    )
                )
            )
            .ToList();

        // Commands/Queries must live in Application.Abstractions assembly
        var offenders = commandQueryTypes
            .Where(t => t.Assembly != appAssembly ||
                !string.IsNullOrWhiteSpace(t.FullName) &&
                !t.FullName.StartsWith("CampFitFurDogs.Application.Abstractions."))
            .Select(t => t.FullName!)
            .ToList();

        offenders.Should().BeEmpty(
            "all Commands and Queries must live in Application.Abstractions, not Application implementation");
    }
}
