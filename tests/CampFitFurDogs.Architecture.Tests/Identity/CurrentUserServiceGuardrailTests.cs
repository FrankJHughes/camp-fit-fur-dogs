using Frank.Identity.Abstractions;

namespace CampFitFurDogs.Architecture.Tests.Identity;

public class CurrentUserGuardrailTests
{
    [Fact]
    public void OnlyOne_ICurrentUser_Implementation_Should_Exist_In_Infrastructure()
    {
        // Arrange
        var infraAssembly = typeof(Frank.Core.Infrastructure.AssemblyMarker).Assembly;

        // Act
        var implementations = infraAssembly
            .GetTypes()
            .Where(t =>
                typeof(ICurrentUser).IsAssignableFrom(t) &&
                !t.IsInterface &&
                !t.IsAbstract)
            .ToList();

        // Assert
        Assert.Single(implementations);
    }
}
