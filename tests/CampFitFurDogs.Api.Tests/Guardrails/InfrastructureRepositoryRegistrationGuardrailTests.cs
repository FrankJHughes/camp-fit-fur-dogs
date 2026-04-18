using FluentAssertions;

namespace CampFitFurDogs.Api.Tests.Guardrails;

public class InfrastructureRepositoryRegistrationGuardrailTests
    : GuardrailTestBase, IClassFixture<CampFitFurDogsApiFactory>
{
    public InfrastructureRepositoryRegistrationGuardrailTests(CampFitFurDogsApiFactory factory)
        : base(factory) { }

    [Fact]
    public void Should_Register_All_Repositories()
    {
        var assembly = typeof(CampFitFurDogs.Infrastructure.DependencyInjection).Assembly;

        var repoTypes = assembly
            .GetTypes()
            .Where(t =>
                t.IsClass &&
                !t.IsAbstract &&
                t.Name.EndsWith("Repository"))
            .ToList();

        repoTypes.Should().NotBeEmpty("repositories must exist to test");

        foreach (var repoType in repoTypes)
        {
            var iface = repoType.GetInterfaces().FirstOrDefault();
            iface.Should().NotBeNull($"Repository {repoType.Name} must implement an interface");

            var resolved = GetAll(iface);
            resolved.Should().NotBeEmpty($"Repository {repoType.Name} must be registered in DI");
        }
    }
}
