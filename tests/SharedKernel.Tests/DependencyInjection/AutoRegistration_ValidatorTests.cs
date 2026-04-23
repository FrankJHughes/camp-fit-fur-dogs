using SharedKernel.DependencyInjection;

namespace SharedKernel.Tests.DependencyInjection;

public sealed class AutoRegistration_ValidatorTests
{
    [Fact]
    public void AddApplication_registers_multiple_validators_for_same_command()
    {
        var services = new ServiceCollection();

        services.AddSharedKernel(
            new[] { typeof(SharedKernel.Tests.DependencyInjection.Fakes.AssemblyMarker).Assembly }
        );

        using var provider = services.BuildServiceProvider();

        var validators = provider.GetServices<IValidator<SharedKernel.Tests.DependencyInjection.Fakes.FakeCommand>>();

        validators.Should().HaveCount(2);
    }

    [Fact]
    public void AddApplication_does_not_register_abstract_validators()
    {
        var services = new ServiceCollection();

        services.AddSharedKernel(
            new[] { typeof(SharedKernel.Tests.DependencyInjection.Fakes.AssemblyMarker).Assembly }
        );

        using var provider = services.BuildServiceProvider();

        var abstractValidator = provider.GetService<SharedKernel.Tests.DependencyInjection.Fakes.AbstractFakeValidator>();

        abstractValidator.Should().BeNull();
    }
}
