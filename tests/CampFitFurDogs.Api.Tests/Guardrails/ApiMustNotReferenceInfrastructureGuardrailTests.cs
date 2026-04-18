using FluentAssertions;
using System.Reflection;
using CampFitFurDogs.Api.Tests.Guardrails.Architecture;

namespace CampFitFurDogs.Api.Tests.Guardrails;

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
