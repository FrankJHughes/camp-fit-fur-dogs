using CampFitFurDogs.Application.Abstractions;
using CampFitFurDogs.Application.Abstractions.Dogs;
using CampFitFurDogs.Application.Abstractions.Dogs.RegisterDog;
using CampFitFurDogs.Application.Dogs.RegisterDog;
using CampFitFurDogs.Application.Tests.Fakes;
using Frank.Core.Application.Abstractions.Cqrs.Commands;
using Frank.Core.Application.Cqrs.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace CampFitFurDogs.Application.Tests.DependencyInjection;

public partial class AutoRegistrationTests
{
    [Fact]
    public void Handlers_should_be_registered_by_convention()
    {
        // Arrange
        var services = new ServiceCollection();

        services.AddFrankCoreApplicationCqrsCommands([
            typeof(CampFitFurDogs.Application.AssemblyMarker).Assembly
        ]);


        // Stub dependencies so handlers can be constructed
        services.AddSingleton<IRegisterDogWriter, FakeRegisterDogWriter>();
        services.AddSingleton<IAppUnitOfWork, FakeAppUnitOfWork>();

        // Act
        var provider = services.BuildServiceProvider();

        // Assert
        var handler = provider.GetService<ICommandHandler<RegisterDogCommand, Guid>>();

        handler.Should().NotBeNull();
        handler.Should().BeOfType<RegisterDogHandler>();
    }

    [Fact]
    public void Validators_should_be_registered_by_convention()
    {
        // Arrange
        var services = new ServiceCollection();

        services.AddValidatorsFromAssemblies(
            [typeof(CampFitFurDogs.Application.AssemblyMarker).Assembly]);

        // Provide required fakes so handlers/validators can be constructed
        services.AddSingleton<IRegisterDogWriter, FakeRegisterDogWriter>();
        services.AddSingleton<IAppUnitOfWork, FakeAppUnitOfWork>();

        // Act
        var provider = services.BuildServiceProvider();

        // Assert
        var validator = provider.GetService<IValidator<RegisterDogCommand>>();

        validator.Should().NotBeNull();
        validator.Should().BeOfType<RegisterDogCommandValidator>();
    }
}
