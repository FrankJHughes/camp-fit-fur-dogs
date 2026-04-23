using System.Linq;
using SharedKernel.Abstractions;
using Xunit;

namespace CampFitFurDogs.Architecture.Tests.Identity;

public class CurrentUserServiceGuardrailTests
{
    [Fact]
    public void OnlyOne_ICurrentUserService_Implementation_Should_Exist_In_Infrastructure()
    {
        // Arrange
        var infraAssembly = typeof(CampFitFurDogs.Infrastructure.AssemblyMarker).Assembly;

        // Act
        var implementations = infraAssembly
            .GetTypes()
            .Where(t =>
                typeof(ICurrentUserService).IsAssignableFrom(t) &&
                !t.IsInterface &&
                !t.IsAbstract)
            .ToList();

        // Assert
        Assert.Single(implementations);
    }
}
