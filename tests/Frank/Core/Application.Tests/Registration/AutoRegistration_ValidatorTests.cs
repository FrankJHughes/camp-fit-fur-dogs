using Microsoft.Extensions.DependencyInjection;

namespace Frank.Core.Application.Tests.Registration;

public sealed class AutoRegistration_ValidatorTests
{
    [Fact]
    public void AddApplication_registers_multiple_validators_for_same_command()
    {
        var services = new ServiceCollection();

        services.AddValidatorsFromAssemblies(
            [typeof(Frank.TestUtilities.ValidServices.AssemblyMarker).Assembly]
        );

        using var provider = services.BuildServiceProvider();

        var validators = provider.GetServices<IValidator<Frank.TestUtilities.ValidServices.FakeCommand>>();

        validators.Should().HaveCount(2);
    }

    [Fact]
    public void AddApplication_does_not_register_abstract_validators()
    {
        var services = new ServiceCollection();

        services.AddValidatorsFromAssemblies(
            [typeof(Frank.TestUtilities.ValidServices.AssemblyMarker).Assembly]
        );

        using var provider = services.BuildServiceProvider();

        var abstractValidator = provider.GetService<Frank.TestUtilities.ValidServices.AbstractFakeValidator>();

        abstractValidator.Should().BeNull();
    }
}
