using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel.Domain;

namespace SharedKernel.Infrastructure.EntityFrameworkCore.Configurations;

public abstract class AggregateRootConfiguration<TAggregateRoot, TId>
    : IEntityTypeConfiguration<TAggregateRoot>
    where TAggregateRoot : AggregateRoot<TId>
    where TId : notnull
{
    public void Configure(EntityTypeBuilder<TAggregateRoot> builder)
    {
        builder.ToTable(TableName);
        builder.HasKey(a => a.Id);
        builder.Ignore(a => a.DomainEvents);
        ConfigureAggregateRoot(builder);
    }

    protected abstract string TableName { get; }

    protected abstract void ConfigureAggregateRoot(EntityTypeBuilder<TAggregateRoot> builder);
}
