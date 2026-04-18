using FluentAssertions;
using System.Reflection;
using CampFitFurDogs.Api.Tests.Guardrails.Architecture;

namespace CampFitFurDogs.Api.Tests.Guardrails;

public class ApiMayOnlyReferenceApplicationAbstractionsGuardrailTests
{
    [Fact]
    public void Api_Should_Only_Reference_Application_Abstractions()
    {
        var apiAssembly = typeof(CampFitFurDogs.Api.AssemblyMarker).Assembly;

        var forbiddenNamespaces = new[]
        {
            "CampFitFurDogs.Application.",            // everything in Application
            "CampFitFurDogs.Application"              // root namespace
        };

        var allowedPrefix = "CampFitFurDogs.Application.Abstractions";

        var offenders = ReferenceScanner
            .FindForbiddenReferences(apiAssembly, forbiddenNamespaces)
            .Where(ns => !ns.StartsWith(allowedPrefix))
            .ToList();

        offenders.Should().BeEmpty(
            "API must depend only on Application.Abstractions, not Application implementation");
    }
}
