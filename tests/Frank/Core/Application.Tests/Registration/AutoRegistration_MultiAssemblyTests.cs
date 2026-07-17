using Frank.Core.Application.Abstractions.Cqrs.Commands;
using Frank.Core.Application.Abstractions.Cqrs.Queries;
using Frank.Core.Application.Cqrs.Commands;
using Frank.Core.Application.Cqrs.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Core.Application.Tests.Registration;

public sealed class AutoRegistration_MultiAssemblyTests
{
    [Fact]
    public void AddApplication_scans_multiple_assemblies()
    {
        var services = new ServiceCollection();

        services.AddFrankCqrsCommands([
            typeof(Frank.TestUtilities.ValidServices.AssemblyMarker).Assembly
        ]);

        services.AddFrankCqrsQueries([
            typeof(Frank.TestUtilities.ValidServices.AssemblyMarker).Assembly
        ]);

        using var provider = services.BuildServiceProvider();

        provider.GetService<ICommandHandler<Frank.TestUtilities.ValidServices.FakeCommand, Frank.TestUtilities.ValidServices.FakeResponse>>()
            .Should().NotBeNull();

        provider.GetService<IQueryHandler<Frank.TestUtilities.ValidServices.FakeQuery, Frank.TestUtilities.ValidServices.FakeResponse>>()
            .Should().NotBeNull();
    }
}
