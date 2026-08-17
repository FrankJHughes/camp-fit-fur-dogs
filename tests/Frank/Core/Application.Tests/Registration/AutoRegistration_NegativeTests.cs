using Frank.Core.Application.Cqrs.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Core.Application.Tests.Registration;

public sealed class AutoRegistration_NegativeTests
{
    [Fact]
    public void AddApplication_throws_when_multiple_handlers_for_same_command()
    { // THIS TEST REQUIRES A DEDICATED ASSEMBLY HAVING THIS CONDITION


        var services = new ServiceCollection();

        Action act = () => services.AddFrankCoreApplicationCqrsCommands([
            typeof(Frank.TestUtilities.InvalidServices.AssemblyMarker).Assembly
        ]);

        act.Should().Throw<InvalidOperationException>();
    }
}

