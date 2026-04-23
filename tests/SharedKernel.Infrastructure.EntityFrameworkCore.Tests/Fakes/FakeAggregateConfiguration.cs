using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel.Infrastructure.EntityFrameworkCore;
using SharedKernel.Infrastructure.EntityFrameworkCore.Tests.Fakes;

namespace SharedKernel.Infrastructure.EntityFrameworkCore.Tests.Fakes;

public sealed class FakeAggregateConfiguration
    : AggregateRootConfiguration<FakeAggregate, FakeAggregateId>
{
    public bool ConfigureCalled { get; private set; }

    protected override string TableName => ""FakeAggregates"";

    protected override void ConfigureAggregateRoot(EntityTypeBuilder<FakeAggregate> builder)
    {
        ConfigureCalled = true;
        builder.Property(x => x.Name);
    }
}
