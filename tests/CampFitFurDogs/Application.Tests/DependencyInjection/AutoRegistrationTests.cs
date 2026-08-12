using CampFitFurDogs.Application.Abstractions.Dogs.RegisterDog;
using CampFitFurDogs.Application.Abstractions.UnitOfWork;
using CampFitFurDogs.Application.Dogs.RegisterDog;
using CampFitFurDogs.Application.Tests.Fakes;
using Frank.Core.Application.Abstractions.Cqrs.Commands;
using Frank.Core.Application.Cqrs.Commands;
using Frank.Identity.Application.Abstractions.Users;
using Microsoft.Extensions.DependencyInjection;

namespace CampFitFurDogs.Application.Tests.DependencyInjection;

public partial class AutoRegistrationTests
{
    private sealed class FakeCurrentUser : ICurrentUser
    {
        public Guid? Id { get; init; }

        public bool IsAuthenticated => throw new NotImplementedException();

        public string? Name => throw new NotImplementedException();

        public FakeCurrentUser(Guid id)
        {
            Id = id;
        }
    }

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
        handler.Should().BeOfType<RegisterDogCommandHandler>();
    }

    [Fact]
    public void Validators_should_be_registered_by_convention()
    {
        // Arrange
        var services = new ServiceCollection();

        services.AddValidatorsFromAssemblies(
            [typeof(CampFitFurDogs.Application.AssemblyMarker).Assembly]);

        // Provide required fakes so validators can be constructed
        services.AddSingleton<IRegisterDogWriter, FakeRegisterDogWriter>();
        services.AddSingleton<IAppUnitOfWork, FakeAppUnitOfWork>();

        // Provide fake current user for owner‑scoped validators
        services.AddSingleton<ICurrentUser>(new FakeCurrentUser(Guid.NewGuid()));

        // Act
        var provider = services.BuildServiceProvider();

        // Assert
        var validator = provider.GetService<IValidator<RegisterDogCommand>>();

        validator.Should().NotBeNull();
        validator.Should().BeOfType<RegisterDogCommandValidator>();
    }
}
