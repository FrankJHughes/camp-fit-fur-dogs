using FluentAssertions;
using CampFitFurDogs.Api.Tests.Guardrails.Architecture;

namespace CampFitFurDogs.Api.Tests.Guardrails;

public class InfrastructureMustNotReferenceApiGuardrailTests
{
    [Fact]
    public void Infrastructure_Should_Not_Reference_Api()
    {
        var infraAssembly = typeof(CampFitFurDogs.Infrastructure.DependencyInjection).Assembly;

        var offenders = ReferenceScanner.FindForbiddenReferences(
            infraAssembly,
            "CampFitFurDogs.Api"
        );

        offenders.Should().BeEmpty("Infrastructure must never reference API");
    }
}
