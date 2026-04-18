using FluentAssertions;
using CampFitFurDogs.Api.Tests.Guardrails.Architecture;

namespace CampFitFurDogs.Api.Tests.Guardrails;

public class DomainMustNotReferenceHigherLayersGuardrailTests
{
    [Fact]
    public void Domain_Should_Not_Reference_Application_Or_Infrastructure()
    {
        var domainAssembly = typeof(CampFitFurDogs.Domain.AssemblyMarker).Assembly;

        var offenders = ReferenceScanner.FindForbiddenReferences(
            domainAssembly,
            "CampFitFurDogs.Application",
            "CampFitFurDogs.Infrastructure",
            "CampFitFurDogs.Api"
        );

        offenders.Should().BeEmpty("Domain must remain pure");
    }
}
