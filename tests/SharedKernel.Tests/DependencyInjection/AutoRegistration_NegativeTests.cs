using SharedKernel.DependencyInjection;

namespace SharedKernel.Tests.DependencyInjection;

public sealed class AutoRegistration_NegativeTests
{
    [Fact]
    public void AddApplication_throws_when_multiple_handlers_for_same_command()
    {
        var services = new ServiceCollection();

        Action act = () => services.AddSharedKernel(
            new[] { typeof(AssemblyMarker).Assembly }
        );

        act.Should().Throw<InvalidOperationException>();
    }
}

