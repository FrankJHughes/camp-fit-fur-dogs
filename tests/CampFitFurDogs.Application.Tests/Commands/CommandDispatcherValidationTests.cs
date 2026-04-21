using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

using SharedKernel.Abstractions;
using SharedKernel.DependencyInjection;

using CampFitFurDogs.Application.Abstractions.Dogs.RegisterDog;
using CampFitFurDogs.Application.Dogs.RegisterDog;
using CampFitFurDogs.Domain.Dogs;
using CampFitFurDogs.Infrastructure;

using CampFitFurDogs.Application.Tests.Fakes;

namespace CampFitFurDogs.Application.Tests.Commands;

public class CommandDispatcherValidationTests
{
    [Fact]
    public async Task Dispatch_should_throw_when_command_is_invalid()
    {
        // Arrange
        var services = new ServiceCollection();

        services.AddSingleton<IDogRepository, FakeDogRepository>();

        var sharedKernelOptions = new SharedKernelOptions();

        services.AddSharedKernel(
            applicationAssemblies: new[]
            {
                typeof(CampFitFurDogs.Application.AssemblyMarker).Assembly
            },
            configure: options =>
            {
                sharedKernelOptions = options;

                options.AddInfrastructureAutoRegistration(
                    assemblies: new[]
                    {
                        typeof(CampFitFurDogs.Infrastructure.AssemblyMarker).Assembly
                    },
                    rules => rules
                        .Add("Repository", ServiceLifetime.Scoped)
                        .Add("Reader", ServiceLifetime.Scoped)
                        .Add("Provider", ServiceLifetime.Scoped)
                        .Add("Service", ServiceLifetime.Scoped));
            });

        services.AddSingleton<IUnitOfWork, FakeUnitOfWork>();
        services.AddSingleton<IDogRepository, FakeDogRepository>();

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
