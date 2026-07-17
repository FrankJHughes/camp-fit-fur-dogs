namespace Frank.Core.Application.Abstractions.ImmutableContexts;

public interface IImmutableContextBuildStep<TContext>
    where TContext : ImmutableContextBase
{
    IImmutableContextBuildStepMetadata Metadata { get; }

    bool CanExecute(TContext context);

    Task<TContext> ExecuteAsync(TContext context, CancellationToken ct);
}
