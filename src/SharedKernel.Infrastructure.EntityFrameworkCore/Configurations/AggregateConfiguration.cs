using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel.Domain;

namespace SharedKernel.Infrastructure.EntityFrameworkCore.Configurations;

public abstract class AggregateRootConfiguration<TAggregateRoot, TId>
    : IEntityTypeConfiguration<TAggregateRoot>
    where TAggregateRoot : AggregateRoot<TId>
    where TId : AggregateId
{
    public void Configure(EntityTypeBuilder<TAggregateRoot> builder)
    {
        builder.ToTable(TableName);

        // Explicit key mapping (works even if Id has a private setter)
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .ValueGeneratedNever();

        // Domain events are never persisted
        builder.Ignore(a => a.DomainEvents);

        // Allow derived configurations to add properties, relationships, indexes, etc.
        ConfigureAggregateRoot(builder);
    }

    protected abstract string TableName { get; }

    protected abstract void ConfigureAggregateRoot(EntityTypeBuilder<TAggregateRoot> builder);
}
