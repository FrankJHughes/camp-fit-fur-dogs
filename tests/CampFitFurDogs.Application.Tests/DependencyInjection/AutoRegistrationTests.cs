using CampFitFurDogs.Application.Abstractions.Dogs.RegisterDog;
using CampFitFurDogs.Application.Dogs.RegisterDog;
using CampFitFurDogs.Application.Tests.Fakes;
using CampFitFurDogs.Domain.Customers;
using CampFitFurDogs.Domain.Dogs;
using FluentValidation;
using Frank;
using Frank.Abstractions.Command;
using Frank.Abstractions.UnitOfWork;
using Microsoft.Extensions.DependencyInjection;

namespace CampFitFurDogs.Application.Tests.DependencyInjection;

public partial class AutoRegistrationTests
{
    [Fact]
    public void Handlers_should_be_registered_by_convention()
    {
        // Arrange
        var services = new ServiceCollection();

        services.AddFrank(
            [typeof(CampFitFurDogs.Application.AssemblyMarker).Assembly]);


        // Stub dependencies so handlers can be constructed
        services.AddSingleton<IDogRepository, FakeDogRepository>();
        services.AddSingleton<ICustomerRepository, FakeCustomerRepository>();
        services.AddSingleton<IUnitOfWork, FakeUnitOfWork>();

        // Act
        var provider = services.BuildServiceProvider();

        services.AddSingleton<IUnitOfWork, FakeUnitOfWork>();

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

        services.AddFrank(
            [typeof(CampFitFurDogs.Application.AssemblyMarker).Assembly]);

        // Provide required fakes so handlers/validators can be constructed
        services.AddSingleton<IDogRepository, FakeDogRepository>();
        services.AddSingleton<ICustomerRepository, FakeCustomerRepository>();
        services.AddSingleton<IUnitOfWork, FakeUnitOfWork>();

        // Act
        var provider = services.BuildServiceProvider();

        // Assert
        var validator = provider.GetService<IValidator<RegisterDogCommand>>();

        validator.Should().NotBeNull();
        validator.Should().BeOfType<RegisterDogCommandValidator>();
    }
}
