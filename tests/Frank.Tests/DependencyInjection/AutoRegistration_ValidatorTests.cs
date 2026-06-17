using Frank.AutoRegistration;

namespace Frank.Tests.DependencyInjection;

public sealed class AutoRegistration_ValidatorTests
{
    [Fact]
    public void AddApplication_registers_multiple_validators_for_same_command()
    {
        var services = new ServiceCollection();

        services.AddFrank(
            new[] { typeof(Frank.Tests.DependencyInjection.Fakes.AssemblyMarker).Assembly }
        );

        using var provider = services.BuildServiceProvider();

        var validators = provider.GetServices<IValidator<Frank.Tests.DependencyInjection.Fakes.FakeCommand>>();

        validators.Should().HaveCount(2);
    }

    [Fact]
    public void AddApplication_does_not_register_abstract_validators()
    {
        var services = new ServiceCollection();

        services.AddFrank(
            new[] { typeof(Frank.Tests.DependencyInjection.Fakes.AssemblyMarker).Assembly }
        );

        using var provider = services.BuildServiceProvider();

        var abstractValidator = provider.GetService<Frank.Tests.DependencyInjection.Fakes.AbstractFakeValidator>();

        abstractValidator.Should().BeNull();
    }
}
