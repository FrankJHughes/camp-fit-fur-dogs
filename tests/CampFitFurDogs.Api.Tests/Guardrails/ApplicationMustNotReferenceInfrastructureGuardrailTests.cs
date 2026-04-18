using FluentAssertions;
using CampFitFurDogs.Api.Tests.Guardrails.Architecture;

namespace CampFitFurDogs.Api.Tests.Guardrails;

public class ApplicationMustNotReferenceInfrastructureGuardrailTests
{
    [Fact]
    public void Application_Should_Not_Reference_Infrastructure()
    {
        var appAssembly = typeof(CampFitFurDogs.Application.DependencyInjection.DependencyInjection).Assembly;

        var offenders = ReferenceScanner.FindForbiddenReferences(
            appAssembly,
            "CampFitFurDogs.Infrastructure"
        );

        offenders.Should().BeEmpty("Application must never reference Infrastructure");
    }
}
