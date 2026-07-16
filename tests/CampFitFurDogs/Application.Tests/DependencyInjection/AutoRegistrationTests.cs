using CampFitFurDogs.Application.Abstractions;
using CampFitFurDogs.Application.Abstractions.Dog.RegisterDog;
using CampFitFurDogs.Application.Dogs.RegisterDog;
using CampFitFurDogs.Application.Tests.Fakes;
using CampFitFurDogs.Domain.Dogs;
using Frank.Core.Application;
using Frank.Core.Application.Abstractions.Command;
using Frank.Core.Application.Abstractions.UnitOfWork;
using Frank.Core.Application.Command;
using Frank.Identity.Domain.Users;
using Frank.TestUtilities.Fakes;
using Microsoft.Extensions.DependencyInjection;

namespace CampFitFurDogs.Application.Tests.DependencyInjection;

public partial class AutoRegistrationTests
{
    [Fact]
    public void Handlers_should_be_registered_by_convention()
    {
        // Arrange
        var services = new ServiceCollection();

        services.AddFrankCommands([
            typeof(CampFitFurDogs.Application.AssemblyMarker).Assembly
        ]);


        // Stub dependencies so handlers can be constructed
        services.AddSingleton<IDogRepository, FakeDogRepository>();
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

        services.AddFrankValidators(
            [typeof(CampFitFurDogs.Application.AssemblyMarker).Assembly]);

        // Provide required fakes so handlers/validators can be constructed
        services.AddSingleton<IDogRepository, FakeDogRepository>();
        services.AddSingleton<IUserRepository, FakeUserRepository>();
        services.AddSingleton<IUnitOfWork, FakeAppUnitOfWork>();

        // Act
        var provider = services.BuildServiceProvider();

        // Assert
        var validator = provider.GetService<IValidator<RegisterDogCommand>>();

        validator.Should().NotBeNull();
        validator.Should().BeOfType<RegisterDogCommandValidator>();
    }
}
