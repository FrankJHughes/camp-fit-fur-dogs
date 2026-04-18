using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using CampFitFurDogs.Application.Abstractions;
using CampFitFurDogs.Application.Abstractions.Dogs.RegisterDog;
using CampFitFurDogs.Application.DependencyInjection;
using CampFitFurDogs.Application.Dogs.RegisterDog;
using CampFitFurDogs.Application.Tests.Fakes;
using CampFitFurDogs.Domain.Dogs;

namespace CampFitFurDogs.Application.Tests.Commands;

public class CommandDispatcherValidationTests
{
    [Fact]
    public async Task Dispatch_should_throw_when_command_is_invalid()
    {
        // Arrange
        var services = new ServiceCollection();

        services.AddSingleton<IDogRepository, FakeDogRepository>();
        services.AddApplicationServices();

        var provider = services.BuildServiceProvider();
        var dispatcher = provider.GetRequiredService<ICommandDispatcher>();

        var invalid = new RegisterDogCommand(
            OwnerId: Guid.Empty,   // invalid
            Name: "",              // invalid
            Breed: "Labrador",
            DateOfBirth: DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1)),
            Sex: "Male"
        );

        // Act
        var act = () => dispatcher.DispatchAsync(invalid, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }
}
