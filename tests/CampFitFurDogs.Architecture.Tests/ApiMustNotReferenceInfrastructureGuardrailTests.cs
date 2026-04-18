using FluentAssertions;
using System.Reflection;

namespace CampFitFurDogs.Architecture.Tests;

public class ApiMustNotReferenceInfrastructureGuardrailTests
{
    [Fact]
    public void Api_Should_Not_Reference_Infrastructure()
    {
        var apiAssembly = typeof(CampFitFurDogs.Api.AssemblyMarker).Assembly;

        var offenders = ReferenceScanner.FindForbiddenReferences(
            apiAssembly,
            "CampFitFurDogs.Infrastructure"
        );

        offenders.Should().BeEmpty("API must not reference Infrastructure");
    }
}
