using Frank.AutoRegistration;

namespace Frank.Tests.DependencyInjection;

public sealed class AutoRegistration_NegativeTests
{
    [Fact]
    public void AddApplication_throws_when_multiple_handlers_for_same_command()
    { // THIS TEST REQUIRES A DEDICATED ASSEMBLY HAVING THIS CONDITION


        var services = new ServiceCollection();

        Action act = () => services.AddFrank(
            [
                typeof(Frank.Tests.DependencyInjection.DuplicateCommandHandlers.AssemblyMarker).Assembly
            ]
        );

        act.Should().Throw<InvalidOperationException>();
    }
}

