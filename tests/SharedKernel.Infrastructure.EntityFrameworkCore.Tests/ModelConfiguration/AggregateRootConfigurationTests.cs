using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using SharedKernel.Infrastructure.EntityFrameworkCore.Tests.Fakes;
using Xunit;

namespace SharedKernel.Infrastructure.EntityFrameworkCore.Tests.ModelConfiguration;

public sealed class AggregateRootConfigurationTests
{
    [Fact]
    public void Base_configuration_applies_expected_defaults_and_invokes_derived_method()
    {
        var modelBuilder = new ModelBuilder();
        var configuration = new FakeAggregateConfiguration();

        configuration.Configure(modelBuilder.Entity<FakeAggregate>());

        var entity = modelBuilder.Model.FindEntityType(typeof(FakeAggregate));
        entity.Should().NotBeNull();

        entity!.GetTableName().Should().Be(""FakeAggregates"");
        entity.FindPrimaryKey()!.Properties.Single().Name.Should().Be(""Id"");
        entity.FindProperty(""Id"")!.ValueGenerated.Should().Be(ValueGenerated.Never);
        entity.FindProperty(""DomainEvents"").Should().BeNull();
        configuration.ConfigureCalled.Should().BeTrue();
        entity.FindProperty(""Name"").Should().NotBeNull();
    }
}
