namespace Frank.Abstractions.ImmutableContext;

public interface IImmutableContextBuilder<TRequest, TContext, TResult>
    where TRequest : ImmutableContextBuilderRequestBase
    where TContext : ImmutableContextBase
    where TResult : ImmutableContextBuilderResultBase
{
    Task<TResult> BuildAsync(TRequest request, CancellationToken ct);
}
